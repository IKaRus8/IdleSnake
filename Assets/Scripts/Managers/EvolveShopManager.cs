using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;

namespace Managers
{
    public class EvolveShopManager : Singleton<EvolveShopManager>
    {
        // [SerializeField]
        // private GameObject _skillsPanel;

        [Serializable]
        public class Node
        {
            public int number;
            public int[] prev;
            public int[] block;
            public int costGrade;
            public GameObject openImageSkill;
            public GameObject pathfindingBlock;
            public Transform skillTransform;
            public Sprite skillSprite;
            public string skillDescription;
            public string skillName;
            public GameObject lockSkillImage;
            public GameObject[] lockedGrades;
            public GameObject[] openedGrades;
            public UnityEvent openAction;
        }

        public Node[] tree;
        private int _costResetSkill = 10;
        private static EvolveGrades grades;

        [SerializeField]
        private TextMeshProUGUI _needUpgrade;
        
        [SerializeField]
        private GameObject _needToUpgradePanel;

        [SerializeField]
        private TextMeshProUGUI _costText;


        [SerializeField]
        private GameObject _strokeOfMainSkill;

        [SerializeField]
        private Sprite _lockSprite;
        [SerializeField]
        private Sprite _egg;

        [SerializeField]
        private GameObject _resetButton;

        [SerializeField]
        private GameObject _closeResetButton;

        [SerializeField]
        private Image _mainSkillImage;

        [SerializeField]
        private TextMeshProUGUI _mainSkillDescription;

        [SerializeField]
        private TextMeshProUGUI _mainSkillName;


        [SerializeField]
        private GameObject _buyButton;

        [SerializeField]
        private GameObject _lockedBuyText;

        private static int chosenSkill;

        private void Start()
        {
            if (PlayerPrefs.HasKey("EvolveGrade"))
            {
                grades = JsonUtility.FromJson<EvolveGrades>(PlayerPrefs.GetString("EvolveGrade"));
            }

            Debug.Log(grades);
            grades ??= new EvolveGrades(tree.Length);

            if (grades.isOpen.Length!=tree.Length)
            {
                grades = new EvolveGrades(tree.Length);
                PlayerPrefs.SetString("version_0_3", "");
            }


            UpdateViewTree();
            UIManager.Instance.changeShopEventSecond += () =>
            {
                Instance.ChoseSkill(0);
            };
        }

        private void UpdateViewTree()
        {
            foreach (var node in tree)
            {
                if (grades.isOpen[node.number])
                {
                    OpenSkill(node);
                    continue;
                }

                bool isPrevOpen = (node.prev.Any(prev => grades.isOpen[prev]) && node.skillName.StartsWith("Pathfinding") || (node.prev.Count(prev => grades.isOpen[prev]) == node.prev.Length));
                bool isBlockOpen = node.block.Any(prev => grades.isOpen[prev]);
                bool isNeedOpen = isPrevOpen && !isBlockOpen;
                node.lockSkillImage.SetActive(!isNeedOpen);
                if (node.pathfindingBlock != null && node.pathfindingBlock.activeSelf)
                {
                    node.pathfindingBlock.SetActive(false);
                }
                if (isBlockOpen)
                {
                    node.lockSkillImage.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                }
                else if (!isNeedOpen && node.number>0 &&  grades.isOpen[node.prev[0]])
                {
                    node.pathfindingBlock.SetActive(true);
                }
            }
            AncestorsManager.Instance.ChangeResetButton(!grades.isOpen[tree.Length - 1]);

            tree[0].lockSkillImage.SetActive(false);
        }

        private void OpenSkill(Node skill)
        {
            foreach (GameObject lockedObject in skill.lockedGrades)
            {
                lockedObject.SetActive(false);
            }

            foreach (GameObject openedObject in skill.openedGrades)
            {
                openedObject.SetActive(true);
            }

            skill.openImageSkill.SetActive(true);
        }

        private void ResetSkill(Node skill)
        {
            foreach (GameObject lockedObject in skill.lockedGrades)
            {
                lockedObject.SetActive(true);
            }

            foreach (GameObject openedObject in skill.openedGrades)
            {
                openedObject.SetActive(false);
            }

            skill.openImageSkill.SetActive(false);
        }

        public void OpenAncestor()
        {

            tree[15].skillName = "Maturity";
            tree[15].skillSprite = _egg;
            tree[15].skillTransform.GetComponent<Image>().sprite = _egg;
            tree[15].openImageSkill.GetComponent<Image>().sprite = _egg;
        }
        public void ChoseSkill(int i)
        {
            chosenSkill = i;
            _strokeOfMainSkill.transform.position = tree[i].skillTransform.position;
            _mainSkillImage.sprite = tree[i].skillSprite;
            _mainSkillDescription.text = tree[i].skillDescription;
            _mainSkillName.text = tree[i].skillName;

            if (grades.isOpen[i])
            {
                _buyButton.SetActive(false);
                _lockedBuyText.SetActive(false);
                _resetButton.SetActive(IAPManager.IsResetProgressBuying);
                _closeResetButton.SetActive(!IAPManager.IsResetProgressBuying);
            }
            else if (i == 0 || ((tree[i].prev.Any(prev => grades.isOpen[prev]) && tree[i].skillName.StartsWith("Pathfinding") || (tree[i].prev.Count(prev => grades.isOpen[prev]) == tree[i].prev.Length)) &&
                                !tree[i].block.Any(prev => grades.isOpen[prev])))
            {
                _buyButton.SetActive(true);
                _resetButton.SetActive(false);
                _closeResetButton.SetActive(false);
                _lockedBuyText.SetActive(false);
                _costText.text = tree[i].costGrade.ToString();
            }
            else if (tree[i].block.Any(prev => grades.isOpen[prev]))
            {
                _buyButton.SetActive(false);
                _resetButton.SetActive(false);
                _closeResetButton.SetActive(false);
                _lockedBuyText.SetActive(false);
                _mainSkillImage.sprite = _lockSprite;
                _mainSkillDescription.text = "???";
                _mainSkillName.text = "???";
            }
            else
            {
                _buyButton.SetActive(false);
                _resetButton.SetActive(false);
                _closeResetButton.SetActive(false);
                _lockedBuyText.SetActive(true);
                _mainSkillImage.sprite = _lockSprite;
                _mainSkillDescription.text = "???";
                _mainSkillName.text = "???";
            }
        }

        public void BuySkill()
        {
            if (PlayerData.EvolvePoint < tree[chosenSkill].costGrade) return;
            PlayerData.EvolvePoint -= tree[chosenSkill].costGrade;
            UIManager.Instance.UpdateEvolvePointValue();
            grades.isOpen[chosenSkill] = true;
            PlayerPrefs.SetString("EvolveGrade", JsonUtility.ToJson(grades));
            OpenSkill(tree[chosenSkill]);
            _buyButton.SetActive(false);
            UpdateViewTree();
            tree[chosenSkill].openAction.Invoke();
            if (IAPManager.IsResetProgressBuying)
                _resetButton.SetActive(true);
            else
                _closeResetButton.SetActive(true);
            AnalyticManager.Snake_Modification(tree[chosenSkill].skillName?.Replace(" ", ""));
            if (chosenSkill > 0 && AdsManager.isEvolveInterstitial)
                AdsManager.Instance.ShowInterstitial();
        }

        public static void BuyGreatEyes()
        {
            if (UpgradesManager.GreatEyes < 1)
                UpgradesManager.GreatEyes = 1;
            MainShopManager.Instance.BuyGreatEyes();
            QuestManager.Instance.AddValueToQuest(QuestManager.Quest.QuestType.UnlockGreatEyes, 1);
        }

        public static void BuyFoodFinding()
        {
            if (UpgradesManager.FoodFinding < 1)
                UpgradesManager.FoodFinding = 1;
            FieldManager.Instance.UpgradeFoodFinding();
            QuestManager.Instance.AddValueToQuest(QuestManager.Quest.QuestType.UnlockFoodFinding, 1);
        }


        public static void BuySteelStomach()
        {
            if (UpgradesManager.SteelStomach < 1)
                UpgradesManager.SteelStomach = 1;
            FieldManager.Instance.UpgradeSteelStomach();
        }


        public void BuyAdrenalineGlands()
        {
            if (BoostManager.AdrenalineGlands < 1)
                BoostManager.AdrenalineGlands = 1;
            FieldManager.Instance.UpgradeBoostSpeed();
        }


        public void BuyFastMetabolism()
        {
            if (BoostManager.FastMetabolism < 1)
                BoostManager.FastMetabolism = 1;
            FieldManager.Instance.UpgradeBoostMetabolism();
        }
        public void BuyFullControl()
        {
            if (BoostManager.FullControl < 1)
                BoostManager.FullControl = 1;
        }


        public static void BuyPathfinding()
        {
            UpgradesManager.Pathfinding += 1;
            AnalyticManager.Field_Expand();
            UIManager.Instance.changeShopEventFirst += FieldManager.Instance.ShowPopup;
            QuestManager.Instance.AddValueToQuest(QuestManager.Quest.QuestType.UnlockPathfinding, 1);
        }


        public static void BuyStrongMuscles()
        {
            if (UpgradesManager.StrongMuscles < 1)
                UpgradesManager.StrongMuscles += 1;
            FieldManager.Instance.UpgradeStrongMuscles(UpgradesManager.StrongMuscles);
        }

        public void NeedToUpgrade(int grade)
        {
            _needToUpgradePanel.SetActive(true);
            _needUpgrade.text = "You need to unlock <color=red>" + tree[grade].skillName + "</color> first";
        }
        public void ResetSkill()
        {
            if (PlayerData.Diamond < _costResetSkill) return;
            PlayerData.Diamond -= _costResetSkill;
            grades.isOpen[chosenSkill] = false;
            PlayerPrefs.SetString("EvolveGrade", JsonUtility.ToJson(grades));
            UpdateViewTree();
            _buyButton.SetActive(true);
            _resetButton.SetActive(false);
            ResetSkill(tree[chosenSkill]);
            ChoseSkill(chosenSkill);
            PlayerData.EvolvePoint += tree[chosenSkill].costGrade;

            UIManager.Instance.UpdateEvolvePointValue();
            if (tree[chosenSkill].skillName.StartsWith("Pathfinding"))
            {
                FieldManager.Instance.ExpandField(FieldManager.Instance.FieldSize - 1);
            }
        }

        public void ResetAll()
        {
            for (int i = 0; i < grades.isOpen.Length; i++)
            {
                grades.isOpen[i] = false;
                ResetSkill(tree[i]);
            }
            PlayerPrefs.SetString("EvolveGrade", JsonUtility.ToJson(grades));
            UpgradesManager.Pathfinding = 0;
            FieldManager.Instance.ExpandField(6);
            FieldManager.Instance.InitializeField();
            UpdateViewTree();
            StartCoroutine(FieldManager.Instance.InitializeSnakeOnField());
        }

        public void UpdateEvolveCost()
        {
            tree[0].costGrade = (int)RemoteConfig.GetLong("Upgrade_Great_Eyes");
            tree[1].costGrade = (int)RemoteConfig.GetLong("Upgrade_Food_Finding");
            tree[2].costGrade = (int)RemoteConfig.GetLong("Upgrade_Pathfinding_01");
            tree[10].costGrade = tree[3].costGrade = (int)RemoteConfig.GetLong("Upgrade_Adrenaline_Glands");
            tree[8].costGrade = tree[4].costGrade = (int)RemoteConfig.GetLong("Upgrade_Fast_Metabolism");
            tree[13].costGrade = tree[5].costGrade = (int)RemoteConfig.GetLong("Upgrade_Strong_Muscles");
            tree[6].costGrade = (int)RemoteConfig.GetLong("Upgrade_Pathfinding_02");
            tree[11].costGrade = tree[7].costGrade = (int)RemoteConfig.GetLong("Upgrade_Steel_Stomach");
            tree[9].costGrade = (int)RemoteConfig.GetLong("Upgrade_Pathfinding_03");
            tree[12].costGrade = (int)RemoteConfig.GetLong("Upgrade_Pathfinding_04");
            tree[14].costGrade = (int)RemoteConfig.GetLong("Upgrade_Pathfinding_05");
            tree[15].costGrade = (int)RemoteConfig.GetLong("Upgrade_Maturity_upgrade");
            tree[16].costGrade = (int)RemoteConfig.GetLong("Upgrade_Full_control");
            FieldManager.Instance._maxFoodParameter = (int)RemoteConfig.GetLong("Food_New_Percent");
        }
    }

    public class EvolveGrades
    {
        public bool[] isOpen;

        public EvolveGrades(int lenght)
        {
            isOpen = new bool[lenght];
        }
    }
}
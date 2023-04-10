using System;
using System.Linq;
using Managers.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;
using Zenject;

namespace Managers
{
    public class AncestorsManager : Singleton<AncestorsManager>
    {
        public static AncestorsGrades grades;


        public Node[] tree;

        [SerializeField]
        private GameObject _lockResetButton;

        [SerializeField]
        private TextMeshProUGUI _costText;


        [SerializeField]
        private GameObject _strokeOfMainSkill;

        [SerializeField]
        private Sprite _lockSprite;


        [SerializeField]
        private Image _mainSkillImage;

        [SerializeField]
        private TextMeshProUGUI _mainSkillDescription;
        [SerializeField]
        private TextMeshProUGUI[] _sizesSnake;

        [SerializeField]
        private TextMeshProUGUI _mainSkillName;


        [SerializeField]
        private GameObject _buyButton;

        [SerializeField]
        private GameObject _lockedBuyText;

        [Inject] 
        private ISnakeLevelProvider _snakeLevelProvider;

        private static int chosenSkill;

        // Start is called before the first frame update
        void Awake()
        {

            if (PlayerPrefs.HasKey("AncestorsGrades"))
            {
                grades = JsonUtility.FromJson<AncestorsGrades>(PlayerPrefs.GetString("AncestorsGrades"));
            }
            grades ??= new AncestorsGrades(tree.Length);
            if (!PlayerPrefs.HasKey("version_0_3"))
            {
                grades = new AncestorsGrades(tree.Length);
            }
            UpdateViewTree();
        }

        public void ChoseSkill(int i)
        {
            chosenSkill = i;
            _strokeOfMainSkill.transform.position = tree[i].skillTransform.position;
            _strokeOfMainSkill.GetComponent<RectTransform>().sizeDelta = new Vector2(tree[i].skillTransform.GetComponent<RectTransform>().sizeDelta.x + 30, tree[i].skillTransform.GetComponent<RectTransform>().sizeDelta.x + 30);
            _mainSkillImage.sprite = tree[i].skillSprite;
            _mainSkillDescription.text = tree[i].skillDescription;
            _mainSkillName.text = tree[i].skillName;

            if (grades.isOpen[i])
            {
                _buyButton.SetActive(false);
                _lockedBuyText.SetActive(false);
            }
            else if (i == 0 || tree[i].prev.Any(prev => grades.isOpen[prev]))
            {
                _buyButton.SetActive(true);
                _lockedBuyText.SetActive(false);
                _costText.text = tree[i].costGrade.ToString();
            }
            else
            {
                _buyButton.SetActive(false);
                _lockedBuyText.SetActive(true);
                _mainSkillImage.sprite = _lockSprite;
                _mainSkillDescription.text = "???";
                _mainSkillName.text = "???";
            }
        }


        public void ResetProgress()
        {
            PlayerData.Ancestor += _snakeLevelProvider.CurrentLevelRx.Value;
            _snakeLevelProvider.ResetAll();
            UpgradesManager.Reset();
            EvolveShopManager.Instance.ResetAll();
            PlayerData.EvolvePoint = 0;
            UIManager.Instance.UpdateEvolvePointValue();
            BoostManager.ResetProgress();
            ChangeResetButton(true);
        }
        public void BuySkill()
        {
            if (PlayerData.Ancestor < tree[chosenSkill].costGrade) return;
            PlayerData.Ancestor -= tree[chosenSkill].costGrade;
            grades.isOpen[chosenSkill] = true;
            PlayerPrefs.SetString("AncestorsGrades", JsonUtility.ToJson(grades));
            tree[chosenSkill].openImageSkill.SetActive(true);
            _buyButton.SetActive(false);
            UpdateViewTree();
            tree[chosenSkill].openAction.Invoke();
            AnalyticManager.Snake_Ancestor_Modification(tree[chosenSkill].skillName?.Replace(" ", ""));
            if (chosenSkill > 0)
                AdsManager.Instance.ShowInterstitial();
        }
        public void UpdateViewTree()
        {
            Debug.Log(JsonUtility.ToJson(grades));
            foreach (var node in tree)
            {
                if (grades.isOpen[node.number])
                {
                    tree[node.number].openImageSkill.SetActive(true);
                    continue;
                }

                bool isPrevOpen = node.prev.Any(prev => grades.isOpen[prev]);
                Debug.Log(isPrevOpen);
                node.lockSkillImage.SetActive(!isPrevOpen);
          
            }

            tree[0].lockSkillImage.SetActive(false);
        }

        public void SetSizeSnake()
        {
            foreach (var text in _sizesSnake)
            {
                text.text = "" + _snakeLevelProvider.CurrentLevelRx.Value;
            }
        }

        public void ChangeResetButton(bool active)
        {
            _lockResetButton.SetActive(active);
        }



        public void UpdateCost()
        {
            tree[1].costGrade = (int)RemoteConfig.GetLong("Ancestor_Stronger_poison");
            tree[2].costGrade =  (int)RemoteConfig.GetLong("Ancestor_Digging");
            tree[3].costGrade = tree[6].costGrade = (int)RemoteConfig.GetLong("Ancestor_Stable_DNA");
            tree[4].costGrade = (int)RemoteConfig.GetLong("Ancestor_Gem_sense");
            tree[5].costGrade = (int)RemoteConfig.GetLong("Ancestor_Hunter_Instinct");
        }

        [Serializable]
        public class Node
        {
            public int number;
            public int[] prev;
            public int costGrade;
            public GameObject openImageSkill;
            public Transform skillTransform;
            public Sprite skillSprite;
            public string skillDescription;
            public string skillName;
            public GameObject lockSkillImage;
            public UnityEvent openAction;
        }


        public class AncestorsGrades
        {
            public bool[] isOpen;

            public AncestorsGrades(int lenght)
            {
                isOpen = new bool[lenght];
            
            }
        }
    }
}

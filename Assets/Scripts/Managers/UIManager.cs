using System.Collections;
using Managers.Interfaces;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;
using Zenject;

namespace Managers
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField]
        private GameObject _shop;
        [SerializeField]
        private DonateShopManager _donateShopManager;
        [SerializeField]
        private TextMeshProUGUI _coinsValue;
        [SerializeField]
        private TextMeshProUGUI _diamondValue;
        [SerializeField]
        private TextMeshProUGUI _ancestorValue;

        [SerializeField]
        private TextMeshProUGUI _sizeValue;

        [SerializeField]
        private TextMeshProUGUI _ePValue;
        [SerializeField]
        private TextMeshProUGUI _ePValueInSkillTreeScreen;

        [SerializeField]
        private TextMeshProUGUI _maxFood;
        [SerializeField]
        private GameObject _reviewWindow;
        [SerializeField]
        private GameObject _firstMenu;
        [SerializeField]
        private GameObject _secondMenu;
        [SerializeField]
        private GameObject _thirdMenu;
        [SerializeField]
        private TextMeshProUGUI _version;

        [SerializeField]
        private Transform _mainStroke;
        [SerializeField]
        private Transform[] _shops;

        [SerializeField]
        private Sprite[] _shopSprites;
        [SerializeField]
        private Image _spriteShop;
        [SerializeField]
        private GameObject[] _nameShops;

        public UnityAction changeShopEventFirst;
        public UnityAction changeShopEventSecond;

        private ICurrencyManager _currencyManager;
        private ISnakeLevelProvider _snakeLevelProvider;
        
        [Inject]
        private void Construct(
            ICurrencyManager currencyManager,
            ISnakeLevelProvider snakeLevelProvider)
        {
            _currencyManager = currencyManager;
            _snakeLevelProvider = snakeLevelProvider;
        }
     
        IEnumerator Start()
        {
            if (UpgradesManager.OpenedAncestors)
            {
                UnLockAncestor();
                yield return null;
                ChangeMenuAnim(0);
            }
            _version.text = Application.version;
            UpdateCoinValue();
            UpdateEvolvePointValue();
            UpdateDiamondValue();
            UpdateAncestorValue();
            UpdateSizeValue();
        }

        public void UpdateCoinValue()
        {
            _coinsValue.text = UpgradesManager.AllCoins.ToString();
        }
        
        public void UpdateFoodValue(string text)
        {
            _maxFood.text = text;
        }

        public void UpdateEvolvePointValue()
        {
            _ePValue.text = PlayerData.EvolvePoint.ToString();
            _ePValueInSkillTreeScreen.text = PlayerData.EvolvePoint.ToString();
        }
        
        public void UpdateDiamondValue()
        {
            _diamondValue.text = PlayerData.Diamond.ToString();
        }

        public void UpdateAncestorValue()
        {
            _ancestorValue.text = PlayerData.Ancestor.ToString();
        }

        public void UpdateSizeValue()
        {
            _sizeValue.text = _snakeLevelProvider.CurrentLevelRx.Value.ToString();
        }

        public void ChangeMenuAnim(int menu)
        {
            _mainStroke.transform.position = _shops[menu].position;
            _spriteShop.sprite = _shopSprites[menu];
            foreach (var nameProduct in _nameShops)
            {
                nameProduct.SetActive(false);
            }
            _nameShops[menu].SetActive(true);
            switch (menu)
            {
                case 0:
                    _firstMenu.SetActive(true);
                    _secondMenu.SetActive(false);
                    _shop.SetActive(false);
                    _thirdMenu.SetActive(false);

                    changeShopEventFirst?.Invoke();
                    changeShopEventFirst = null;
                    break;
                case 1:
                    _firstMenu.SetActive(false);
                    _secondMenu.SetActive(true);
                    _shop.SetActive(false);

                    _thirdMenu.SetActive(false);

                    changeShopEventSecond?.Invoke();
                    changeShopEventSecond = null;
                    break;
                case 2:
                    _donateShopManager.OpenShop(3);
                    break;
                case 3:
                    _firstMenu.SetActive(false);
                    _secondMenu.SetActive(false);
                    _shop.SetActive(false);
                    _thirdMenu.SetActive(true);
                  
                    break;
            }

        }

        public void DebugAddEvolvePoint()
        {
            if (!DebugController.isDebug) return;
            PlayerData.EvolvePoint += 100;
            UpdateEvolvePointValue();
        } 
        public void DebugAddAncestorPoint()
        {
            if (!DebugController.isDebug) return;
            PlayerData.Ancestor += 100;
        }
        public void DebugAddCoin()
        {
            if (!DebugController.isDebug) 
            { 
                ChangeMenuAnim(3);
                return; 
            }
            
            _currencyManager.ChangeFood(100);
            _currencyManager.ChangeHard(100);
        }

        public void ResetAll()
        {
            PlayerPrefs.DeleteAll();
            Application.Quit();
        }

        public void OpenAncestor()
        {
            AncestorsManager.grades.isOpen[0] = true;
            UnLockAncestor();
            QuestManager.Instance.AddValueToQuest(QuestManager.Quest.QuestType.UnlockAncestor, 1);
            PlayerPrefs.SetString("AncestorsGrades", JsonUtility.ToJson(AncestorsManager.grades));
        }

        private void UnLockAncestor()
        {
            if (_shops[3].gameObject.activeSelf) return;
            TutorialManager.Instance.ShowAncestorHand();
            EvolveShopManager.Instance.OpenAncestor();
            _shops[3].gameObject.SetActive(true);
            _mainStroke.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _mainStroke.GetComponent<RectTransform>().rect.width * 0.75f);
            AncestorsManager.Instance.UpdateViewTree();
            AncestorsManager.Instance.ChoseSkill(0);
            UpgradesManager.OpenedAncestors = true;
        }


        public static void OpenReview()
        {
            Application.OpenURL(URLConstants.Store);
        }

        public void OpenReviewWindow()
        {
            _reviewWindow.SetActive(true);
        }
    }
}

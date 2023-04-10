using System;
using Managers.Interfaces;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Managers
{
    public class LevelGrowManager : MonoBehaviour, ISnakeLevelProvider  //Utilities.Singleton<LevelGrowManager>
    {
        [SerializeField]
        private RectTransform _progressBar;

        [SerializeField]
        private TextEatFruit _pointPrefab;

        public static TextEatFruit pointPrefab;

        [SerializeField]
        private TextMeshProUGUI _progressText;

        private static TextMeshProUGUI progressText;

        [SerializeField]
        private Image _growButton;

        [SerializeField]
        private GameObject _newSkillPointPanel;

        [SerializeField]
        private GameObject _maxSizePanel;

        [SerializeField]
        private GameObject _shadowGrowButton;

        private static GameObject shadowGrowButton;

        [SerializeField]
        private TextMeshProUGUI _newLevelText;

        [Inject] 
        private ICurrencyManager _currencyManager;

        private static Image growButton;

        private static RectTransform progressBar;

        //private static int currentLevel;
        private static int currentLevelState;
        private static int nextGrow;
        private static int _baseMultiplyGrow = 2;
        private static float _multiplyGrow = 1.5f;

        public static int baseGrowForFood = 1;
        public static int upGrowForFood = 1;
        private static float _startWight = 360;
        private static float percentMaxGrow=0.25f;

        public bool IsGrowUpReady => FoodLeft.Value <= 0;

        public ReactiveProperty<int> FoodLeft { get; } = new();

        public ReactiveProperty<int> CurrentLevelRx { get; } = new();

        public int CurrentLevel
        {
            get => PlayerPrefs.GetInt("CurrentLevel", 2);
            private set
            {
                PlayerPrefs.SetInt("CurrentLevel", value);
                CurrentLevelRx.Value = value;
                //currentLevel = value;
            }
        }

        private static int CurrentLevelStateSave
        {
            get => PlayerPrefs.GetInt("CurrentLevelState", 0);
            set
            {
                PlayerPrefs.SetInt("CurrentLevelState", value);
                currentLevelState = value;
            }
        }

        private void Start()
        {
            pointPrefab = _pointPrefab;
            shadowGrowButton = _shadowGrowButton;
            growButton = _growButton;
            CurrentLevelRx.Value = CurrentLevel;
            currentLevelState = CurrentLevelStateSave;
            progressBar = _progressBar;
            progressText = _progressText;
            nextGrow = 10 * Mathf.CeilToInt(Mathf.Pow(CurrentLevelRx.Value * _baseMultiplyGrow, _multiplyGrow) / 10f);
            CheckGrowSnake();
            UpdateProgressBar();
            AncestorsManager.Instance.SetSizeSnake();

            //костыль
            IncreaseLevelState(0);
        }

        public void EatApple(int fruit, int id)
        {
            var add = (nextGrow * 1.5f) <= currentLevelState
                ? 0
                : (baseGrowForFood * (1 + id + (AncestorsManager.grades.isOpen[3] ? 1 : 0)+ (AncestorsManager.grades.isOpen[6] ? 1 : 0)));
            
            //CurrentLevelState = currentLevelState + add;
            IncreaseLevelState(add);
            
            var text = Instantiate(pointPrefab, Snake.Instance.Head.position, new Quaternion());
            text._text.text = fruit.ToString();
            
            CheckGrowSnake();
            UpdateProgressBar();

            _currencyManager.ChangeFood(fruit);
            //UpgradesManager.AllCoins += fruit;
            //UIManager.Instance.UpdateCoinValue();
            
            Debug.Log($"Current Level {CurrentLevelRx.Value}: {currentLevelState}/{nextGrow}");
        }

        private void IncreaseLevelState(int value)
        {
            currentLevelState += value;

            CurrentLevelStateSave = currentLevelState;

            FoodLeft.Value = nextGrow - currentLevelState;
        }

        [Obsolete]
        private static void CheckGrowSnake()
        {
            if (currentLevelState < nextGrow)
            {
                return;
            }
            
            growButton.raycastTarget = true;
            growButton.color = Color.white;
            growButton.rectTransform.anchoredPosition = Vector3.zero;
            shadowGrowButton.SetActive(true);
        }

        public void NewGrowLevel()
        {
            _newLevelText.text = $"Now your Snake is <color=red>{CurrentLevelRx.Value + 1}</color> meters long!";
            if (FieldManager.Instance.FieldSize * FieldManager.Instance.FieldSize * percentMaxGrow >=
                Snake.Instance.Segments.Count-1)
            {
                _newSkillPointPanel.SetActive(true);
            }
            else
            {
                _maxSizePanel.SetActive(true);
            }
            if (CurrentLevelRx.Value > 2 && AdsManager.isGrowInterstitial)
                AdsManager.Instance.ShowInterstitial();
        }

        public void CloseNewSkillPointPanel()
        {
            _newSkillPointPanel.SetActive(false);
            PlayerData.EvolvePoint += 1;
            //CurrentLevelState = currentLevelState - nextGrow;

            var level = CurrentLevelRx.Value;
            
            if (level == 2)
            {
                TutorialManager.Instance.ShowEvolveHand();
            }
            if(level == 5)
            {
                UIManager.Instance.OpenReviewWindow();
            }
            CurrentLevel = level + 1;
            
            AncestorsManager.Instance.SetSizeSnake();
            Snake.Instance.AddSegment();
            AnalyticManager.Snake_Growth();
            AnalyticManager.Snake_Growth_Size(level + 1);
            
            if (level == 16)
            {
                QuestManager.Instance.AddValueToQuest(QuestManager.Quest.QuestType.Reach16Size, 1);
            }
            
            /*growButton.raycastTarget = false;
            growButton.color = Color.black;
            shadowGrowButton.SetActive(false);
            growButton.rectTransform.anchoredPosition = Vector3.down * 5f;*/
            
            nextGrow = 10 * Mathf.CeilToInt(Mathf.Pow(level * _baseMultiplyGrow, _multiplyGrow) / 10f);
            
            Debug.Log("Next grow " + nextGrow);
            
            //UpdateProgressBar();
            //CheckGrowSnake();
            IncreaseLevelState(-nextGrow);
            UIManager.Instance.UpdateEvolvePointValue();
            UIManager.Instance.UpdateSizeValue();
        }

        private static void UpdateProgressBar()
        {
            progressBar.offsetMax =
                new Vector2(
                    nextGrow <= currentLevelState
                        ? 0
                        : -_startWight + _startWight *  currentLevelState / nextGrow, progressBar.offsetMax.y);
            progressText.text = (nextGrow * 1.5f) <= currentLevelState ? "MAX" : $"{currentLevelState} / {nextGrow}";
        }

        public static void LoadValue()
        {
            _baseMultiplyGrow = (int) RemoteConfig.GetDouble("Snake_GrowthBase");
            _multiplyGrow = (float) RemoteConfig.GetDouble("Snake_GrowthMult");
            baseGrowForFood = (int) RemoteConfig.GetDouble("Food_Start");
            upGrowForFood = ((int) RemoteConfig.GetDouble("Food_Percent")) / 100;
            FieldManager.speedSnakeForTime = (float) RemoteConfig.GetDouble("Snake_Speed");
            FieldManager.percentMaxFood = (float) RemoteConfig.GetDouble("Field_MaxFoodPercent")/100;
            FieldManager.percentMaxGood = (float) RemoteConfig.GetDouble("Field_MaxThingPercent") /100;
            percentMaxGrow = (float) RemoteConfig.GetDouble("Field_MaxSnakePercent")/100;
        }

        public void ResetAll()
        {
            CurrentLevelStateSave = 0;
            CurrentLevel = 2;
            growButton.raycastTarget = false;
            growButton.color = Color.black;
            shadowGrowButton.SetActive(false);
            growButton.rectTransform.anchoredPosition = Vector3.down * 5f;
            nextGrow = 10 * Mathf.CeilToInt(Mathf.Pow(CurrentLevelRx.Value * _baseMultiplyGrow, _multiplyGrow) / 10f);
            UpdateProgressBar();
            CheckGrowSnake();
            UIManager.Instance.UpdateSizeValue();

        }

        public static void DebugGrowSnake()
        {
            if (!DebugController.isDebug) 
            { 
                UIManager.Instance.ChangeMenuAnim(3); 
                return; 
            }
            
            CurrentLevelStateSave = currentLevelState + 100;
            UpdateProgressBar();
            CheckGrowSnake();
        }
    }
}
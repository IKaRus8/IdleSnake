using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using Managers.Interfaces;
using UniRx;

namespace Managers
{

    public class QuestManager : Utilities.Singleton<QuestManager>, IQuestManager
    {
        [Serializable]
        public class Quest
        {
            public enum QuestType
            {
                UnlockGreatEyes,
                UnlockFoodFinding,
                UnlockPathfinding,
                Reach16Size,
                UnlockAncestor
            }

            [Header("Quest info")]
            public QuestType Type;

            public string QuestDesc;

            private int _questCurrentValue = -1;

            public int QuestCurrentValue
            {

                get
                {
                    if (_questCurrentValue == -1) _questCurrentValue = PlayerPrefs.GetInt(Type + "Value", 0);
                    return _questCurrentValue;
                }

                set
                {
                    _questCurrentValue = value;
                    PlayerPrefs.SetInt(Type + "Value", value);
                }
            }

            public int QuestGoalValue;

            public List<QuestType> PrevType = new();

            [Header("Quest object")]
            public GameObject QuestGameObject;

            public TextMeshProUGUI QuestDescText;

            public TextMeshProUGUI QuestProgressText;

            public Image[] Light;

            private bool _isStarted;
            private bool _isCompleted;
            public void StartQuest()
            {
                if (_isStarted) return;
                foreach (var light in Light)
                {
                    light.enabled = true;
                }
                _isStarted = true;
            } 
            public void CompleteQuest()
            {
                if (_isCompleted) return;
                foreach (var light in Light)
                {
                    light.enabled =  false;
                }
                _isCompleted = true;
            }
            public bool IsComplete()
            {
                return QuestCurrentValue == QuestGoalValue;
            }


            public bool IsAvailable()
            {
                return PrevType.All(type => Instance.IsQuestTypeComplete(type));
            }
        }

        private static bool IsNotAvailableShow
        {
            get => PlayerPrefs.GetInt("IsNotAvailableQuestShowPopup", 0) == 1;
            set => PlayerPrefs.SetInt("IsNotAvailableQuestShowPopup", value ? 1 : 0);
        }

        [SerializeField]
        private List<Quest> _questList = new();

        [SerializeField]
        private GameObject _noAvailableQuestInList;

        [SerializeField]
        private GameObject _noAvailableQuestPopup;

        [Header("Current quest main"), SerializeField]
        private GameObject _currentQuestObject;

        [SerializeField]
        private TextMeshProUGUI _currentQuestDesc;

        [SerializeField]
        private TextMeshProUGUI _currentQuestProgress;

        private readonly Dictionary<Quest.QuestType, Quest> _questDict = new ();

        public ReactiveProperty<string> QuestProgress { get; } = new();

        private void Awake()
        {
            foreach (var t in _questList)
                _questDict.Add(t.Type, t);

            UpdateCurrentQuestUI();
        }

        private bool IsQuestTypeAvailable(Quest.QuestType questType)
        {
            Quest currentQuest = _questDict[questType];
            return currentQuest.PrevType.All(IsQuestTypeComplete);
        }


        private bool IsQuestTypeComplete(Quest.QuestType questType)
        {
            return _questDict[questType].QuestCurrentValue == _questDict[questType].QuestGoalValue;
        }

     

        public void UpdateQuestListUI()
        {
            _noAvailableQuestInList.SetActive(true);
            foreach (Quest quest in _questDict.Values)
            {
                bool isEnable = quest.IsAvailable();
                quest.QuestGameObject.SetActive(isEnable);
                if (!isEnable) continue;
                _noAvailableQuestInList.SetActive(false);
                quest.QuestDescText.text = quest.QuestDesc;
                quest.QuestProgressText.text = $"{quest.QuestCurrentValue}/{quest.QuestGoalValue}";
                if (quest.IsComplete()) { 
                    quest.QuestDescText.fontStyle = FontStyles.Strikethrough;
                }
                IsNotAvailableShow = false;

            }
        }

        private void UpdateCurrentQuestUI()
        {
            _currentQuestObject.SetActive(false);
            foreach (Quest quest in _questDict.Values)
            {
                bool isEnable = quest.IsAvailable() && !quest.IsComplete();
                if (isEnable)
                {
                    quest.StartQuest();
                    _currentQuestObject.SetActive(true);
                    _currentQuestDesc.text = quest.QuestDesc;
                    _currentQuestProgress.text = $"{quest.QuestCurrentValue}/{quest.QuestGoalValue}";
                    
                    QuestProgress.Value = $"{quest.QuestDesc} {quest.QuestCurrentValue}/{quest.QuestGoalValue}";
                    
                    IsNotAvailableShow = false;
                    return;
                }
                if (quest.IsComplete())
                {
                    quest.CompleteQuest();
                }

            }

            if (!IsNotAvailableShow)
            {
                IsNotAvailableShow = true;
                _noAvailableQuestPopup.SetActive(true);
            }
        }

        public void AddValueToQuest(Quest.QuestType questType, int value, bool isForce = false)
        {
            if (isForce || (IsQuestTypeAvailable(questType) && !IsQuestTypeComplete(questType))) _questDict[questType].QuestCurrentValue += value;
            UpdateCurrentQuestUI();
            /*   if (IsComplete(_questDict[questType]){
                   bool flagAll = true;
                   foreach(Quest quest in _questDict.Values)
                   {
                       flagAll = flagAll && IsComplete(quest);
                   }
                   if (flagAll)
                   {
                       _currentQuestObject.SetActive(false);

                   }
               }*/


        }
    }
}
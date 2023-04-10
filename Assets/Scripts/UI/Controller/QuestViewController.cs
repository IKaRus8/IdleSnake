using System;
using Extensions.Core;
using Managers.Interfaces;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Controller
{
    public class QuestViewController : MonoBehaviour
    {
        [SerializeField] 
        private Button _button;
        [SerializeField] 
        private GameObject _panel;
        [SerializeField] 
        private TextMeshProUGUI _questText;

        [Inject] 
        private IQuestManager _questManager;

        private bool _isActive;

        private void Awake()
        {
            _button.CheckForNull();
            _panel.CheckForNull();
            _questText.CheckForNull();
            
            _button.onClick.AddListener(() => SetEnabled(!_isActive));

            _questManager.QuestProgress.Subscribe(UpdateText).AddTo(this);
        }

        private void SetEnabled(bool value)
        {
            _isActive = value;
            
            _panel.SetActive(value);
        }

        private void UpdateText(string text)
        {
            _questText.text = text;
        }
    }
}
using System;
using Extensions.Core;
using Managers.Interfaces;
using Services;
using Services.Interfaces;
using UI.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Popups
{
    public class OptionPopup : MonoBehaviour, IPopup
    {
        [SerializeField] 
        private Button _closeButton;
        [SerializeField] 
        private Toggle _soundToggle;
        [SerializeField] 
        private Toggle _musicToggle;

        private IPopupService _popupService;
        private ISoundManager _soundManager;
        
        public GameObject GO => gameObject;

        [Inject]
        private void Construct(
            IPopupService popupService,
            ISoundManager soundManager)
        {
            _popupService = popupService;
            _soundManager = soundManager;
        }

        private void Awake()
        {
            _closeButton.CheckForNull();
            _soundToggle.CheckForNull();
            _musicToggle.CheckForNull();
            
            _closeButton.onClick.AddListener(Close);

            _soundToggle.isOn = _soundManager.IsSoundOn;
            _musicToggle.isOn = _soundManager.IsMusicOn;
            
            _soundToggle.onValueChanged.AddListener(OnSoundChange);
            _musicToggle.onValueChanged.AddListener(OnMusicChange);
        }

        private void OnSoundChange(bool value)
        {
            _soundManager.ChangeSoundOff(!value);
        }

        private void OnMusicChange(bool value)
        {
            _soundManager.ChangeMusic(value);
        }

        private void Close()
        {
            _popupService.ClosePopup(this);
        }
    }
}
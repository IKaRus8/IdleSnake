using System;
using Extensions.Core;
using UI.Interfaces;
using UnityEngine;

namespace UI
{
    public class UIContainer : MonoBehaviour, IUIContainer
    {
        [SerializeField] 
        private Transform _popupContainer;

        [SerializeField] 
        private GameObject _screenBack;

        public Transform PopupContainer => _popupContainer;

        public GameObject ScreenBack => _screenBack;

        private void Awake()
        {
            _popupContainer.CheckForNull();
            _screenBack.CheckForNull();
        }
    }
}
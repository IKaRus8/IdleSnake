using System;
using System.Collections.Generic;
using Extensions.Core;
using UI.Interfaces;
using UI.Shop;
using UnityEngine;

namespace UI.Managers
{
    public class ScreenManager : MonoBehaviour, IScreenManager
    {
        [SerializeField] 
        private List<BaseScreen> _sreens = new();

        private void Awake()
        {
            _sreens.CheckSelfAndItemsForNullOrEmpty();
        }

        public void Show<T>() where T : BaseScreen
        {
            var opened = false;
            
            foreach (var screen in _sreens)
            {
                if (screen is T)
                {
                    screen.Show();
                    
                    opened = true;
                    continue;
                }
                
                screen.Hide();
            }

            if (!opened)
            {
                Debug.LogWarning($"Not found screen {typeof(T)}");
            }
        }

        public void HideAll()
        {
            foreach (var screen in _sreens)
            {
                screen.Hide();
            }
        }

        public void HideAllExcept<T>() where T : BaseScreen
        {
            foreach (var screen in _sreens)
            {
                if (screen is T)
                {
                    continue;
                }
                
                screen.Hide();
            }
        }
    }
}
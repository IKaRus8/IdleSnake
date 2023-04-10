using System;
using Extensions.Core;
using Managers.Interfaces;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Managers
{
    public class ResourceViewManager : MonoBehaviour
    {
        [SerializeField] 
        private ResourceView _foodResource;
        [SerializeField] 
        private ResourceView _hardResource;

        [Inject] 
        private ICurrencyManager _currencyManager;
        
        private void Awake()
        {
            _foodResource.CheckForNull();
            _hardResource.CheckForNull();

            _currencyManager.FoodPointsRx.Subscribe(OnFoodUpgrade).AddTo(this);
            _currencyManager.HardQuantityRx.Subscribe(OnHardUpgrade).AddTo(this);
        }

        private void OnFoodUpgrade(int value)
        {
            _foodResource.SetValue(value);
        }

        private void OnHardUpgrade(int value)
        {
            _hardResource.SetValue(value);
        }
    }
}
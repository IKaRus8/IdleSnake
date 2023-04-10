using System;
using System.Collections.Generic;
using System.Linq;
using Extensions.Core;
using Managers.Interfaces;
using TMPro;
using UI.Interfaces;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Shop
{
    public class ShopScreenController : BaseScreen, IFullScreenView
    {
        [SerializeField] 
        private List<ShopProduct> _foodItems;
        private List<ShopProduct> _hardItems;

        private IBundleProvider _bundleProvider;

        [Inject]
        private void Construct(IBundleProvider bundleProvider)
        {
            _bundleProvider = bundleProvider;
        }

        private void Awake()
        {
            _foodItems.CheckSelfAndItemsForNullOrEmpty();
            //_hardItems.CheckSelfAndItemsForNullOrEmpty();

            _bundleProvider.IsInitialized.Subscribe(OnInitialized).AddTo(this);
        }

        private void OnInitialized(bool value)
        {
            if (!value)
            {
                return;
            }

            SetFoodBundles();
        }

        private void SetFoodBundles()
        {
            foreach (var item in _foodItems)
            {
                var id = item.ItemId;

                var model = _bundleProvider.ShopItems.FirstOrDefault(i => i.ItemId.Contains(id));

                if (model == null)
                {
                    Debug.LogError($"Not found shop item with id {id}");
                }
                
                item.Initialize(model);
            }
        }

        private void SetHardBundles()
        {
            foreach (var item in _hardItems)
            {
                var id = item.ItemId;

                var model = _bundleProvider.ShopItems.FirstOrDefault(i => i.ItemId.Contains(id));

                if (model == null)
                {
                    Debug.LogError($"Not found shop item with id {id}");
                }
                
                item.Initialize(model);
            }
        }
    }
}
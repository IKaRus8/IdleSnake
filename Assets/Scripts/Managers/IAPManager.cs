using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions.Core;
using JetBrains.Annotations;
using Managers.Interfaces;
using Models;
using UI.Shop;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using Utilities;
using Zenject;

namespace Managers
{
    public class IAPManager : Singleton<IAPManager>, IStoreListener, IBundleProvider
    {
        private static string _smallMegaBundle = "small_mega_bundle";
        private static string _mediumMegaBundle = "medium_mega_bundle";
        private static string _bigMegaBundle = "big_mega_bundle";
        
        private static string _foodSmallMegaBundle = "food_small_mega_bundle";
        private static string _foodMediumMegaBundle = "food_medium_mega_bundle";
        private static string _foodLargeMegaBundle = "food_large_mega_bundle";
        
        private static string _gemSmallMegaBundle = "gem_small_mega_bundle";
        private static string _gemMediumMegaBundle = "gem_medium_mega_bundle";
        private static string _gemLargeMegaBundle = "gem_large_mega_bundle";
        
        private static string _snakeUpgradeNoAds = "snake_upgrade_no_ads";
        private const string SnakeUpgradeNoAds0 = "snake_upgrade_no_ads";
        private static string _snakeUpgradeMaxFood = "snake_upgrade_max_food";
        private const string SnakeUpgradeMaxFood0 = "snake_upgrade_max_food";
        private static string _snakeUpgradeDoubleFood = "snake_upgrade_double_food";
        private const string SnakeUpgradeDoubleFood0 = "snake_upgrade_double_food";
        private static string _snakeUpgradeResetSkill = "snake_upgrade_reset_skill";
        private const string SnakeUpgradeResetSkill0 = "snake_upgrade_reset_skill";

        public static int Price = 0;
        private static Action purchased;

        private static IStoreController StoreController { get; set; }

        private static IExtensionProvider _storeExtensionProvider;

        [SerializeField]
        private GameObject _BuyRemoveAdsButton;
        [SerializeField]
        private GameObject _BuyMaxFoodButton;
        [SerializeField]
        private GameObject _BuyDoubleFoodButton;
        [SerializeField]
        private GameObject _BuyResetProgressButton;
        [SerializeField]
        private GameObject _BuyMaxFood;

        [Inject] 
        private ICurrencyManager _currencyManager;

        public ShopItemModel[] ShopItems { get; private set; }

        public static bool IsSubscribeEnable { get; private set; }
        public static bool IsDoubleFoodBuying { get; private set; }
        public static bool IsResetProgressBuying { get; private set; }

        public string[] FoodBundle => new[]
        {
            _foodSmallMegaBundle,
            _foodMediumMegaBundle,
            _foodLargeMegaBundle
        };

        public string[] GemBundle => new[]
        {
            _gemSmallMegaBundle,
            _gemMediumMegaBundle,
            _gemLargeMegaBundle
        };

        public ReactiveProperty<bool> IsInitialized { get; } = new();

        public void IAPInitialization()
        {
            Debug.Log("Begin init IAP");
            Debug.Log($"Price = {Price}");
            
            if (Price == 1)
            {
                _smallMegaBundle += "_1";
                _mediumMegaBundle += "_1";
                _bigMegaBundle += "_1";
                _foodSmallMegaBundle += "_1";
                _foodMediumMegaBundle += "_1";
                _foodLargeMegaBundle += "_1";
                _gemSmallMegaBundle += "_1";
                _gemMediumMegaBundle += "_1";
                _gemLargeMegaBundle += "_1";
                _snakeUpgradeNoAds += "_1";
                _snakeUpgradeMaxFood += "_1";
                _snakeUpgradeDoubleFood += "_1";
                _snakeUpgradeResetSkill += "_1";
            }
            
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            
            builder.AddProduct(_smallMegaBundle, ProductType.Consumable)
                .AddProduct(_mediumMegaBundle, ProductType.Consumable)
                .AddProduct(_bigMegaBundle, ProductType.Consumable)
                .AddProduct(_foodSmallMegaBundle, ProductType.Consumable)
                .AddProduct(_foodMediumMegaBundle, ProductType.Consumable)
                .AddProduct(_foodLargeMegaBundle, ProductType.Consumable)
                .AddProduct(_gemSmallMegaBundle, ProductType.Consumable)
                .AddProduct(_gemMediumMegaBundle, ProductType.Consumable)
                .AddProduct(_gemLargeMegaBundle, ProductType.Consumable)
                .AddProduct(_snakeUpgradeDoubleFood, ProductType.NonConsumable)
                .AddProduct(_snakeUpgradeMaxFood, ProductType.NonConsumable)
                .AddProduct(_snakeUpgradeNoAds, ProductType.NonConsumable)
                .AddProduct(_snakeUpgradeResetSkill, ProductType.NonConsumable);
            
            if (Price == 1) 
            {
                builder.AddProduct(SnakeUpgradeDoubleFood0, ProductType.NonConsumable)
                    .AddProduct(SnakeUpgradeMaxFood0, ProductType.NonConsumable)
                    .AddProduct(SnakeUpgradeNoAds0, ProductType.NonConsumable)
                    .AddProduct(SnakeUpgradeResetSkill0, ProductType.NonConsumable); 
            }
            
            Debug.Log("Builder added products");
            UnityPurchasing.Initialize(this, builder);
            Debug.Log("UnityPurchasing successful Initialize");


            //ShopItems.CheckSelfAndItemsForNullOrEmpty();
        }

        private IEnumerable<ShopItemModel> GetItemsModels1()
        {
            yield return new ShopItemModel
            {
                ItemId = _foodSmallMegaBundle,
                Price = GetPriceForId(_foodSmallMegaBundle),
                Description = GetDescriptionForId(_foodSmallMegaBundle),
                Callback = () => BuyFoodBundle(1000)
            };
            
            yield return new ShopItemModel()
            {
                ItemId = _foodMediumMegaBundle,
                Price = GetPriceForId(_foodMediumMegaBundle),
                Description = GetDescriptionForId(_foodMediumMegaBundle),
                Callback = () => BuyFoodBundle(10000)
            };
            
            yield return new ShopItemModel()
            {
                ItemId = _foodLargeMegaBundle,
                Price = GetPriceForId(_foodLargeMegaBundle),
                Description = GetDescriptionForId(_foodLargeMegaBundle),
                Callback = () => BuyFoodBundle(10000)
            };
            
            yield return new ShopItemModel()
            {
                ItemId = _gemSmallMegaBundle,
                Price = GetPriceForId(_gemSmallMegaBundle),
                Description = GetDescriptionForId(_gemSmallMegaBundle),
                Callback = () => BuyHardBundle(10)
            };
            
            yield return new ShopItemModel()
            {
                ItemId = _gemMediumMegaBundle,
                Price = GetPriceForId(_gemMediumMegaBundle),
                Description = GetDescriptionForId(_gemMediumMegaBundle),
                Callback = () => BuyHardBundle(100)
            };
            
            yield return new ShopItemModel()
            {
                ItemId = _gemLargeMegaBundle,
                Price = GetPriceForId(_gemLargeMegaBundle),
                Description = GetDescriptionForId(_gemLargeMegaBundle),
                Callback = () => BuyHardBundle(1000)
            };
        }

        private List<ShopItemModel> GetItemsModels()
        {
            var result = new List<ShopItemModel>
            {
                new()
                {
                    ItemId = _foodSmallMegaBundle,
                    Price = GetPriceForId(_foodSmallMegaBundle),
                    Description = GetDescriptionForId(_foodSmallMegaBundle),
                    Callback = () => BuyFoodBundle(1000)
                },
                new()
                {
                    ItemId = _foodMediumMegaBundle,
                    Price = GetPriceForId(_foodMediumMegaBundle),
                    Description = GetDescriptionForId(_foodMediumMegaBundle),
                    Callback = () => BuyFoodBundle(10000)
                },
                new()
                {
                    ItemId = _foodLargeMegaBundle,
                    Price = GetPriceForId(_foodLargeMegaBundle),
                    Description = GetDescriptionForId(_foodLargeMegaBundle),
                    Callback = () => BuyFoodBundle(10000)
                },
                new()
                {
                    ItemId = _gemSmallMegaBundle,
                    Price = GetPriceForId(_gemSmallMegaBundle),
                    Description = GetDescriptionForId(_gemSmallMegaBundle),
                    Callback = () => BuyHardBundle(10)
                },
                new()
                {
                    ItemId = _gemMediumMegaBundle,
                    Price = GetPriceForId(_gemMediumMegaBundle),
                    Description = GetDescriptionForId(_gemMediumMegaBundle),
                    Callback = () => BuyHardBundle(100)
                },
                new()
                {
                    ItemId = _gemLargeMegaBundle,
                    Price = GetPriceForId(_gemLargeMegaBundle),
                    Description = GetDescriptionForId(_gemLargeMegaBundle),
                    Callback = () => BuyHardBundle(1000)
                },
                
            };

            return result;
        }

        public static bool IsIAPInitialized()
        {
            return StoreController != null && _storeExtensionProvider != null;
        }
        
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogWarning($"Init failed {error.ToString()}");
        }

        //public static IEnumerator CheckSubscription()
        //{
        //    while (!IsIAPInitialized())
        //    {
        //        IAPManager.Instance.IAPInitializate();
        //        yield return new WaitForSeconds(0.5f);
        //    }
        //    if (_storeController != null || _storeController.products != null)
        //    {
        //        foreach (var product in _storeController.products.all)
        //        {
        //            if (product.hasReceipt)
        //            {
        //                IsSubscribeEnable = true;
        //                break;
        //            }
        //            IsSubscribeEnable = false;
        //        }
        //    }
        //}

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            purchased?.Invoke();
            purchased = null;
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            purchased = null;
        }

        /// <summary>
        /// ���������, ������ �� �����.
        /// </summary>
        /// <param name="id">������ ������ � ������.</param>
        /// <returns></returns>
        private static bool CheckBuyState(string id)
        {
            var product = StoreController.products.WithID(id);
            
            if (product.hasReceipt)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            StoreController = controller;
            _storeExtensionProvider = extensions;
            Debug.Log($"OnInitialized. StoreController not null = {StoreController != null} \n " +
                      $"ExtensionProvider not null = {_storeExtensionProvider != null}");
            
            if (!DebugController.isDebug)
            {
                if (CheckBuyState(_snakeUpgradeNoAds))
                    RemoveAds();
                if (CheckBuyState(_snakeUpgradeDoubleFood))
                    DoubleFood();
                if (CheckBuyState(_snakeUpgradeMaxFood))
                    MaxFood();
                if (CheckBuyState(_snakeUpgradeResetSkill))
                    ResetSkill();
                
                if (Price == 0)
                {
                    return;
                }

                if (CheckBuyState(SnakeUpgradeNoAds0))
                    RemoveAds();
                if (CheckBuyState(SnakeUpgradeDoubleFood0))
                    DoubleFood();
                if (CheckBuyState(SnakeUpgradeMaxFood0))
                    MaxFood();
                if (CheckBuyState(SnakeUpgradeResetSkill0))
                    ResetSkill();
            }
            
            ShopItems = GetItemsModels().ToArray();
            
            IsInitialized.Value = true;
        }

        public static void BuyProductID(string productId, Action action)
        {
            Debug.Log("Try to buy: " + productId);
            if (DebugController.isDebug)
            {
                action?.Invoke();
                return;
            }

            if (!IsIAPInitialized())
            {
                return;
            }
            
            purchased = action;
            var product = StoreController.products.WithID(productId);
            if (product is {availableToPurchase: true})
            {
                StoreController.InitiatePurchase(product);
            }
        }


        public static string GetPriceForId(string id)
        {
            return StoreController.products.WithStoreSpecificID(id).metadata.localizedPriceString;
        }

        public static string GetDescriptionForId(string id)
        {
            return StoreController.products.WithStoreSpecificID(id).metadata.localizedDescription;
        }

        public void BuyBundleSmall()
        {

            UpgradesManager.AllCoins += 1000;
            PlayerData.Diamond += 10;
            Managers.UIManager.Instance.UpdateCoinValue();
        }
        
        public void BuyBundleMedium()
        {
            UpgradesManager.AllCoins += 10000;
            PlayerData.Diamond += 100;
            UIManager.Instance.UpdateCoinValue();
        }
        
        public void BuyBundleBig()
        {
            UpgradesManager.AllCoins += 100000;
            PlayerData.Diamond += 1000;
            UIManager.Instance.UpdateCoinValue();
        }

        private void BuyFoodBundle(int quantity)
        {
            _currencyManager.ChangeFood(quantity);
        }
        
        public void BuyFoodBundleSmall()
        {
            UpgradesManager.AllCoins += 10000;
            UIManager.Instance.UpdateCoinValue();
        }
        
        public void BuyFoodBundleMedium()
        {
            UpgradesManager.AllCoins += 100000;
            UIManager.Instance.UpdateCoinValue();
        }
        
        public void BuyFoodBundleLarge()
        {
            UpgradesManager.AllCoins += 1000;
            UIManager.Instance.UpdateCoinValue();
        }

        private void BuyHardBundle(int quantity)
        {
            _currencyManager.ChangeHard(quantity);
        }
        
        public void BuyDiamondBundleSmall()
        {
            PlayerData.Diamond += 10;
        }
        
        public void BuyDiamondBundleMedium()
        {
            PlayerData.Diamond += 100;
        }
        
        public void BuyDiamondBundleLarge()
        {
            PlayerData.Diamond += 1000;
        } 
        
        public void BuyRemoveAds()
        {
            RemoveAds();
        }
        
        private void RemoveAds()
        {
            IsSubscribeEnable = true;
            _BuyRemoveAdsButton.SetActive(false);
            MainShopManager.Instance.HideRewardButtons();
        }
        
        public void BuyResetProgress()
        {
            ResetSkill();
        }
        
        private void ResetSkill()
        {
            _BuyResetProgressButton.SetActive(false);
            IsResetProgressBuying = true;
            Managers.UIManager.Instance.changeShopEventSecond += () =>
            {
                Managers.EvolveShopManager.Instance.ChoseSkill(0);
            };
        }

        public void CurrentMaxFood()
        {
            _BuyMaxFood.SetActive(false);
        }
        
        private void MaxFood()
        {
            _BuyMaxFoodButton.SetActive(false);
            _BuyMaxFood.SetActive(true);
        } 
        
        public void BuyMaxFood()
        {
            MaxFood();
        } 
        
        public void BuyDoubleFood()
        {
            DoubleFood();


        }
        
        private void DoubleFood()
        {
            IsDoubleFoodBuying = true;
            _BuyDoubleFoodButton.SetActive(false);
        }

        //public static void BuyDefaultNonConsumable()
        //{
        //    AnalitycManager.NonConsumableOpen();

        //    BuyProductID(_nonConsumableNoAds, delegate
        //    {
        //        AnalitycManager.NonConsumablePayment();
        //    });
        //}  
    }
}


using System;
using System.Collections;
using Extensions.Core;
using Managers;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Shop
{
    public class ShopProduct : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _description;

        [SerializeField]
        private TextMeshProUGUI _price;

        [SerializeField]
        private string _storeId;

        [SerializeField]
        private UnityEvent action;

        private Action _callback;

        public string ItemId => _storeId;
        private string _purchaseId;

        private void Awake()
        {
            _description.CheckForNull();
            _price.CheckForNull();
            _storeId.CheckForNull();
        }

        private IEnumerator Start1()
        {
            while (!IAPManager.IsIAPInitialized())
            {
                yield return null;
            }
        
            _price.text = IAPManager.GetPriceForId(_storeId + ((IAPManager.Price == 1) ? "_1" : ""));
            _description.text = IAPManager.GetDescriptionForId(_storeId + ((IAPManager.Price == 1) ? "_1" : ""));
        
            GetComponentInChildren<Button>()?.onClick.AddListener(BuyProduct);
        }

        public void BuyProduct()
        {
            //IAPManager.BuyProductID(_storeId + ((IAPManager.Price == 1) ? "_1" : ""), action);

            if (DebugController.isDebug)
            {
                _callback?.Invoke();
            }
            else
            {
                IAPManager.BuyProductID(_purchaseId, _callback);
            }
        }

        public void Initialize(ShopItemModel model)
        {
            _purchaseId = model.ItemId;
        
            _price.text = model.Price;
            _description.text = model.Description;

            _callback = model.Callback;
        } 
    }
}
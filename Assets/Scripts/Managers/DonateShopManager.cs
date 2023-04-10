using TMPro;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class DonateShopManager : Singleton<DonateShopManager>
    {
        [SerializeField]
        private GameObject[] _shops;

        [SerializeField]
        private TextMeshProUGUI _titleText;

        [SerializeField]
        private GameObject[] _maksCurrentShop;

        private readonly string[] _nameShops = { "Bundles",  "Food", "Gems", "Unique Upgrades" };

        public void OpenShop(int shop)
        {
            _titleText.text = _nameShops[shop];
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            for (int i = 0; i < _shops.Length; i++)
            {
                _shops[i].SetActive(false);
                _maksCurrentShop[i].SetActive(false);
            }
            _shops[shop].SetActive(true);
            _maksCurrentShop[shop].SetActive(true);

        }



        public void InitShop()
        {
            InitPrice();
            InitDescription();
        }

        private void InitPrice()
        {

        }

        private void InitDescription()
        {

        }
    }
}

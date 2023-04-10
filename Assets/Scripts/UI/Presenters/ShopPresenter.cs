using UI.Interfaces;
using UI.Shop;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Presenters
{
    [RequireComponent(typeof(Button))]
    public class ShopPresenter : MonoBehaviour
    {
        [Inject] 
        private IScreenManager _screenManager;

        public void OpenShop()
        {
            _screenManager.Show<ShopScreenController>();
        }
    }
}
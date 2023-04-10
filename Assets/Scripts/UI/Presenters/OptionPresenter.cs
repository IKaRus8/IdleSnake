using Services.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Presenters
{
    [RequireComponent(typeof(Button))]
    public class OptionPresenter : MonoBehaviour
    {
        private const string OptionPopupPath = "Prefabs/UI/Popup/OptionPopup";
        
        [Inject] 
        private IPopupService _popupService;

        public void OpenOptionPopup()
        {
            _popupService.CreatePopup(OptionPopupPath, true);
        }
    }
}
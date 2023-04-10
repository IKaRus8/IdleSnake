using UnityEngine;

namespace Managers
{

    public class TutorialManager : Utilities.Singleton<TutorialManager>
    {
        [SerializeField]
        private GameObject _tutorialHand;
        [SerializeField]
        private GameObject _tutorialBoostHand;
        [SerializeField]
        private GameObject _tutorialEvolveHand;
        [SerializeField]
        private GameObject _tutorialDestroyHand;
        [SerializeField]
        private GameObject _tutorialAncestorHand;

        private bool _isHandShowPrefs
        {
            get => PlayerPrefs.GetInt("IsTutorialHandShow", 0)==1;
            set => PlayerPrefs.SetInt("IsTutorialHandShow", value ? 1 : 0);
        }  
        private bool _isHandBoostShowPrefs
        {
            get => PlayerPrefs.GetInt("IsTutorialBoostHandShow", 0)==1;
            set => PlayerPrefs.SetInt("IsTutorialBoostHandShow", value ? 1 : 0);
        }  
        private bool _isHandEvolveShowPrefs
        {
            get => PlayerPrefs.GetInt("IsTutorialEvolveHandShow", 0)==1;
            set => PlayerPrefs.SetInt("IsTutorialEvolveHandShow", value ? 1 : 0);
        } 
        private bool _isHandDestroyPrefs
        {
            get => PlayerPrefs.GetInt("IsTutorialDestroyHandShow", 0)==1;
            set => PlayerPrefs.SetInt("IsTutorialDestroyHandShow", value ? 1 : 0);
        }
        private bool _isHandAncestorPrefs
        {
            get => PlayerPrefs.GetInt("IsTutorialAncestorHandShow", 0)==1;
            set => PlayerPrefs.SetInt("IsTutorialAncestorHandShow", value ? 1 : 0);
        }

        private bool _isHandShow;
        private bool _isHandBoostShow;
        private bool _isHandEvolveShow;
        private bool _isHandDestroyShow;
        private bool _isHandAncestorShow;

        private void Start()
        {
            _isHandShow = _isHandShowPrefs;
            _isHandBoostShow = _isHandBoostShowPrefs;
            _isHandEvolveShow = _isHandEvolveShowPrefs;
            _isHandDestroyShow = _isHandDestroyPrefs;
            _isHandAncestorShow = _isHandAncestorPrefs;
            _tutorialHand.SetActive(!_isHandShow);
        }

        public void HideHand()
        {
            if (_isHandShow) return;
            _isHandShow = true;
            _isHandShowPrefs = _isHandShow;
            _tutorialHand.SetActive(false);
        }

        public void ShowBoostHand()
        {
            if (_isHandBoostShow) return;
            UIManager.Instance.changeShopEventFirst += ()=> _tutorialBoostHand.SetActive(true);
        }

        public void HideBoostHand()
        {
            if (_isHandBoostShow) return;
            _isHandBoostShow = true;
            _isHandBoostShowPrefs = _isHandBoostShow;
            _tutorialBoostHand.SetActive(false);
        }

        public void ShowEvolveHand()
        {
            if (_isHandEvolveShow) return;
            _tutorialEvolveHand.SetActive(true);
            UIManager.Instance.changeShopEventSecond +=  HideEvolveHand;
        }

        private void HideEvolveHand()
        {
            if (_isHandEvolveShow) return;
            _isHandEvolveShow = true;
            _isHandEvolveShowPrefs = _isHandEvolveShow;
            _tutorialEvolveHand.SetActive(false);
        }  
        
        public void ShowDestroyHand()
        {
            if (_isHandDestroyShow) return;
            _tutorialDestroyHand.SetActive(true);
            _tutorialDestroyHand.transform.position = FieldManager.SnakeStanObj.transform.position;
        }

        public void HideDestroyHand()
        {
            if (_isHandDestroyShow) return;
            _isHandDestroyShow = true;
            _isHandDestroyPrefs = _isHandDestroyShow;
            _tutorialDestroyHand.SetActive(false);
        }
        public void ShowAncestorHand()
        {
            if (_isHandAncestorShow) return;
            _tutorialAncestorHand.SetActive(true);
        }

        public void HideAncestorHand()
        {
            if (_isHandAncestorShow) return;
            _isHandAncestorShow = true;
            _isHandAncestorPrefs = _isHandAncestorShow;
            _tutorialAncestorHand.SetActive(false);
        }
    }
}

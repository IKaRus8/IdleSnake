using Extensions.Core;
using Managers.Interfaces;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Controller
{
    public class SnakeInfoController : MonoBehaviour
    {
        private const string GrowUpText = "Need more food:";
        
        [SerializeField] 
        private TextMeshProUGUI _label;
        [SerializeField] 
        private Button _upButton;

        [Inject] 
        private ISnakeLevelProvider _snakeLevelProvider;

        private void Awake()
        {
            _label.CheckForNull();
            _upButton.CheckForNull();
            
            _upButton.onClick.AddListener(_snakeLevelProvider.NewGrowLevel);

            _snakeLevelProvider.FoodLeft.Subscribe(OnGrowUpStatusChanged).AddTo(this);
        }

        public void SwitchVisibility()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        private void OnGrowUpStatusChanged(int foodLeft)
        {
            if (_snakeLevelProvider.IsGrowUpReady)
            {
                _label.gameObject.SetActive(false);
                _upButton.gameObject.SetActive(true);
            }
            else
            {
                _label.text = $"{GrowUpText} {foodLeft}";
                
                _label.gameObject.SetActive(true);
                _upButton.gameObject.SetActive(false);
            }
        }
    }
}
using Extensions.Core;
using Managers.Interfaces;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Controller
{
    public class SnakeLevelController : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI _levelText;
        
        [Inject] 
        private ISnakeLevelProvider _snakeLevelProvider;

        private void Awake()
        {
            _levelText.CheckForNull();

            _snakeLevelProvider.CurrentLevelRx.Subscribe(l => _levelText.text = l.ToString()).AddTo(this);
        }
    }
}
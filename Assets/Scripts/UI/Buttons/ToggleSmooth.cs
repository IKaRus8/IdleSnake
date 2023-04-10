using System.Collections;
using Extensions.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Buttons
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleSmooth : MonoBehaviour
    {
        public float Speed = 5f;
        
        [SerializeField] 
        private RectTransform _checmark;

        private Coroutine _coroutine;
        private Toggle _toggle;

        protected void Awake()
        {
            _checmark.CheckForNull();
            _toggle = GetComponent<Toggle>();
            
            OnValueChanged(_toggle.isOn);
            
            _toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(bool value)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            
            if (value)
            {
                _coroutine = StartCoroutine(ToEnable());
            }
            else
            {
                _coroutine = StartCoroutine(ToDisable());
            }
        }

        private IEnumerator ToEnable()
        {
            var startValue = _checmark.pivot.x;

            while (startValue < 1)
            {
                startValue += Time.deltaTime * Speed;

                _checmark.pivot = new Vector2(startValue, 0.5f);
                
                yield return null;
            }

            _checmark.pivot = new Vector2(1, 0.5f);
        }

        private IEnumerator ToDisable()
        {
            var startValue = _checmark.pivot.x;

            while (startValue > 0)
            {
                startValue -= Time.deltaTime * Speed;

                _checmark.pivot = new Vector2(startValue, 0.5f);

                yield return null;
            }
            
            _checmark.pivot = new Vector2(0, 0.5f);
        }

        protected void OnDestroy()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
        }
    }
}
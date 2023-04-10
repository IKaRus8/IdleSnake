using Extensions.Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ResourceView : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI _valueText;

        private void Awake()
        {
            _valueText.CheckForNull();
        }

        public void SetValue(int value)
        {
            _valueText.text = value.ToString();
        }
    }
}

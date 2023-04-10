using Models;
using UnityEngine;

namespace UI.Buttons
{
    public class GMButton : MonoBehaviour
    {
        public void OpenGM()
        {
            Application.OpenURL(URLConstants.GM);
        }
    }
}
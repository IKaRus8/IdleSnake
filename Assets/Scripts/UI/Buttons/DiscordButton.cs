using Models;
using UnityEngine;

namespace UI.Buttons
{
    public class DiscordButton : MonoBehaviour
    {
        public void OpenDiscord()
        {
            Application.OpenURL(URLConstants.Discord);
        }
    }
}
using UI.Interfaces;
using UI.Skills;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Presenters
{
    [RequireComponent(typeof(Button))]
    public class SkillScreenPresenter : MonoBehaviour
    {
        [Inject] 
        private IScreenManager _screenManager;

        public void OpenOldSkills()
        {
            _screenManager.Show<OldSkillScreenController>();
        }

        public void OpenSkills()
        {
            _screenManager.Show<SkillScreenController>();
        }
    }
}
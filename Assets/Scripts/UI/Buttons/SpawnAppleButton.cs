using UI.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Buttons
{
    [RequireComponent(typeof(Button))]
    public class SpawnAppleButton : MonoBehaviour
    {
        [Inject]
        private IAppleSpawner _appleSpawner;

        public void SpawnApple()
        {
            _appleSpawner.SpawnRandomApple();
        }
    }
}
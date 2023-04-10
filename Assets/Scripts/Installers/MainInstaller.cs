using System;
using System.Runtime.InteropServices;
using Extensions.Core;
using Managers;
using Managers.Interfaces;
using Services;
using Services.Interfaces;
using Signals;
using UI;
using UI.Interfaces;
using UI.Managers;
using UnityEngine;
using Zenject;

namespace Snake.Idle.Installer
{
    public class MainInstaller : MonoInstaller<MainInstaller>
    {
        [SerializeField]
        private LevelGrowManager _levelGrowManager;
        [SerializeField]
        private ScreenManager _screenManager;
        [SerializeField] 
        private FieldManager _fieldManager;
        [SerializeField] 
        private IAPManager _iapManager;
        [SerializeField]
        private UIContainer _uiContainer;
        [SerializeField] 
        private SoundManager _soundManager;
        [SerializeField]
        private QuestManager _questManager;

        private void CheckInstances()
        {
            _levelGrowManager.CheckForNull();
            _screenManager.CheckForNull();
            _fieldManager.CheckForNull();
            _iapManager.CheckForNull();
            _uiContainer.CheckForNull();
            _soundManager.CheckForNull();
            _questManager.CheckForNull();
        }

        public override void InstallBindings()
        {
            CheckInstances();
            
            Container.Bind<ICurrencyManager>().To<CurrencyManager>().AsSingle();

            Container.Bind<IPopupService>().To<PopupService>().AsSingle();

            Container.BindInterfacesTo<LevelGrowManager>().FromInstance(_levelGrowManager).AsSingle();

            Container.BindInterfacesTo<ScreenManager>().FromInstance(_screenManager).AsSingle();
            
            Container.BindInterfacesTo<FieldManager>().FromInstance(_fieldManager).AsSingle();

            Container.BindInterfacesTo<IAPManager>().FromInstance(_iapManager).AsSingle();

            Container.BindInterfacesTo<UIContainer>().FromInstance(_uiContainer).AsSingle();

            Container.BindInterfacesTo<SoundManager>().FromInstance(_soundManager).AsSingle();

            Container.BindInterfacesTo<QuestManager>().FromInstance(_questManager).AsSingle();
            
            BindSignals();
        }

        private void BindSignals()
        {
            SignalBusInstaller.Install(Container);
            
            Container.DeclareSignal<GameStartedSignal>();
        }
    }
}
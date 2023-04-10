using JetBrains.Annotations;
using Managers.Interfaces;
using Signals;
using UniRx;
using Zenject;

namespace Managers
{
    [UsedImplicitly]
    public class CurrencyManager : ICurrencyManager
    {
        public ReactiveProperty<int> FoodPointsRx { get; set; } = new();
        
        public ReactiveProperty<int> HardQuantityRx { get; set; } = new();

        [Inject]
        private void Construct(SignalBus signalBus)
        {
            signalBus.Subscribe<GameStartedSignal>(s => OnGameStarted());
        }

        private void OnGameStarted()
        {
            FoodPointsRx.Value = UpgradesManager.AllCoins;
            HardQuantityRx.Value = PlayerData.Diamond;
        }
        
        public void ChangeFood(int value)
        {
            FoodPointsRx.Value += value;

            UpgradesManager.AllCoins = FoodPointsRx.Value;
        }

        public void ChangeHard(int value)
        {
            HardQuantityRx.Value += value;

            PlayerData.Diamond = HardQuantityRx.Value;
        }
    }
}
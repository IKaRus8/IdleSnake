using UniRx;

namespace Managers.Interfaces
{
    public interface ICurrencyManager
    {
        ReactiveProperty<int> FoodPointsRx { get; }
        ReactiveProperty<int> HardQuantityRx { get; }

        void ChangeFood(int value);

        void ChangeHard(int value);
    }
}
using UniRx;

namespace Managers.Interfaces
{
    public interface ISnakeLevelProvider
    {
        ReactiveProperty<int> CurrentLevelRx { get; }
        
        ReactiveProperty<int> FoodLeft { get; }
        
        bool IsGrowUpReady { get; }

        void EatApple(int fruit, int id);

        void NewGrowLevel();
        
        void ResetAll();
    }
}
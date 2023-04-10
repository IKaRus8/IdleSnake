using Models;
using UniRx;

namespace Managers.Interfaces
{
    public interface IBundleProvider
    {
        ReactiveProperty<bool> IsInitialized { get; }

        ShopItemModel[] ShopItems { get; }
        
        string[] FoodBundle { get; }
        
        string[] GemBundle { get; }
    }
}
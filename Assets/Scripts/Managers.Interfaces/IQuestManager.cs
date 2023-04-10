using UniRx;

namespace Managers.Interfaces
{
    public interface IQuestManager
    {
        ReactiveProperty<string> QuestProgress { get; }
    }
}
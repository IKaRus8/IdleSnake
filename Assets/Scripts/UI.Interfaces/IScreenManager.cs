namespace UI.Interfaces
{
    public interface IScreenManager
    {
        void Show<T>() where T : BaseScreen;

        void HideAll();
    }
}
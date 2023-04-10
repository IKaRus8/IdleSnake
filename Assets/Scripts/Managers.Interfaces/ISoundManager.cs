namespace Managers.Interfaces
{
    public interface ISoundManager
    {
        bool IsSoundOn { get; }
        
        bool IsMusicOn { get; }
        
        void ChangeSoundOff(bool value);

        void ChangeMusic(bool isMusicOn);

        void PlayBoost();
    }
}
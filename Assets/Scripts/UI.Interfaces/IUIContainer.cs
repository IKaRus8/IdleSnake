using UnityEngine;

namespace UI.Interfaces
{
    public interface IUIContainer
    {
        Transform PopupContainer { get; }
        GameObject ScreenBack { get; }
    }
}
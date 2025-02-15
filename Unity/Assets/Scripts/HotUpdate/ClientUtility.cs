using JKFrame;
using UnityEngine;

public static class ClientUtility
{
    public const string emptySlotPath = "UI_EmptySlot";
    public static bool GetWindowActiveState<T>(out T window) where T : UI_WindowBase
    {
        window = UISystem.GetWindow<T>();
        return !(window == null || !window.gameObject.activeInHierarchy);
    }

}

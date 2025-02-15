using JKFrame;
using UnityEngine;
using UnityEngine.UI;

public class UI_MessagePopupWindow : UI_WindowBase
{
    [SerializeField] private Text messageText;
    [SerializeField] private Image bglineImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private new Animation animation;

    public void ShowMessageByLocalzationKey(string localzationKey, Color color)
    {
        string message = LocalizationSystem.GetContent<LocalizationStringData>(localzationKey, LocalizationSystem.LanguageType).content;
        ShowMessage(message, color);
    }

    public void ShowMessage(string message, Color color)
    {
        messageText.text = message;
        messageText.color = color;
        bglineImage.color = color;
        iconImage.color = color;
        animation.Play("Popup");
    }
    #region 动画事件
    private void OnPopupEnd()
    {
        animation.Stop();
        UISystem.Close<UI_MessagePopupWindow>();
    }
    #endregion
}

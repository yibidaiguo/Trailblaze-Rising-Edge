using JKFrame;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenuWindow : UI_CustomWindowBase
{
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    public override void Init()
    {
        loginButton.onClick.AddListener(LoginButtonClick);
        registerButton.onClick.AddListener(RegisterButtonClick);
        settingsButton.onClick.AddListener(SettingsButtonClick);
        quitButton.onClick.AddListener(QuitButtonClick);
    }
    private void SettingsButtonClick()
    {
        UISystem.Show<UI_GameSettingsWindow>();
    }

    private void LoginButtonClick()
    {
        if (NetManager.Instance.IsConnectedClient || NetManager.Instance.InitClient())
        {
            UISystem.Show<UI_LoginWindow>();
        }
        else
        {
            UISystem.Show<UI_MessagePopupWindow>().ShowMessage("Net error", Color.red);
        }
    }

    private void RegisterButtonClick()
    {
        if (NetManager.Instance.IsConnectedClient || NetManager.Instance.InitClient())
        {
            UISystem.Show<UI_RegisterWindow>();
        }
        else
        {
            UISystem.Show<UI_MessagePopupWindow>().ShowMessage("Net error", Color.red);
        }
    }

    private void QuitButtonClick()
    {
        Application.Quit();
    }
}

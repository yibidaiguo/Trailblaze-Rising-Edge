using JKFrame;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UI_RegisterWindow : UI_CustomWindowBase
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button loginButton;
    [SerializeField] private InputField nameInputField;
    [SerializeField] private InputField passwordInputField;
    [SerializeField] private InputField rePasswordInputField;

    public override void Init()
    {
        closeButton.onClick.AddListener(CloseButtonClick);
        submitButton.onClick.AddListener(SubmitButtonClick);
        loginButton.onClick.AddListener(LoginButtonClick);
        nameInputField.onValueChanged.AddListener(OnInputFieldsValueChanged);
        passwordInputField.onValueChanged.AddListener(OnInputFieldsValueChanged);
        rePasswordInputField.onValueChanged.AddListener(OnInputFieldsValueChanged);
    }
    public override void OnShow()
    {
        base.OnShow();
        submitButton.interactable = false;
        passwordInputField.text = "";
        rePasswordInputField.text = "";
        NetMessageManager.Instance.RegisterMessageCallback(NetMessageType.S_C_Register, OnS_C_Register);
    }

    public override void OnClose()
    {
        base.OnClose();
        NetMessageManager.Instance.UnRegisterMessageCallback(NetMessageType.S_C_Register, OnS_C_Register);
    }

    private void CloseButtonClick()
    {
        UISystem.Close<UI_RegisterWindow>();
    }

    private void LoginButtonClick()
    {
        UISystem.Close<UI_RegisterWindow>();
        UISystem.Show<UI_LoginWindow>();
    }

    private void OnInputFieldsValueChanged(string arg0)
    {
        submitButton.interactable = AccountFormatUtility.CheckName(nameInputField.text)
            && AccountFormatUtility.CheckPassword(passwordInputField.text)
            && passwordInputField.text == rePasswordInputField.text;
    }
    private void SubmitButtonClick()
    {
        submitButton.interactable = false;
        NetMessageManager.Instance.SendMessageToServer(NetMessageType.C_S_Register, new C_S_Register
        {
            accountInfo = new AccountInfo
            {
                playerName = nameInputField.text,
                password = passwordInputField.text
            }
        });
    }
    private void OnS_C_Register(ulong clientID, INetworkSerializable serializable)
    {
        S_C_Register netMessage = (S_C_Register)serializable;
        if (netMessage.errorCode == ErrorCode.None)
        {
            UISystem.Show<UI_MessagePopupWindow>().ShowMessageByLocalzationKey("注册已成功", Color.green);
        }
        else
        {
            UISystem.Show<UI_MessagePopupWindow>().ShowMessageByLocalzationKey(netMessage.errorCode.ToString(), Color.red);
        }
        submitButton.interactable = true;
    }
}

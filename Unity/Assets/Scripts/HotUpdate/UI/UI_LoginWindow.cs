using JKFrame;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UI_LoginWindow : UI_CustomWindowBase
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private InputField nameInputField;
    [SerializeField] private InputField passwordInputField;
    [SerializeField] private Toggle remerberAccountToggle;

    public override void Init()
    {
        closeButton.onClick.AddListener(CloseButtonClick);
        submitButton.onClick.AddListener(SubmitButtonClick);
        registerButton.onClick.AddListener(RegisterButtonClick);
        nameInputField.onValueChanged.AddListener(OnInputFieldsValueChanged);
        passwordInputField.onValueChanged.AddListener(OnInputFieldsValueChanged);
    }

    public override void OnShow()
    {
        base.OnShow();
        submitButton.interactable = false;
        GameSetting gameSetting = ClientGlobal.Instance.gameSetting;
        nameInputField.text = gameSetting.remerberPlayerName != null ? gameSetting.remerberPlayerName : "";
        passwordInputField.text = gameSetting.remerberpassword != null ? gameSetting.remerberpassword : "";
        NetMessageManager.Instance.RegisterMessageCallback(NetMessageType.S_C_Login, OnS_C_Login);
    }
    public override void OnClose()
    {
        base.OnClose();
        NetMessageManager.Instance.UnRegisterMessageCallback(NetMessageType.S_C_Login, OnS_C_Login);
    }

    private void CloseButtonClick()
    {
        UISystem.Close<UI_LoginWindow>();
    }
    private void RegisterButtonClick()
    {
        UISystem.Close<UI_LoginWindow>();
        UISystem.Show<UI_RegisterWindow>();
    }

    private void OnInputFieldsValueChanged(string arg0)
    {
        submitButton.interactable = AccountFormatUtility.CheckName(nameInputField.text)
            && AccountFormatUtility.CheckPassword(passwordInputField.text);
    }
    private void SubmitButtonClick()
    {
        submitButton.interactable = false;
        // 记住账号
        if (remerberAccountToggle.isOn)
        {
            ClientGlobal.Instance.RemerberAccount(nameInputField.text, passwordInputField.text);
        }

        NetMessageManager.Instance.SendMessageToServer(NetMessageType.C_S_Login, new C_S_Login
        {
            accountInfo = new AccountInfo
            {
                playerName = nameInputField.text,
                password = passwordInputField.text
            }
        });
    }
    private void OnS_C_Login(ulong clientID, INetworkSerializable serializable)
    {
        submitButton.interactable = true;
        S_C_Login netMessage = (S_C_Login)serializable;
        if (netMessage.errorCode == ErrorCode.None)
        {
            ClientGlobal.Instance.EnterGameScene();
        }
        else
        {
            UISystem.Show<UI_MessagePopupWindow>().ShowMessageByLocalzationKey(netMessage.errorCode.ToString(), Color.red);
        }
    }

}

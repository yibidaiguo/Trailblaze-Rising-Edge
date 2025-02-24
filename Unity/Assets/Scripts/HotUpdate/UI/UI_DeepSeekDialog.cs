using System;
using System.Collections;
using JKFrame;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_DeepSeekDialog : UI_CustomWindowBase, IPointerClickHandler
{
    [SerializeField] private Text npcNameText;
    [SerializeField] private Text contentText;
    [SerializeField] private Text playerNameText;
    [SerializeField] private InputField inputField;
    [SerializeField] private Button closeButton;
    private string npcNameKey;
    private string currentAIAnswer;

    public override void Init()
    {
        playerNameText.text = PlayerManager.Instance.localPlayer.name;
        closeButton.onClick.AddListener(Close);
        inputField.onEndEdit.AddListener(OnSubmit);
    }

    public void Show(string playerName, string npcNameKey,DialogConfig dialogConfig)
    {
        this.npcNameKey = npcNameKey;
        playerNameText.text = playerName;
        NetMessageManager.Instance.RegisterMessageCallback(NetMessageType.S_C_AIAnswer, OnAIAnswer);
    }

    public override void OnClose()
    {
        base.OnClose();
        NetMessageManager.Instance.UnRegisterMessageCallback(NetMessageType.S_C_AIAnswer, OnAIAnswer);
    }

    private void OnAIAnswer(ulong clientID, INetworkSerializable serializable)
    {
        S_C_AIAnswer msg = (S_C_AIAnswer)serializable;
        StartShowAiAnswer(msg.message);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        StopAllCoroutines();
        contentText.text = currentAIAnswer;
    }

    private void StartShowAiAnswer(string message)
    {
        StartCoroutine(ShowAIAnswer(message));
    }

    private IEnumerator ShowAIAnswer(string message)
    {
        if (message == null) yield break;
        npcNameText.text = LocalizationSystem
            .GetContent<LocalizationStringData>(npcNameKey, LocalizationSystem.LanguageType).content;
        currentAIAnswer = message;
        // 逐字效果
        string target = message;
        string current = "";
        foreach (char item in target)
        {
            current += item;
            yield return CoroutineTool.WaitForSeconds(0.1f);
            contentText.text = current;
        }
    }

    private void OnSubmit(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return;
        NetMessageManager.Instance.SendMessageToServer(NetMessageType.C_S_ChatToAI,
            new C_S_ChatToAI() {npcName = "Crafter0", message = content });
        inputField.text = "";
        inputField.Select();
        inputField.ActivateInputField();
    }

    private void Close()
    {
        UISystem.Close<UI_DeepSeekDialog>();
    }
}
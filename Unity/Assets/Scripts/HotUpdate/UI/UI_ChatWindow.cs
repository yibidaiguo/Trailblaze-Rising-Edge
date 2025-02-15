using JKFrame;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ChatWindow : UI_CustomWindowBase
{
    private const int itemCount = 15;
    [SerializeField] private Transform main;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform itemRoot;
    [SerializeField] private InputField chatInputField;
    private Image scrollRectImage;
    private Queue<UI_ChatWindowItem> itemQueue = new Queue<UI_ChatWindowItem>(itemCount);

    public override void Init()
    {
        scrollRectImage = scrollRect.GetComponent<Image>();
        chatInputField.onSubmit.AddListener(OnChatInputFieldSubmit);
        main.OnMouseEnter(OnEnter);
        main.OnMouseExit(OnExit);
        OnExit(null);
    }
    public override void OnShow()
    {
        base.OnShow();
        NetMessageManager.Instance.RegisterMessageCallback(MessageType.S_C_ChatMessage, OnChatMessage);
    }

    private void OnChatMessage(ulong serverID, INetworkSerializable serializable)
    {
        S_C_ChatMessage message = (S_C_ChatMessage)serializable;
        AddItem(message.playerName, message.message);
    }

    private void OnEnter(PointerEventData data)
    {
        chatInputField.gameObject.SetActive(true);
        scrollRect.vertical = true;
        scrollRectImage.color = new Color(0, 0, 0, 0.2f);
    }

    private void OnExit(PointerEventData data)
    {
        chatInputField.gameObject.SetActive(false);
        scrollRect.vertical = false;
        scrollRectImage.color = new Color(0, 0, 0, 0);
    }

    private void OnChatInputFieldSubmit(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return; // 避免发送纯空格
        // 发送聊天消息
        NetMessageManager.Instance.SendMessageToServer(MessageType.C_S_ChatMessage, new C_S_ChatMessage
        {
            message = content
        });
        chatInputField.text = "";
        chatInputField.Select();
        chatInputField.ActivateInputField();    // 让输入框重新成为焦点
    }

    public void AddItem(string name, string content)
    {
        // 如果原本就在最下方，在收到新消息时，要自动滑到最下方
        bool onEnd = scrollRect.verticalNormalizedPosition <= 0.1f; // 0代表最下方
        // 清除最上方的
        if (itemQueue.Count >= itemCount)
        {
            DestroyItem(itemQueue.Dequeue());
        }
        UI_ChatWindowItem item = CreateItem();
        itemQueue.Enqueue(item);
        item.Init(name, content);
        // 有一种特殊情况，就是在未填满之前（消息不够多）verticalNormalizedPosition会一直是0，但是当超出范围的瞬间会变成1，这种情况也要去底部
        // Unity的更新构建机制并不是当前帧，所以最好延迟2帧去执行操作
        if (onEnd)
        {
            StartCoroutine(LockScorllToEnd());
        }
    }

    private IEnumerator LockScorllToEnd()
    {
        yield return CoroutineTool.WaitForFrames(2);
        scrollRect.verticalNormalizedPosition = 0;
    }

    private UI_ChatWindowItem CreateItem()
    {
        return ResSystem.InstantiateGameObject<UI_ChatWindowItem>(nameof(UI_ChatWindowItem), itemRoot);
    }

    private void DestroyItem(UI_ChatWindowItem item)
    {
        item.GameObjectPushPool();
    }
    public override void OnClose()
    {
        base.OnClose();
        NetMessageManager.Instance.UnRegisterMessageCallback(MessageType.S_C_ChatMessage, OnChatMessage);
        PoolSystem.ClearGameObject(nameof(UI_ChatWindowItem));
    }
}

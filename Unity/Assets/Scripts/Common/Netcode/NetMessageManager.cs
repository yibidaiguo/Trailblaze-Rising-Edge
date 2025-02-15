using JKFrame;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetMessageManager : SingletonMono<NetMessageManager>
{
    private CustomMessagingManager messagingManager => NetManager.Instance.CustomMessagingManager;
    private Dictionary<MessageType, Action<ulong, INetworkSerializable>> receiveMessageCallbackDic = new Dictionary<MessageType, Action<ulong, INetworkSerializable>>();
    public void Init()
    {
        messagingManager.OnUnnamedMessage += ReceiveMessage;
    }
    private void ReceiveMessage(ulong clientId, FastBufferReader reader)
    {
        //try
        //{
        reader.ReadValueSafe(out MessageType messageType);
        Debug.Log("收到网络信息:" + messageType);
        switch (messageType)
        {
            case MessageType.C_S_Register:
                reader.ReadValueSafe(out C_S_Register C_S_Register);
                TriggerMessageCallback(MessageType.C_S_Register, clientId, C_S_Register);
                break;
            case MessageType.C_S_Login:
                reader.ReadValueSafe(out C_S_Login C_S_Login);
                TriggerMessageCallback(MessageType.C_S_Login, clientId, C_S_Login);
                break;
            case MessageType.S_C_Register:
                reader.ReadValueSafe(out S_C_Register S_C_Register);
                TriggerMessageCallback(MessageType.S_C_Register, clientId, S_C_Register);
                break;
            case MessageType.S_C_Login:
                reader.ReadValueSafe(out S_C_Login S_C_Login);
                TriggerMessageCallback(MessageType.S_C_Login, clientId, S_C_Login);
                break;
            case MessageType.C_S_EnterGame:
                reader.ReadValueSafe(out C_S_EnterGame C_S_EnterGame);
                TriggerMessageCallback(MessageType.C_S_EnterGame, clientId, C_S_EnterGame);
                break;
            case MessageType.C_S_Disonnect:
                reader.ReadValueSafe(out C_S_Disonnect C_S_Disonnect);
                TriggerMessageCallback(MessageType.C_S_Disonnect, clientId, C_S_Disonnect);
                break;
            case MessageType.S_C_Disonnect:
                reader.ReadValueSafe(out S_C_Disonnect S_C_Disonnect);
                TriggerMessageCallback(MessageType.S_C_Disonnect, clientId, S_C_Disonnect);
                break;
            case MessageType.C_S_ChatMessage:
                reader.ReadValueSafe(out C_S_ChatMessage C_S_ChatMessage);
                TriggerMessageCallback(MessageType.C_S_ChatMessage, clientId, C_S_ChatMessage);
                break;
            case MessageType.S_C_ChatMessage:
                reader.ReadValueSafe(out S_C_ChatMessage S_C_ChatMessage);
                TriggerMessageCallback(MessageType.S_C_ChatMessage, clientId, S_C_ChatMessage);
                break;
            case MessageType.C_S_GetBagData:
                reader.ReadValueSafe(out C_S_GetBagData C_S_GetBagData);
                TriggerMessageCallback(MessageType.C_S_GetBagData, clientId, C_S_GetBagData);
                break;
            case MessageType.S_C_GetBagData:
                reader.ReadValueSafe(out S_C_GetBagData S_C_GetBagData);
                TriggerMessageCallback(MessageType.S_C_GetBagData, clientId, S_C_GetBagData);
                break;
            case MessageType.C_S_BagUseItem:
                reader.ReadValueSafe(out C_S_BagUseItem C_S_BagUseItem);
                TriggerMessageCallback(MessageType.C_S_BagUseItem, clientId, C_S_BagUseItem);
                break;
            case MessageType.S_C_BagUpdateItem:
                reader.ReadValueSafe(out S_C_BagUpdateItem S_C_BagUpdateItem);
                TriggerMessageCallback(MessageType.S_C_BagUpdateItem, clientId, S_C_BagUpdateItem);
                break;
            case MessageType.C_S_BagSwapItem:
                reader.ReadValueSafe(out C_S_BagSwapItem C_S_BagSwapItem);
                TriggerMessageCallback(MessageType.C_S_BagSwapItem, clientId, C_S_BagSwapItem);
                break;
            case MessageType.S_C_ShortcutBarUpdateItem:
                reader.ReadValueSafe(out S_C_ShortcutBarUpdateItem S_C_ShortcutBarUpdateItem);
                TriggerMessageCallback(MessageType.S_C_ShortcutBarUpdateItem, clientId, S_C_ShortcutBarUpdateItem);
                break;
            case MessageType.C_S_ShortcutBarSetItem:
                reader.ReadValueSafe(out C_S_ShortcutBarSetItem C_S_ShortcutBarSetItem);
                TriggerMessageCallback(MessageType.C_S_ShortcutBarSetItem, clientId, C_S_ShortcutBarSetItem);
                break;
            case MessageType.C_S_ShortcutBarSwapItem:
                reader.ReadValueSafe(out C_S_ShortcutBarSwapItem C_S_ShortcutBarSwapItem);
                TriggerMessageCallback(MessageType.C_S_ShortcutBarSwapItem, clientId, C_S_ShortcutBarSwapItem);
                break;
            case MessageType.C_S_ShopBuyItem:
                reader.ReadValueSafe(out C_S_ShopBuyItem C_S_ShopBuyItem);
                TriggerMessageCallback(MessageType.C_S_ShopBuyItem, clientId, C_S_ShopBuyItem);
                break;
            case MessageType.S_C_UpdateCoinCount:
                reader.ReadValueSafe(out S_C_UpdateCoinCount S_C_UpdateCoinCount);
                TriggerMessageCallback(MessageType.S_C_UpdateCoinCount, clientId, S_C_UpdateCoinCount);
                break;
            case MessageType.C_S_BagSellItem:
                reader.ReadValueSafe(out C_S_BagSellItem C_S_BagSellItem);
                TriggerMessageCallback(MessageType.C_S_BagSellItem, clientId, C_S_BagSellItem);
                break;
            case MessageType.C_S_CraftItem:
                reader.ReadValueSafe(out C_S_CraftItem C_S_CraftItem);
                TriggerMessageCallback(MessageType.C_S_CraftItem, clientId, C_S_CraftItem);
                break;
            case MessageType.C_S_GetTaskDatas:
                reader.ReadValueSafe(out C_S_GetTaskDatas C_S_GetTaskDatas);
                TriggerMessageCallback(MessageType.C_S_GetTaskDatas, clientId, C_S_GetTaskDatas);
                break;
            case MessageType.S_C_GetTaskDatas:
                reader.ReadValueSafe(out S_C_GetTaskDatas S_C_GetTaskDatas);
                TriggerMessageCallback(MessageType.S_C_GetTaskDatas, clientId, S_C_GetTaskDatas);
                break;
            case MessageType.C_S_CompleteDialogTask:
                reader.ReadValueSafe(out C_S_CompleteDialogTask C_S_CompleteDialogTask);
                TriggerMessageCallback(MessageType.C_S_CompleteDialogTask, clientId, C_S_CompleteDialogTask);
                break;
            case MessageType.S_C_CompleteTask:
                reader.ReadValueSafe(out S_C_CompleteTask S_C_CompleteTask);
                TriggerMessageCallback(MessageType.S_C_CompleteTask, clientId, S_C_CompleteTask);
                break;
            case MessageType.S_C_AddTask:
                reader.ReadValueSafe(out S_C_AddTask S_C_AddTask);
                TriggerMessageCallback(MessageType.S_C_AddTask, clientId, S_C_AddTask);
                break;
            case MessageType.S_C_UpdateTask:
                reader.ReadValueSafe(out S_C_UpdateTask S_C_UpdateTask);
                TriggerMessageCallback(MessageType.S_C_UpdateTask, clientId, S_C_UpdateTask);
                break;
            case MessageType.C_S_AddTask:
                reader.ReadValueSafe(out C_S_AddTask C_S_AddTask);
                TriggerMessageCallback(MessageType.C_S_AddTask, clientId, C_S_AddTask);
                break;
        }
        //}
        //catch (Exception e)
        //{
        //    Debug.Log("消息接收失败!" + e.Message);
        //}
    }

    private FastBufferWriter WriteData<T>(MessageType messageType, T data) where T : INetworkSerializable
    {
        // 默认1024字节，当不足时候会在10240范围内自动扩容
        FastBufferWriter writer = new FastBufferWriter(1024, Allocator.Temp, 10240);
        using (writer)
        {
            writer.WriteValueSafe(messageType); // 协议头
            writer.WriteValueSafe(data);    // 协议主体
        }
        return writer;
    }

    public void SendMessageToServer<T>(MessageType messageType, T data) where T : INetworkSerializable
    {
        messagingManager.SendUnnamedMessage(NetManager.ServerClientId, WriteData(messageType, data));
    }
    public void SendMessageToClient<T>(MessageType messageType, T data, ulong clientID) where T : INetworkSerializable
    {
        messagingManager.SendUnnamedMessage(clientID, WriteData(messageType, data));
    }
    public void SendMessageToClients<T>(MessageType messageType, T data, IReadOnlyList<ulong> clientIDS) where T : INetworkSerializable
    {
        messagingManager.SendUnnamedMessage(clientIDS, WriteData(messageType, data));
    }
    public void SendMessageAllClient<T>(MessageType messageType, T data, IReadOnlyList<ulong> clientIDS) where T : INetworkSerializable
    {
        messagingManager.SendUnnamedMessageToAll(WriteData(messageType, data));
    }


    public void RegisterMessageCallback(MessageType messageType, Action<ulong, INetworkSerializable> callback)
    {
        if (receiveMessageCallbackDic.ContainsKey(messageType))
        {
            receiveMessageCallbackDic[messageType] += callback;
        }
        else
        {
            receiveMessageCallbackDic.Add(messageType, callback);
        }
    }
    public void UnRegisterMessageCallback(MessageType messageType, Action<ulong, INetworkSerializable> callback)
    {
        if (receiveMessageCallbackDic.ContainsKey(messageType))
        {
            receiveMessageCallbackDic[messageType] -= callback;
        }
    }

    private void TriggerMessageCallback(MessageType messageType, ulong clientID, INetworkSerializable data)
    {
        if (receiveMessageCallbackDic.TryGetValue(messageType, out Action<ulong, INetworkSerializable> callback))
        {
            callback?.Invoke(clientID, data);
        }
    }
}

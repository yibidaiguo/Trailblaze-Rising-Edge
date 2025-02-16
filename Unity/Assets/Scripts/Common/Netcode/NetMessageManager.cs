using JKFrame;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;

public partial class NetMessageManager : SingletonMono<NetMessageManager>
{
    private CustomMessagingManager messagingManager => NetManager.Instance.CustomMessagingManager;
    private Dictionary<MessageType, Action<ulong, INetworkSerializable>> receiveMessageCallbackDic = new();

    partial void OnInit();
    public void Init()
    {
        OnInit();
    }
    
    //收发消息，自动生成
    partial void ReceiveMessage(ulong clientId, FastBufferReader reader);

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

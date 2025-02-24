using JKFrame;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public partial class NetMessageManager : SingletonMono<NetMessageManager>
{
    private CustomMessagingManager messagingManager => NetManager.Instance.CustomMessagingManager;
    private Dictionary<NetMessageType, Action<ulong, INetworkSerializable>> receiveMessageCallbackDic = new();

    partial void OnInit();
    public void Init()
    {
        OnInit();
    }
    
    //收发消息，自动生成
    partial void ReceiveMessage(ulong clientId, FastBufferReader reader);

    private FastBufferWriter WriteData<T>(NetMessageType netMessageType, T data) where T : INetworkSerializable
    {
        // 默认1024字节，当不足时候会在10240范围内自动扩容
        FastBufferWriter writer = new FastBufferWriter(1024, Allocator.Temp, 10240);
        using (writer)
        {
            writer.WriteValueSafe(netMessageType); // 协议头
            writer.WriteValueSafe(data);    // 协议主体
        }
        return writer;
    }

    public void SendMessageToServer<T>(NetMessageType netMessageType, T data) where T : INetworkSerializable
    {
        messagingManager.SendUnnamedMessage(NetManager.ServerClientId, WriteData(netMessageType, data));
    }
    public void SendMessageToClient<T>(NetMessageType netMessageType, T data, ulong clientID) where T : INetworkSerializable
    {
        messagingManager.SendUnnamedMessage(clientID, WriteData(netMessageType, data));
    }
    public void SendMessageToClients<T>(NetMessageType netMessageType, T data, IReadOnlyList<ulong> clientIDS) where T : INetworkSerializable
    {
        messagingManager.SendUnnamedMessage(clientIDS, WriteData(netMessageType, data));
    }
    public void SendMessageAllClient<T>(NetMessageType netMessageType, T data, IReadOnlyList<ulong> clientIDS) where T : INetworkSerializable
    {
        messagingManager.SendUnnamedMessageToAll(WriteData(netMessageType, data));
    }


    public void RegisterMessageCallback(NetMessageType netMessageType, Action<ulong, INetworkSerializable> callback)
    {
        if (receiveMessageCallbackDic.ContainsKey(netMessageType))
        {
            receiveMessageCallbackDic[netMessageType] += callback;
        }
        else
        {
            receiveMessageCallbackDic.Add(netMessageType, callback);
        }
    }
    public void UnRegisterMessageCallback(NetMessageType netMessageType, Action<ulong, INetworkSerializable> callback)
    {
        if (receiveMessageCallbackDic.ContainsKey(netMessageType))
        {
            receiveMessageCallbackDic[netMessageType] -= callback;
        }
    }

    private void TriggerMessageCallback(NetMessageType netMessageType, ulong clientID, INetworkSerializable data)
    {
        if (receiveMessageCallbackDic.TryGetValue(netMessageType, out Action<ulong, INetworkSerializable> callback))
        {
            callback?.Invoke(clientID, data);
        }
    }
}

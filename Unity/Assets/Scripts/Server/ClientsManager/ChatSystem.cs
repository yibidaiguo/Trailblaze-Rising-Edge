using JKFrame;
using System;
using System.Collections.Generic;
using Unity.Netcode;

// 负责聊天系统的部分
public partial class ClientsManager : SingletonMono<ClientsManager>
{
    public void InitChatSystem()
    {
        NetMessageManager.Instance.RegisterMessageCallback(NetMessageType.C_S_ChatMessage, OnClientChatMessage);
    }

    // 当客户端发来聊天消息
    private void OnClientChatMessage(ulong clientID, INetworkSerializable serializable)
    {
        string chatMessage = ((C_S_ChatMessage)serializable).message;
        if (string.IsNullOrWhiteSpace(chatMessage)) return; // 消息有效性验证
        if (!clientIDDic.TryGetValue(clientID, out Client sourceClient) || sourceClient.playerData == null) return; // 检查源头客户端的有效性
        // 发送给所有游戏状态下的客户端
        if (clientStateDic.TryGetValue(ClientState.Gaming, out HashSet<Client> clients))
        {
            S_C_ChatMessage message = new S_C_ChatMessage { playerName = sourceClient.playerData.name, message = chatMessage };
            foreach (Client client in clients)
            {
                NetMessageManager.Instance.SendMessageToClient(NetMessageType.S_C_ChatMessage, message, client.clientID);
            }
        }
    }
}
using Unity.Netcode;

public partial class ClientsManager
{
    void InitAISystem()
    {
        NetMessageManager.Instance.RegisterMessageCallback(NetMessageType.C_S_ChatToAI, OnClientChatToAI);
    }

    private void OnClientChatToAI(ulong clientID, INetworkSerializable serializable)
    {
        if (clientIDDic.TryGetValue(clientID, out Client client) && client.playerData != null)
        {
            C_S_ChatToAI message = (C_S_ChatToAI)serializable;
            DeepSeekServerController.Instance.SendMessageToDeepSeek(clientID,message.message,message.npcName,
                client.playerData.name);
        }
    }
}
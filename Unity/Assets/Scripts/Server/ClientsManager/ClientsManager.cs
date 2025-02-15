using JKFrame;
using System.Collections.Generic;

public partial class ClientsManager : SingletonMono<ClientsManager>
{
    private Dictionary<ClientState, HashSet<Client>> clientStateDic;
    // Key:clientID
    private Dictionary<ulong, Client> clientIDDic;
    // Key:账号 Value:ClientID
    private Dictionary<string, ulong> accountDic;
    public void Init()
    {
        clientStateDic = new Dictionary<ClientState, HashSet<Client>>()
        {
            { ClientState.Connected,new HashSet<Client>(100)},
            { ClientState.Logined,new HashSet<Client>(100)},
            { ClientState.Gaming,new HashSet<Client>(100)},
        };
        clientIDDic = new Dictionary<ulong, Client>(100);
        accountDic = new Dictionary<string, ulong>(100);
        NetManager.Instance.OnClientConnectedCallback += OnClientConnected;
        NetManager.Instance.OnClientDisconnectCallback += OnClientNetCodeDisconnect;
        InitLoginSystem();
        InitChatSystem();
        InitItemSystem();
        InitTaskSystem();
    }

    private void SetClientState(ulong clientID, ClientState newState)
    {
        if (clientIDDic.TryGetValue(clientID, out Client client))
        {
            clientStateDic[client.clientState].Remove(client);
            clientStateDic[newState].Add(client);
            client.clientState = newState;
        }
    }

    // 连接成功
    private void OnClientConnected(ulong clientID)
    {
        Client client = ResSystem.GetOrNew<Client>();
        client.clientID = clientID;
        clientIDDic.Add(clientID, client);
        SetClientState(clientID, ClientState.Connected);
    }

    // 客户端完全退出
    private void OnClientNetCodeDisconnect(ulong clientID)
    {
        if (clientIDDic.Remove(clientID, out Client client))
        {
            clientStateDic[client.clientState].Remove(client);
            if (client.playerData != null) accountDic.Remove(client.playerData.name);
            // 目前采用的是Netcode自己的管理，也就是客户端掉线会自动清除所述网络对象
            if (client.playerController != null) NetManager.Instance.DestroyObject(client.playerController.mainController.NetworkObject);
            client.playerData = null;
            client.playerController = null;
            client.OnDestroy();
        }
    }

    private void SavePlayerData(Client client)
    {
        if (client.playerController != null)
        {
            client.playerData.characterData.hp = client.playerController.mainController.currentHp.Value;
            client.playerData.characterData.position = client.playerController.transform.position;
        }
        DatabaseManager.Instance.SavePlayerData(client.playerData);
    }
}

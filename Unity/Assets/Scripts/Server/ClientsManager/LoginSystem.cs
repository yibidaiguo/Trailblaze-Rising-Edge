using JKFrame;
using System;
using Unity.Netcode;
using UnityEngine;

// 负责登录系统的部分
public partial class ClientsManager : SingletonMono<ClientsManager>
{
    public void InitLoginSystem()
    {
        NetMessageManager.Instance.RegisterMessageCallback(MessageType.C_S_Register, OnClientRegister);
        NetMessageManager.Instance.RegisterMessageCallback(MessageType.C_S_Login, OnClientLogin);
        NetMessageManager.Instance.RegisterMessageCallback(MessageType.C_S_EnterGame, OnClientEnterGame);
        NetMessageManager.Instance.RegisterMessageCallback(MessageType.C_S_Disonnect, OnClientDisonnect);
    }

    // 申请注册
    private void OnClientRegister(ulong clientID, INetworkSerializable serializable)
    {
        C_S_Register netMessage = (C_S_Register)serializable;
        AccountInfo accountInfo = netMessage.accountInfo;
        S_C_Register result = new S_C_Register { errorCode = ErrorCode.None };
        // 校验格式
        if (!AccountFormatUtility.CheckName(accountInfo.playerName)
            || !AccountFormatUtility.CheckPassword(accountInfo.password))
        {
            result.errorCode = ErrorCode.AccountFormat;
        }
        // 校验是否已有玩家
        else if (DatabaseManager.Instance.GetPlayerData(accountInfo.playerName) != null)
        {
            result.errorCode = ErrorCode.NameDuplication;
        }
        else
        {
            CreateDefaultPlayerData(accountInfo);
        }
        // 回复客户端
        NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_Register, result, clientID);
    }

    // 生成默认账号数据
    private PlayerData CreateDefaultPlayerData(AccountInfo accountInfo)
    {
        PlayerData playerData = ResSystem.GetOrNew<PlayerData>();
        ServerConfig serverConfig = ServerResSystem.serverConfig;

        // 账号数据
        playerData.name = accountInfo.playerName;
        playerData.password = accountInfo.password;

        // 角色数据
        playerData.characterData = new CharacterData
        {
            position = serverConfig.playerDefaultPostion,
            usedWeaponName = "Weapon_0",
            hp = serverConfig.playerMaxHp
        };

        // 任务数据
        playerData.taskDatas.tasks.Add(new TaskData { taskConfigId = "Task_1" });

        // 物品数据
        playerData.bagData.itemList[0] = (new WeaponData() { id = "Weapon_0" });
        playerData.bagData.itemList[1] = (new WeaponData() { id = "Weapon_1" });
        playerData.bagData.itemList[2] = (new ConsumableData() { id = "Consumable_0", count = 1 });
        playerData.bagData.itemList[3] = (new ConsumableData() { id = "Consumable_1", count = 2 });
        playerData.bagData.itemList[4] = (new ConsumableData() { id = "Consumable_2", count = 3 });
        playerData.bagData.itemList[5] = (new ConsumableData() { id = "Consumable_3", count = 4 });
        playerData.bagData.itemList[6] = (new ConsumableData() { id = "Consumable_4", count = 5 });
        playerData.bagData.itemList[7] = (new MaterialData() { id = "Material_0", count = 4 });
        playerData.bagData.itemList[8] = (new MaterialData() { id = "Material_1", count = 5 });
        playerData.bagData.itemList[9] = (new MaterialData() { id = "Material_2", count = 99 });
        playerData.bagData.shortcutBarIndes[0] = 0;
        playerData.bagData.coinCount = ServerResSystem.serverConfig.playerDefaultCointCount;
        DatabaseManager.Instance.CreatePlayerData(playerData);
        return playerData;
    }

    // 申请登录
    private void OnClientLogin(ulong clientID, INetworkSerializable serializable)
    {
        C_S_Login netMessage = (C_S_Login)serializable;
        AccountInfo accountInfo = netMessage.accountInfo;
        S_C_Login result = new S_C_Login { errorCode = ErrorCode.None };
        // 校验格式
        if (!AccountFormatUtility.CheckName(accountInfo.playerName)
            || !AccountFormatUtility.CheckPassword(accountInfo.password))
        {
            result.errorCode = ErrorCode.AccountFormat;
        }
        else
        {
            // 检查是否有这个玩家，并且账号信息正确
            PlayerData playerData = DatabaseManager.Instance.GetPlayerData(accountInfo.playerName);

            if (playerData == null || playerData.password != accountInfo.password)
            {
                result.errorCode = ErrorCode.NameOrPassword;
            }
            else
            {
                // 检查挤号
                if (accountDic.TryGetValue(accountInfo.playerName, out ulong oldClientID))
                {
                    // 通过旧客户端
                    NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_Disonnect, new S_C_Disonnect
                    {
                        errorCode = ErrorCode.AccountRepeatLogin
                    }, oldClientID);

                    // 设置旧客户端为已连接但是未登录状态
                    SetClientState(oldClientID, ClientState.Connected);

                    // 可能存在的角色需要销毁
                    if (clientIDDic.TryGetValue(oldClientID, out Client oldClient))
                    {
                        if (oldClient.playerController != null)
                        {
                            NetManager.Instance.DestroyObject(oldClient.playerController.mainController.NetworkObject);
                            oldClient.playerController = null;
                        }
                        oldClient.playerData = null;
                    }
                }
                accountDic[accountInfo.playerName] = clientID;

                // 玩家登录成功,关联Client和PlayerData
                Client client = clientIDDic[clientID];
                client.playerData = playerData;
                client.playerData.bagData.dataVersion = 0; // 背包版本号默认为0
                SetClientState(clientID, ClientState.Logined);
            }
        }
        // 回复客户端
        NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_Login, result, clientID);
    }
    // 玩家进入游戏
    private void OnClientEnterGame(ulong clientID, INetworkSerializable serializable)
    {
        // 无需回复客户端，直接创建角色
        Client client = clientIDDic[clientID];
        if (client.clientState == ClientState.Gaming) return;
        SetClientState(clientID, ClientState.Gaming);
        PlayerData playerData = client.playerData;
        CharacterData characterData = playerData.characterData;
        PlayerServerController serverController = NetManager.Instance.SpawnObject<PlayerServerController>(clientID, ServerResSystem.serverConfig.playerPrefab, characterData.position, Quaternion.Euler(0, characterData.rotation_Y, 0));
        // 初始化玩家的服务端控制脚本
        serverController.mainController.NetworkObject.SpawnWithOwnership(clientID);
        serverController.mainController.NetworkObject.NetworkShow(clientID);
        serverController.mainController.playerName.Value = playerData.name;
        serverController.mainController.usedWeaponName.Value = playerData.characterData.usedWeaponName;
        serverController.mainController.currentHp.Value = playerData.characterData.hp;
        client.playerController = serverController;
    }

    // 客户端退出菜单场景
    private void OnClientDisonnect(ulong clientID, INetworkSerializable serializable)
    {
        Client client = clientIDDic[clientID];
        // 设置旧客户端为已连接但是未登录状态
        SetClientState(clientID, ClientState.Connected);
        if (client.playerController != null)
        {
            NetManager.Instance.DestroyObject(client.playerController.mainController.NetworkObject);
            client.playerController = null;
        }
        if (client.playerData != null)
        {
            accountDic.Remove(client.playerData.name);
            client.playerData = null;
        }

        // 回复消息
        NetMessageManager.Instance.SendMessageToClient<S_C_Disonnect>(MessageType.S_C_Disonnect, default, clientID);
    }

    public void OnPlayerDie(PlayerServerController player)
    {
        // 重生
        player.mainController.currentHp.Value = ServerResSystem.serverConfig.playerRespawnHp;
        player.mainController.transform.position = ServerResSystem.serverConfig.playerDefaultPostion;
    }
}
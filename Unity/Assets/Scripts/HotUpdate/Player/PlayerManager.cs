using JKFrame;
using UnityEngine;
using Unity.Netcode;
public class PlayerManager : SingletonMono<PlayerManager>
{
    public PlayerClientController localPlayer { get; private set; }

    // 玩家是否可以控制角色，以后可能受到多个方面的影响，目前只和鼠标显示关联
    public bool playerControlEnable { get; private set; }
    public int UsedWeaponIndex => bagData.usedWeaponIndex;

    private bool requestOpenBagWindow = false;
    public BagData bagData { get; private set; }
    public TaskDatas taskDatas { get; private set; }

    public string PlayerName => localPlayer.mainController.playerName.Value.ToString();
    public void Init()
    {
        PlayerController.SetGetWeaponFunc(GetWeapon);
        EventSystem.AddTypeEventListener<SpawnPlayerEvent>(OnSpawnPlayerEvent);
        EventSystem.AddTypeEventListener<MouseActiveStateChangedEvent>(OnMouseActiveStateChangedEvent);
        NetMessageManager.Instance.RegisterMessageCallback(NetMessageType.S_C_GetBagData, OnS_C_GetBagData);
        NetMessageManager.Instance.RegisterMessageCallback(NetMessageType.S_C_BagUpdateItem, OnS_C_UpdateItem);
        NetMessageManager.Instance.RegisterMessageCallback(NetMessageType.S_C_ShortcutBarUpdateItem, OnS_C_ShortcutBarUpdateItem);
        NetMessageManager.Instance.RegisterMessageCallback(NetMessageType.S_C_UpdateCoinCount, OnS_C_UpdateCoinCount);
        NetMessageManager.Instance.RegisterMessageCallback(NetMessageType.S_C_GetTaskDatas, OnS_C_GetTaskDatas);
        NetMessageManager.Instance.RegisterMessageCallback(NetMessageType.S_C_CompleteTask, OnS_C_CompleteTask);
        NetMessageManager.Instance.RegisterMessageCallback(NetMessageType.S_C_AddTask, OnS_C_AddTask);
        NetMessageManager.Instance.RegisterMessageCallback(NetMessageType.S_C_UpdateTask, OnS_C_UpdateTask);
        ClientGlobal.Instance.ActiveMouse = false;
        RequestBagData();
        RequesetTaskDatas();
    }



    private void OnDestroy()
    {
        EventSystem.RemoveTypeEventListener<SpawnPlayerEvent>(OnSpawnPlayerEvent);
        EventSystem.RemoveTypeEventListener<MouseActiveStateChangedEvent>(OnMouseActiveStateChangedEvent);
        NetMessageManager.Instance.UnRegisterMessageCallback(NetMessageType.S_C_GetBagData, OnS_C_GetBagData);
        NetMessageManager.Instance.UnRegisterMessageCallback(NetMessageType.S_C_BagUpdateItem, OnS_C_UpdateItem);
        NetMessageManager.Instance.UnRegisterMessageCallback(NetMessageType.S_C_ShortcutBarUpdateItem, OnS_C_ShortcutBarUpdateItem);
        NetMessageManager.Instance.UnRegisterMessageCallback(NetMessageType.S_C_UpdateCoinCount, OnS_C_UpdateCoinCount);
        NetMessageManager.Instance.UnRegisterMessageCallback(NetMessageType.S_C_GetTaskDatas, OnS_C_GetTaskDatas);
        NetMessageManager.Instance.UnRegisterMessageCallback(NetMessageType.S_C_CompleteTask, OnS_C_CompleteTask);
        NetMessageManager.Instance.UnRegisterMessageCallback(NetMessageType.S_C_AddTask, OnS_C_AddTask);
        NetMessageManager.Instance.UnRegisterMessageCallback(NetMessageType.S_C_UpdateTask, OnS_C_UpdateTask);
    }



    private void OnSpawnPlayerEvent(SpawnPlayerEvent arg)
    {
        if (arg.newPlayer.IsOwner)
        {
            InitLocalPlayer((PlayerClientController)arg.newPlayer.sideController);
        }
    }

    public bool IsLoadingCompleted()
    {
        return localPlayer != null;
    }

    private void InitLocalPlayer(PlayerClientController player)
    {
        localPlayer = player;
        CameraManager.Instance.Init(localPlayer.cameraLooakTarget, localPlayer.cameraFollowTarget);
        localPlayer.canControl = playerControlEnable;
    }

    private void OnMouseActiveStateChangedEvent(MouseActiveStateChangedEvent arg)
    {
        // 目前只和鼠标是否显示关联
        playerControlEnable = !arg.activeState;
        CameraManager.Instance.SetControlState(playerControlEnable);
        if (localPlayer != null)
        {
            localPlayer.canControl = playerControlEnable;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!ClientUtility.GetWindowActiveState(out UI_GamePopupWindow gamePopupWindow))
            {
                UISystem.Show<UI_GamePopupWindow>();
            }
            else UISystem.Close<UI_GamePopupWindow>();

        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!ClientUtility.GetWindowActiveState(out UI_BagWindow bagWindow))
            {
                OpenBagWindow();
            }
            else UISystem.Close<UI_BagWindow>();
        }
    }

    #region 背包与物品
    private void OpenBagWindow()
    {
        requestOpenBagWindow = true;
        RequestBagData();
    }

    private void RequestBagData()
    {
        // 请求网络
        int dataVersion = bagData == null ? -1 : bagData.dataVersion;
        NetMessageManager.Instance.SendMessageToServer(NetMessageType.C_S_GetBagData, new C_S_GetBagData { dataVersion = dataVersion });
        // 等网络消息回发
    }

    private void OnS_C_GetBagData(ulong serverID, INetworkSerializable serializable)
    {
        S_C_GetBagData message = (S_C_GetBagData)serializable;
        if (message.haveBagData && message.bagData != null)
        {
            this.bagData = message.bagData;
        }
        if (requestOpenBagWindow)
        {
            requestOpenBagWindow = false;
            UISystem.Show<UI_BagWindow>().Show(bagData);
        }
        if (UISystem.GetWindow<UI_ShortcutBarWindow>() == null)
        {
            UISystem.Show<UI_ShortcutBarWindow>().Show(bagData);
        }
    }
    public void UseItem(int slotIndex)
    {
        C_S_BagUseItem message = new C_S_BagUseItem { bagIndex = slotIndex };
        NetMessageManager.Instance.SendMessageToServer(NetMessageType.C_S_BagUseItem, message);
    }
    private void OnS_C_UpdateItem(ulong serverID, INetworkSerializable serializable)
    {
        S_C_BagUpdateItem message = (S_C_BagUpdateItem)serializable;
        // 版本一致不需要考虑
        if (bagData == null || bagData.dataVersion == message.bagDataVersion) return;
        ItemDataBase itemData = message.newItemData;
        bagData.itemList[message.itemIndex] = itemData;
        bagData.dataVersion = message.bagDataVersion;

        if (message.usedWeapon) // 更新武器
        {
            bagData.usedWeaponIndex = message.itemIndex;
        }
        // 如果背包是打开状态则同步给背包
        if (ClientUtility.GetWindowActiveState(out UI_BagWindow bagWindow))
        {
            bagWindow.UpdateItem(message.itemIndex, itemData);
        }
        if (ClientUtility.GetWindowActiveState(out UI_ShortcutBarWindow shortcutBarWindow))
        {
            shortcutBarWindow.UpdateItemByBagIndex(message.itemIndex, itemData);
        }
        if (ClientUtility.GetWindowActiveState(out UI_CraftWindow craftWindow))
        {
            craftWindow.UpdateCraftArea();
        }
    }

    private GameObject GetWeapon(string weapoName)
    {
        GameObject weaponObj = PoolSystem.GetGameObject(weapoName);
        if (weaponObj == null)
        {
            WeaponConfig weaponConfig = ResSystem.LoadAsset<WeaponConfig>(weapoName);
            weaponObj = Instantiate(weaponConfig.prefab);
            weaponObj.name = weapoName;
        }
        return weaponObj;
    }

    private void OnS_C_ShortcutBarUpdateItem(ulong serverID, INetworkSerializable serializable)
    {
        S_C_ShortcutBarUpdateItem message = (S_C_ShortcutBarUpdateItem)serializable;
        if (bagData.dataVersion == message.bagDataVersion) return;
        bagData.dataVersion = message.bagDataVersion;
        // 通知快捷栏
        if (ClientUtility.GetWindowActiveState(out UI_ShortcutBarWindow shortcutBarWindow))
        {
            shortcutBarWindow.SetItem(message.shortcutBarIndex, message.bagIndex, bagData);
        }
    }

    public void OpenShop(string merchatConfigName)
    {
        if (!ClientUtility.GetWindowActiveState<UI_ShopWindow>(out _))
        {
            UISystem.Show<UI_ShopWindow>().Show(ResSystem.LoadAsset<MerchantConfig>(merchatConfigName));
            if (!requestOpenBagWindow && !ClientUtility.GetWindowActiveState(out UI_BagWindow bagWindow))
            {
                OpenBagWindow();
            }
        }
    }

    public void ShopBuyItem(ItemConfigBase targetItemConfig, int bagIndex)
    {
        // 金币是否足够
        if (targetItemConfig.price > bagData.coinCount)
        {
            UISystem.Show<UI_MessagePopupWindow>().ShowMessageByLocalzationKey(ErrorCode.CoinsInsufficient.ToString(), Color.yellow);
            return;
        }

        // 背包空间检测
        ItemDataBase existeItemData = bagData.TryGetItem(targetItemConfig.name, out int existeBagIndex);
        ItemDataBase targetSlotItemData = bagData.itemList[bagIndex];

        bool check;
        // 堆叠物品可以是空位或者是同物品
        if (targetItemConfig.GetDefaultItemData() is StackableItemDataBase)
        {
            check = targetSlotItemData == null || existeItemData != null;
            if (existeItemData != null)
            {
                bagIndex = existeBagIndex;
            }
        }
        // 武器必须在空位
        else
        {
            check = targetSlotItemData == null;
        }
        if (!check)
        {
            UISystem.Show<UI_MessagePopupWindow>().ShowMessageByLocalzationKey(ErrorCode.LackOfBagSpace.ToString(), Color.yellow);
            return;
        }
        // 发送网络消息
        NetMessageManager.Instance.SendMessageToServer(NetMessageType.C_S_ShopBuyItem,
            new C_S_ShopBuyItem { itemID = targetItemConfig.name, bagIndex = bagIndex });
    }

    private void OnS_C_UpdateCoinCount(ulong serverID, INetworkSerializable serializable)
    {
        S_C_UpdateCoinCount message = (S_C_UpdateCoinCount)serializable;
        if (bagData.dataVersion == message.bagDataVersion) return;
        bagData.dataVersion = message.bagDataVersion;
        bagData.coinCount = message.coinCount;
        if (ClientUtility.GetWindowActiveState(out UI_BagWindow bagWindow))
        {
            bagWindow.UpdateCoin(bagData.coinCount);
        }

    }

    public void OpenCraft(string configName)
    {
        if (!ClientUtility.GetWindowActiveState<UI_CraftWindow>(out _))
        {
            UISystem.Show<UI_CraftWindow>().Show(ResSystem.LoadAsset<CrafterConfig>(configName));
            if (!requestOpenBagWindow && !ClientUtility.GetWindowActiveState(out UI_BagWindow bagWindow))
            {
                OpenBagWindow();
            }
        }
    }
    #endregion

    #region 任务

    private void RequesetTaskDatas()
    {
        // 请求网络
        int dataVersion = taskDatas == null ? -1 : taskDatas.dataVersion;
        NetMessageManager.Instance.SendMessageToServer(NetMessageType.C_S_GetTaskDatas, new C_S_GetTaskDatas { dataVersion = dataVersion });
        // 等网络消息回发
    }

    private void OnS_C_GetTaskDatas(ulong serverID, INetworkSerializable serializable)
    {
        S_C_GetTaskDatas message = (S_C_GetTaskDatas)serializable;
        if (message.haveData && message.taskDatas != null)
        {
            this.taskDatas = message.taskDatas;
        }
        if (UISystem.GetWindow<UI_TaskWindow>() == null)
        {
            UISystem.Show<UI_TaskWindow>().Show(taskDatas);
        }
        if (UISystem.GetWindow<UI_TaskGuiderWindow>() == null)
        {
            UI_TaskGuiderWindow guiderWindow = UISystem.Show<UI_TaskGuiderWindow>();
            for (int i = 0; i < taskDatas.tasks.Count; i++)
            {
                TaskData taskData = taskDatas.tasks[i];
                TaskConfig taskConfig = ResSystem.LoadAsset<TaskConfig>(taskData.taskConfigId);
                guiderWindow.AddItem(taskConfig.guidePosition);
            }
        }
    }

    public void CompleteDialogTask(int taskIndex)
    {
        NetMessageManager.Instance.SendMessageToServer(NetMessageType.C_S_CompleteDialogTask, new C_S_CompleteDialogTask { taskIndex = taskIndex });
    }

    private void OnS_C_CompleteTask(ulong serverID, INetworkSerializable serializable)
    {
        S_C_CompleteTask message = (S_C_CompleteTask)serializable;
        if (taskDatas.dataVersion == message.dataVersion) return;
        taskDatas.dataVersion = message.dataVersion;
        TaskData taskData = taskDatas.tasks[message.taskIndex];

        TaskConfig taskConfig = ResSystem.LoadAsset<TaskConfig>(taskData.taskConfigId);
        // 显示奖励
        if (taskConfig.taskReward is CoinTaskReward coinTaskReward)
        {
            string popupMessage = LocalizationSystem.GetContent<LocalizationStringData>("完成任务，获得硬币：", LocalizationSystem.LanguageType).content + coinTaskReward.count;
            UISystem.Show<UI_MessagePopupWindow>().ShowMessage(popupMessage, Color.green);
        }
        taskDatas.tasks.RemoveAt(message.taskIndex);

        if (ClientUtility.GetWindowActiveState(out UI_TaskWindow taskWindow))
        {
            taskWindow.RemoveTask(message.taskIndex);
        }
        if (ClientUtility.GetWindowActiveState(out UI_TaskGuiderWindow guiderWindow))
        {
            guiderWindow.RemoveItem(message.taskIndex);
        }
    }

    private void OnS_C_AddTask(ulong serverID, INetworkSerializable serializable)
    {
        S_C_AddTask message = (S_C_AddTask)serializable;
        if (taskDatas.dataVersion == message.dataVersion) return;
        taskDatas.dataVersion = message.dataVersion;
        taskDatas.tasks.Add(message.taskData);

        if (ClientUtility.GetWindowActiveState(out UI_TaskWindow window))
        {
            window.AddTask(message.taskData);
        }
        if (ClientUtility.GetWindowActiveState(out UI_TaskGuiderWindow guiderWindow))
        {
            TaskData taskData = taskDatas.tasks[taskDatas.tasks.Count - 1];
            TaskConfig taskConfig = ResSystem.LoadAsset<TaskConfig>(taskData.taskConfigId);
            guiderWindow.AddItem(taskConfig.guidePosition);
        }
    }

    private void OnS_C_UpdateTask(ulong serverID, INetworkSerializable serializable)
    {
        S_C_UpdateTask message = (S_C_UpdateTask)serializable;
        if (taskDatas.dataVersion == message.dataVersion) return;
        taskDatas.dataVersion = message.dataVersion;
        taskDatas.tasks[message.taskIndex] = message.taskData;
        if (ClientUtility.GetWindowActiveState(out UI_TaskWindow window))
        {
            window.UpdateTask(message.taskIndex, message.taskData);
        }
        if (ClientUtility.GetWindowActiveState(out UI_TaskGuiderWindow guiderWindow))
        {
            TaskData taskData = taskDatas.tasks[message.taskIndex];
            TaskConfig taskConfig = ResSystem.LoadAsset<TaskConfig>(taskData.taskConfigId);
            guiderWindow.UpdateItem(taskConfig.guidePosition, message.taskIndex);
        }
    }
    #endregion

}

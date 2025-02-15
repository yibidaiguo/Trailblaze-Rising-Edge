using Unity.Netcode;

public enum MessageType : byte
{
    None,

    #region 注册登录
    C_S_Register,
    S_C_Register,
    C_S_Login,
    S_C_Login,
    C_S_EnterGame,
    C_S_Disonnect,
    S_C_Disonnect,
    #endregion

    #region 聊天
    C_S_ChatMessage,
    S_C_ChatMessage,
    #endregion

    #region 背包和物品
    C_S_GetBagData,
    S_C_GetBagData,
    C_S_BagUseItem,
    S_C_BagUpdateItem,
    C_S_BagSwapItem,
    S_C_ShortcutBarUpdateItem,
    C_S_ShortcutBarSetItem,
    C_S_ShortcutBarSwapItem,
    C_S_ShopBuyItem,
    S_C_UpdateCoinCount,
    C_S_BagSellItem,
    C_S_CraftItem,
    #endregion

    #region 任务
    C_S_GetTaskDatas,
    S_C_GetTaskDatas,
    C_S_CompleteDialogTask,
    S_C_CompleteTask,
    S_C_AddTask,
    S_C_UpdateTask,
    C_S_AddTask
    #endregion
}
public enum ErrorCode : byte
{
    None,                // 意味着成功
    AccountFormat,       // 账号格式
    NameDuplication,     // 名称重复
    NameOrPassword,      // 名称或密码错误
    AccountRepeatLogin,  // 名称或密码错误
    CoinsInsufficient,   // 硬币不足
    LackOfBagSpace,      // 缺少背包空间
    UsedWeaponCannotSell,// 使用状态下的武器无法出售
}

#region 注册与登录
public struct C_S_Register : INetworkSerializable
{
    public AccountInfo accountInfo;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        accountInfo.NetworkSerialize(serializer);
    }
}

public struct S_C_Register : INetworkSerializable
{
    public ErrorCode errorCode;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref errorCode);
    }
}

public struct AccountInfo : INetworkSerializable
{
    public string playerName;
    public string password;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref password);
    }
}
public struct C_S_Login : INetworkSerializable
{
    public AccountInfo accountInfo;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        accountInfo.NetworkSerialize(serializer);
    }
}

public struct S_C_Login : INetworkSerializable
{
    public ErrorCode errorCode;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref errorCode);
    }
}

public struct C_S_EnterGame : INetworkSerializable
{
    public ErrorCode errorCode; // 纯占位
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref errorCode);
    }
}

public struct C_S_Disonnect : INetworkSerializable
{
    public ErrorCode errorCode; // 纯占位
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref errorCode);
    }
}


public struct S_C_Disonnect : INetworkSerializable
{
    public ErrorCode errorCode;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref errorCode);
    }
}

#endregion

#region 聊天

public struct C_S_ChatMessage : INetworkSerializable
{
    public string message;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref message);
    }
}

public struct S_C_ChatMessage : INetworkSerializable
{
    public string playerName;
    public string message;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref message);
    }
}


#endregion

#region 背包与物品

public struct C_S_GetBagData : INetworkSerializable
{
    public int dataVersion;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref dataVersion);
    }
}
public struct S_C_GetBagData : INetworkSerializable
{
    public bool haveBagData;
    public BagData bagData;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref haveBagData);

        if (!haveBagData) return;
        if (serializer.IsReader) // 反序列化时如果haveBagData = true，意味着要保存背包数据
        {
            if (bagData == null) bagData = new BagData();
            bagData.NetworkSerialize(serializer);
        }
        else // 对象转数据
        {
            bagData.NetworkSerialize(serializer);
        }
    }
}

public struct C_S_BagUseItem : INetworkSerializable
{
    public int bagIndex;   // 背包中的位置
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref bagIndex);
    }
}

public struct S_C_BagUpdateItem : INetworkSerializable
{
    public int bagDataVersion;
    public int itemIndex;
    public ItemType itemType;
    public bool usedWeapon;
    public ItemDataBase newItemData;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref bagDataVersion);
        serializer.SerializeValue(ref itemIndex);
        serializer.SerializeValue(ref itemType);
        serializer.SerializeValue(ref usedWeapon);
        if (serializer.IsReader) // 反序列化，将数据转为对象，则要求数据不能为null
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                    newItemData = new WeaponData();
                    break;
                case ItemType.Consumable:
                    newItemData = new ConsumableData();
                    break;
                case ItemType.Material:
                    newItemData = new MaterialData();
                    break;
            }
        }
        if (newItemData != null) newItemData.NetworkSerialize(serializer);
    }
}

public struct C_S_BagSwapItem : INetworkSerializable
{
    public int bagIndexA;
    public int bagIndexB;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref bagIndexA);
        serializer.SerializeValue(ref bagIndexB);

    }
}

public struct S_C_ShortcutBarUpdateItem : INetworkSerializable
{
    public int shortcutBarIndex;
    public int bagIndex;
    public int bagDataVersion;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref shortcutBarIndex);
        serializer.SerializeValue(ref bagIndex);
        serializer.SerializeValue(ref bagDataVersion);
    }
}

public struct C_S_ShortcutBarSetItem : INetworkSerializable
{
    public int shortcutBarIndex;
    public int bagIndex;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref shortcutBarIndex);
        serializer.SerializeValue(ref bagIndex);
    }
}

public struct C_S_ShortcutBarSwapItem : INetworkSerializable
{
    public int shortcutBarIndexA;
    public int shortcutBarIndexB;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref shortcutBarIndexA);
        serializer.SerializeValue(ref shortcutBarIndexB);
    }
}

public struct C_S_ShopBuyItem : INetworkSerializable
{
    public string itemID;
    public int bagIndex;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref itemID);
        serializer.SerializeValue(ref bagIndex);
    }
}

public struct S_C_UpdateCoinCount : INetworkSerializable
{
    public int bagDataVersion;
    public int coinCount;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref bagDataVersion);
        serializer.SerializeValue(ref coinCount);
    }
}

public struct C_S_BagSellItem : INetworkSerializable
{
    public int bagIndex;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref bagIndex);
    }
}

public struct C_S_CraftItem : INetworkSerializable
{
    public string targetItemName;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref targetItemName);
    }
}


#endregion

#region 任务

public struct C_S_GetTaskDatas : INetworkSerializable
{
    public int dataVersion;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref dataVersion);
    }
}
public struct S_C_GetTaskDatas : INetworkSerializable
{
    public bool haveData;
    public TaskDatas taskDatas;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref haveData);

        if (!haveData) return;
        if (serializer.IsReader) // 反序列化时 haveBag = true，意味着要保存背包数据
        {
            if (taskDatas == null) taskDatas = new TaskDatas();
            taskDatas.NetworkSerialize(serializer);
        }
        else // 对象转数据
        {
            taskDatas.NetworkSerialize(serializer);
        }
    }
}

public struct C_S_CompleteDialogTask : INetworkSerializable
{
    public int taskIndex;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref taskIndex);
    }
}
public struct S_C_CompleteTask : INetworkSerializable
{
    public int taskIndex;
    public int dataVersion;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref taskIndex);
        serializer.SerializeValue(ref dataVersion);
    }
}

public struct S_C_AddTask : INetworkSerializable
{
    public int dataVersion;
    public TaskData taskData;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref dataVersion);
        if (serializer.IsReader)
        {
            taskData = new TaskData();
        }
        serializer.SerializeValue(ref taskData);
    }
}

public struct S_C_UpdateTask : INetworkSerializable
{
    public int dataVersion;
    public int taskIndex;
    public TaskData taskData;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref dataVersion);
        serializer.SerializeValue(ref taskIndex);
        if (serializer.IsReader)
        {
            taskData = new TaskData();
        }
        serializer.SerializeValue(ref taskData);
    }
}

public struct C_S_AddTask : INetworkSerializable
{
    public string taskID;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref taskID);
    }
}
#endregion

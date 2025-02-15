using JKFrame;
using Unity.Netcode;
using UnityEngine;

// 负责聊天系统的部分
public partial class ClientsManager : SingletonMono<ClientsManager>
{
    public void InitItemSystem()
    {
        PlayerController.SetGetWeaponFunc(GetWeapon);
        NetMessageManager.Instance.RegisterMessageCallback(MessageType.C_S_GetBagData, OnClientGetBagData);
        NetMessageManager.Instance.RegisterMessageCallback(MessageType.C_S_BagUseItem, OnClientBagUseItem);
        NetMessageManager.Instance.RegisterMessageCallback(MessageType.C_S_BagSwapItem, OnClientBagSwapItem);
        NetMessageManager.Instance.RegisterMessageCallback(MessageType.C_S_ShortcutBarSetItem, OnClientShortcutBarSetItem);
        NetMessageManager.Instance.RegisterMessageCallback(MessageType.C_S_ShortcutBarSwapItem, OnClientShortcutBarSwapItem);
        NetMessageManager.Instance.RegisterMessageCallback(MessageType.C_S_ShopBuyItem, OnClientShopBuyItem);
        NetMessageManager.Instance.RegisterMessageCallback(MessageType.C_S_BagSellItem, OnClientBagSellItem);
        NetMessageManager.Instance.RegisterMessageCallback(MessageType.C_S_CraftItem, OnClientCraftItem);

    }

    // 当客户端请求背包数据
    private void OnClientGetBagData(ulong clientID, INetworkSerializable serializable)
    {
        if (clientIDDic.TryGetValue(clientID, out Client client) && client.playerData != null)
        {
            C_S_GetBagData message = (C_S_GetBagData)serializable;
            S_C_GetBagData result = new S_C_GetBagData
            {
                haveBagData = client.playerData.bagData.dataVersion != message.dataVersion
            };
            if (result.haveBagData) result.bagData = client.playerData.bagData;
            NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_GetBagData, result, clientID);
        }
    }
    // 当客户端请求使用物品
    private void OnClientBagUseItem(ulong clientID, INetworkSerializable serializable)
    {
        if (clientIDDic.TryGetValue(clientID, out Client client) && client.playerData != null)
        {
            C_S_BagUseItem message = (C_S_BagUseItem)serializable;
            BagData bagData = client.playerData.bagData;
            if (!bagData.CheckBagIndexRange(message.bagIndex)) return;
            ItemDataBase originalData = bagData.itemList[message.bagIndex];
            if (originalData == null) return;
            ItemType originalType = GlobalUtility.GetItemType(originalData);

            ItemDataBase newItemData = bagData.TryUseItem(message.bagIndex);
            ItemType newItemType = GlobalUtility.GetItemType(newItemData);
            S_C_BagUpdateItem result = new S_C_BagUpdateItem
            {
                itemIndex = message.bagIndex,
                bagDataVersion = bagData.dataVersion,
                newItemData = newItemData,
                itemType = newItemType,
                usedWeapon = newItemType == ItemType.Weapon
            };
            NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_BagUpdateItem, result, clientID);

            if (originalType == ItemType.Weapon) // 更新角色的实际武器
            {
                client.playerController.mainController.UpdateWeaponNetVar(newItemData.id);
            }
            else if (originalType == ItemType.Consumable) // 吃药
            {
                ConsumableConfig consumableConfig = ServerResSystem.GetItemConfig<ConsumableConfig>(originalData.id);
                client.playerController.AddHp(consumableConfig.HPRegeneration);
            }
            CheckAllCollectItemTask(client);
        }
    }

    private GameObject GetWeapon(string weapoName)
    {
        GameObject weaponObj = PoolSystem.GetGameObject(weapoName);
        if (weaponObj == null)
        {
            WeaponConfig weaponConfig = ServerResSystem.GetItemConfig<WeaponConfig>(weapoName);
            weaponObj = Instantiate(weaponConfig.prefab);
            weaponObj.name = weapoName;
        }
        return weaponObj;
    }

    // 当客户端互换背包中的物品
    private void OnClientBagSwapItem(ulong clientID, INetworkSerializable serializable)
    {
        if (clientIDDic.TryGetValue(clientID, out Client client) && client.playerData != null)
        {
            C_S_BagSwapItem message = (C_S_BagSwapItem)serializable;
            BagData bagData = client.playerData.bagData;

            if (!bagData.CheckBagIndexRange(message.bagIndexA) || !bagData.CheckBagIndexRange(message.bagIndexB)) return;

            // 交换数据
            bagData.SwapItem(message.bagIndexA, message.bagIndexB);

            ItemDataBase itemAData = bagData.itemList[message.bagIndexA];
            ItemDataBase itemBData = bagData.itemList[message.bagIndexB];
            ItemType itemAType = itemAData == null ? ItemType.Empty : itemAData.GetItemType();
            ItemType itemBType = itemBData == null ? ItemType.Empty : itemBData.GetItemType();

            S_C_BagUpdateItem resultA = new S_C_BagUpdateItem
            {
                itemIndex = message.bagIndexA,
                bagDataVersion = bagData.dataVersion,
                newItemData = itemAData,
                itemType = itemAType,
                usedWeapon = bagData.usedWeaponIndex == message.bagIndexA,
            };
            bagData.AddDataVersion(); // 避免第二条消息因为版本号一直被客户端过滤
            S_C_BagUpdateItem resultB = new S_C_BagUpdateItem
            {
                itemIndex = message.bagIndexB,
                bagDataVersion = bagData.dataVersion,
                newItemData = itemBData,
                itemType = itemBType,
                usedWeapon = bagData.usedWeaponIndex == message.bagIndexB,
            };
            NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_BagUpdateItem, resultA, clientID);
            NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_BagUpdateItem, resultB, clientID);

            // 也许会涉及到快捷键的修改，也就是A或者B原本是一个快捷键
            if (bagData.TryGetShortcutBarIndex(message.bagIndexA, out int shortcutAIndex))
            {
                bagData.AddDataVersion();
                S_C_ShortcutBarUpdateItem shortcutBarUpdateItem = new S_C_ShortcutBarUpdateItem
                {
                    shortcutBarIndex = shortcutAIndex,
                    bagIndex = message.bagIndexB,
                    bagDataVersion = bagData.dataVersion
                };
                NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_ShortcutBarUpdateItem, shortcutBarUpdateItem, clientID);
            }
            if (bagData.TryGetShortcutBarIndex(message.bagIndexB, out int shortcutBIndex))
            {
                bagData.AddDataVersion();
                S_C_ShortcutBarUpdateItem shortcutBarUpdateItem = new S_C_ShortcutBarUpdateItem
                {
                    shortcutBarIndex = shortcutBIndex,
                    bagIndex = message.bagIndexA,
                    bagDataVersion = bagData.dataVersion
                };
                NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_ShortcutBarUpdateItem, shortcutBarUpdateItem, clientID);
            }
            if (shortcutAIndex != -1)
            {
                bagData.UpdateShortcutBarItem(shortcutAIndex, message.bagIndexB);
            }
            if (shortcutBIndex != -1)
            {
                bagData.UpdateShortcutBarItem(shortcutBIndex, message.bagIndexA);
            }
        }
    }

    // 当客户端设置物品快捷键
    private void OnClientShortcutBarSetItem(ulong clientID, INetworkSerializable serializable)
    {
        if (clientIDDic.TryGetValue(clientID, out Client client) && client.playerData != null)
        {
            C_S_ShortcutBarSetItem message = (C_S_ShortcutBarSetItem)serializable;
            BagData bagData = client.playerData.bagData;

            message.bagIndex = bagData.CheckBagIndexRange(message.bagIndex) ? message.bagIndex : -1;
            if (!bagData.CheckShortcutBarIndexRange(message.shortcutBarIndex)) return;

            // 找到当前快捷栏中可能存在的重复项，将其移出
            if (bagData.TryGetShortcutBarIndex(message.bagIndex, out int shortcutBarIndex))
            {
                bagData.RemoveShortcutBarItem(shortcutBarIndex);
                bagData.AddDataVersion();
                S_C_ShortcutBarUpdateItem result1 = new S_C_ShortcutBarUpdateItem
                {
                    shortcutBarIndex = shortcutBarIndex,
                    bagIndex = -1,
                    bagDataVersion = bagData.dataVersion
                };
                NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_ShortcutBarUpdateItem, result1, clientID);
            }
            // 设置修改后的快捷键
            bagData.UpdateShortcutBarItem(message.shortcutBarIndex, message.bagIndex);
            bagData.AddDataVersion();
            S_C_ShortcutBarUpdateItem result2 = new S_C_ShortcutBarUpdateItem
            {
                shortcutBarIndex = message.shortcutBarIndex,
                bagIndex = message.bagIndex,
                bagDataVersion = bagData.dataVersion
            };
            NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_ShortcutBarUpdateItem, result2, clientID);
        }
    }

    // 当客户端交换快捷键
    private void OnClientShortcutBarSwapItem(ulong clientID, INetworkSerializable serializable)
    {
        if (clientIDDic.TryGetValue(clientID, out Client client) && client.playerData != null)
        {
            C_S_ShortcutBarSwapItem message = (C_S_ShortcutBarSwapItem)serializable;
            BagData bagData = client.playerData.bagData;
            if (!bagData.CheckShortcutBarIndexRange(message.shortcutBarIndexA) || !bagData.CheckShortcutBarIndexRange(message.shortcutBarIndexB)) return;
            bagData.SwapShortcutBarItem(message.shortcutBarIndexA, message.shortcutBarIndexB);
            bagData.AddDataVersion();
            S_C_ShortcutBarUpdateItem result1 = new S_C_ShortcutBarUpdateItem
            {
                shortcutBarIndex = message.shortcutBarIndexA,
                bagIndex = bagData.shortcutBarIndes[message.shortcutBarIndexA],
                bagDataVersion = bagData.dataVersion
            };
            NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_ShortcutBarUpdateItem, result1, clientID);

            bagData.AddDataVersion();
            S_C_ShortcutBarUpdateItem result2 = new S_C_ShortcutBarUpdateItem
            {
                shortcutBarIndex = message.shortcutBarIndexB,
                bagIndex = bagData.shortcutBarIndes[message.shortcutBarIndexB],
                bagDataVersion = bagData.dataVersion
            };
            NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_ShortcutBarUpdateItem, result2, clientID);
        }
    }

    // 当客户端从商店购买物品
    private void OnClientShopBuyItem(ulong clientID, INetworkSerializable serializable)
    {
        if (clientIDDic.TryGetValue(clientID, out Client client) && client.playerData != null)
        {
            C_S_ShopBuyItem message = (C_S_ShopBuyItem)serializable;
            BagData bagData = client.playerData.bagData;
            if (!bagData.CheckBagIndexRange(message.bagIndex)) return;

            // 物品不存在的判断
            ItemConfigBase itemConfig = ServerResSystem.GetItemConfig<ItemConfigBase>(message.itemID);
            if (itemConfig == null) return;
            // 金币检查
            if (bagData.coinCount < itemConfig.price) return;

            if (bagData.TryAddItem(itemConfig, 1, message.bagIndex))
            {
                bagData.AddDataVersion();
                // 回复客户端增加物品
                NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_BagUpdateItem,
                    new S_C_BagUpdateItem
                    {
                        itemIndex = message.bagIndex,
                        bagDataVersion = bagData.dataVersion,
                        newItemData = bagData.itemList[message.bagIndex],
                        itemType = GlobalUtility.GetItemType(bagData.itemList[message.bagIndex]),
                        usedWeapon = false
                    }, clientID);

                // 回复客户端金币更新
                bagData.coinCount -= itemConfig.price;
                bagData.AddDataVersion();
                NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_UpdateCoinCount,
                    new S_C_UpdateCoinCount
                    {
                        bagDataVersion = bagData.dataVersion,
                        coinCount = bagData.coinCount,
                    }, clientID);
                CheckAllCollectItemTask(client);
            }
        }
    }

    // 当客户端从背包出售物品
    public void OnClientBagSellItem(ulong clientID, INetworkSerializable serializable)
    {
        if (clientIDDic.TryGetValue(clientID, out Client client) && client.playerData != null)
        {
            C_S_BagSellItem message = (C_S_BagSellItem)serializable;
            BagData bagData = client.playerData.bagData;
            // index范围有效，物品不是空格子，不是当前使用的武器
            if (!bagData.CheckBagIndexRange(message.bagIndex) || message.bagIndex == bagData.usedWeaponIndex) return;
            ItemDataBase itemData = bagData.itemList[message.bagIndex];
            if (itemData == null) return;

            ItemConfigBase itemConfig = ServerResSystem.GetItemConfig<ItemConfigBase>(bagData.itemList[message.bagIndex].id);
            // 销毁物品
            bagData.RemoveItem(message.bagIndex);
            bagData.AddDataVersion();
            NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_BagUpdateItem,
                new S_C_BagUpdateItem
                {
                    itemIndex = message.bagIndex,
                    bagDataVersion = bagData.dataVersion,
                    newItemData = null,
                    itemType = ItemType.Empty,
                    usedWeapon = false
                }, clientID);

            // 增加金币
            int itemCount = 1;
            if (itemData is StackableItemDataBase)
            {
                itemCount = ((StackableItemDataBase)itemData).count;
            }
            bagData.coinCount += (itemConfig.price / 2) * itemCount;
            bagData.AddDataVersion();
            NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_UpdateCoinCount,
                new S_C_UpdateCoinCount
                {
                    bagDataVersion = bagData.dataVersion,
                    coinCount = bagData.coinCount,
                }, clientID);
            CheckAllCollectItemTask(client);
        }
    }

    // 当客户端合成物品
    public void OnClientCraftItem(ulong clientID, INetworkSerializable serializable)
    {
        if (clientIDDic.TryGetValue(clientID, out Client client) && client.playerData != null)
        {
            C_S_CraftItem message = (C_S_CraftItem)serializable;
            ItemConfigBase targetItemConfig = ServerResSystem.GetItemConfig<ItemConfigBase>(message.targetItemName);
            if (targetItemConfig == null) return;
            BagData bagData = client.playerData.bagData;
            if (bagData.CheckCraft(targetItemConfig, out bool containUsedWeapon))
            {
                int updateItemIndex = -1; // 最终更新物品的位置
                if (containUsedWeapon) // 合成中需要的材料涉及到当前使用武器的，必须目标物品也是武器，进行替换
                {
                    if (targetItemConfig is WeaponConfig)
                    {
                        updateItemIndex = bagData.usedWeaponIndex;
                    }
                }
                else bagData.TryAddItem(targetItemConfig, 1, out updateItemIndex);// 尝试添加

                if (updateItemIndex != -1)
                {
                    // 移除全部用来合成的物品
                    foreach (System.Collections.Generic.KeyValuePair<string, int> item in targetItemConfig.carftConfig.itemDic)
                    {
                        ItemDataBase itemData = bagData.TryGetItem(item.Key, out int itemIndex);
                        // 如果涉及到当前武器的，之前已经处理过了
                        if (containUsedWeapon && itemIndex == bagData.usedWeaponIndex) continue;
                        bagData.RemoveItem(itemIndex, item.Value);
                        bagData.AddDataVersion();
                        NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_BagUpdateItem, new S_C_BagUpdateItem
                        {
                            itemIndex = itemIndex,
                            bagDataVersion = bagData.dataVersion,
                            newItemData = bagData.itemList[itemIndex],
                            itemType = GlobalUtility.GetItemType(bagData.itemList[itemIndex]),
                            usedWeapon = false

                        }, clientID);
                    }

                    if (containUsedWeapon)
                    {
                        // 覆盖掉之前的武器
                        bagData.itemList[updateItemIndex] = targetItemConfig.GetDefaultItemData().Copy();
                        client.playerController.mainController.UpdateWeaponNetVar(targetItemConfig.name);
                    }

                    bagData.AddDataVersion();
                    NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_BagUpdateItem, new S_C_BagUpdateItem
                    {
                        itemIndex = updateItemIndex,
                        bagDataVersion = bagData.dataVersion,
                        newItemData = bagData.itemList[updateItemIndex],
                        itemType = GlobalUtility.GetItemType(bagData.itemList[updateItemIndex]),
                        usedWeapon = containUsedWeapon
                    }, clientID);
                    CheckAllCollectItemTask(client);
                }
            }
        }
    }
}
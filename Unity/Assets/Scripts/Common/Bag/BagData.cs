using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BagData : INetworkSerializable
{
    // 背包的格子是有固定的数量上限，空格子的表现为itemList[i] = null
    public const int itemCount = 30;
    [BsonIgnore] // 避免保存到数据库
    public int dataVersion;
    public List<ItemDataBase> itemList = new List<ItemDataBase>(itemCount);
    public int usedWeaponIndex; // 正在使用的武器格子索引
    public int[] shortcutBarIndes = new int[GlobalUtility.itemShortcutBarCount];
    public int coinCount;
    public BagData()
    {
        for (int i = 0; i < itemCount; i++)
        {
            itemList.Add(null);
        }

        for (int i = 0; i < GlobalUtility.itemShortcutBarCount; i++)
        {
            shortcutBarIndes[i] = -1; // -1代表默认是空格子
        }
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref usedWeaponIndex);
        serializer.SerializeValue(ref coinCount);
        serializer.SerializeValue(ref shortcutBarIndes);
        for (int i = 0; i < itemCount; i++)
        {
            if (serializer.IsReader) // 反序列化,数据转为对象
            {
                FastBufferReader reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out ItemType itemType);
                switch (itemType)
                {
                    case ItemType.Empty:
                        itemList[i] = null;
                        break;
                    case ItemType.Weapon:
                        WeaponData weaponData = new WeaponData();
                        weaponData.NetworkSerialize(serializer);
                        itemList[i] = weaponData;
                        break;
                    case ItemType.Consumable:
                        ConsumableData consumableData = new ConsumableData();
                        consumableData.NetworkSerialize(serializer);
                        itemList[i] = consumableData;
                        break;
                    case ItemType.Material:
                        MaterialData materialData = new MaterialData();
                        materialData.NetworkSerialize(serializer);
                        itemList[i] = materialData;
                        break;
                }
            }
            else // 序列化,将对象转为数据
            {
                FastBufferWriter writer = serializer.GetFastBufferWriter();
                ItemDataBase itemData = itemList[i];
                if (itemData == null)
                {
                    writer.WriteValueSafe(ItemType.Empty);
                }
                else
                {
                    if (itemData is WeaponData) writer.WriteValueSafe(ItemType.Weapon);
                    else if (itemData is ConsumableData) writer.WriteValueSafe(ItemType.Consumable);
                    else if (itemData is MaterialData) writer.WriteValueSafe(ItemType.Material);
                    itemData.NetworkSerialize(serializer);
                }
            }
        }
    }


    public bool CheckBagIndexRange(int index)
    {
        return index >= 0 && index < itemCount;
    }

    public bool CheckShortcutBarIndexRange(int index)
    {
        return index >= 0 && index < shortcutBarIndes.Length;
    }

    public ItemDataBase TryUseItem(int index)
    {
        ItemDataBase itemData = itemList[index];
        // 只有武器与消耗品才可以使用
        if (itemData is WeaponData)
        {
            AddDataVersion();
            usedWeaponIndex = index;
            return itemData;
        }
        else if (itemData is ConsumableData)
        {
            AddDataVersion();
            ConsumableData consumableData = (ConsumableData)itemData;
            consumableData.count -= 1;
            if (consumableData.count <= 0) // 物品全部使用完毕
            {
                itemList[index] = null;
                itemData = null;
            }
            return itemData;
        }
        return null;
    }



    public ItemDataBase TryGetItem(string id, out int index)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            ItemDataBase itemData = itemList[i];
            if (itemData != null && itemData.id == id)
            {
                index = i;
                return itemData;
            }
        }
        index = -1;
        return null;
    }

    public bool TryGetFirstEmptyIndex(out int index)
    {
        index = -1;
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] == null)
            {
                index = i;
                return true;
            }
        }
        index = -1;
        return false;
    }

    public void SwapItem(int itemIndexA, int itemIndexB)
    {
        if (usedWeaponIndex == itemIndexA) usedWeaponIndex = itemIndexB;
        else if (usedWeaponIndex == itemIndexB) usedWeaponIndex = itemIndexA;

        ItemDataBase temp = itemList[itemIndexA];
        itemList[itemIndexA] = itemList[itemIndexB];
        itemList[itemIndexB] = temp;
        AddDataVersion();
    }

    public void AddDataVersion()
    {
        dataVersion += 1;
    }

    public bool TryGetShortcutBarIndex(int bagIndex, out int shortCutBarIndex)
    {
        for (int i = 0; i < shortcutBarIndes.Length; i++)
        {
            if (shortcutBarIndes[i] == bagIndex)
            {
                shortCutBarIndex = i;
                return true;
            }
        }
        shortCutBarIndex = -1;
        return false;
    }

    public void UpdateShortcutBarItem(int shortcutIndex, int bagIndex)
    {
        shortcutBarIndes[shortcutIndex] = bagIndex;
    }

    public void RemoveShortcutBarItem(int shortcutIndex)
    {
        UpdateShortcutBarItem(shortcutIndex, -1);
    }

    public void SwapShortcutBarItem(int shortcutBarIndexA, int shortcutBarIndexB)
    {
        int temp = shortcutBarIndes[shortcutBarIndexA];
        shortcutBarIndes[shortcutBarIndexA] = shortcutBarIndes[shortcutBarIndexB];
        shortcutBarIndes[shortcutBarIndexB] = temp;
    }

    // 尝试添加物品，指定位置
    public bool TryAddItem(ItemConfigBase targetItemConfig, int stackableCount, int targetIndex)
    {
        bool isStackableItemData = targetItemConfig.GetDefaultItemData() is StackableItemDataBase;
        if (isStackableItemData)
        {
            if (itemList[targetIndex] == null) // 空位
            {
                StackableItemDataBase newData = (StackableItemDataBase)targetItemConfig.GetDefaultItemData().Copy();
                newData.count = 1;
                itemList[targetIndex] = newData;
                return true;
            }
            else if (itemList[targetIndex].id == targetItemConfig.name)
            {
                ((StackableItemDataBase)itemList[targetIndex]).count += 1;
                return true;
            }
        }
        else
        {
            if (itemList[targetIndex] == null)
            {
                itemList[targetIndex] = targetItemConfig.GetDefaultItemData().Copy();
                return true;
            }
        }
        return false;
    }

    // 尝试添加物品，不指定位置
    public bool TryAddItem(ItemConfigBase targetItemConfig, int stackableCount, out int itemIndex)
    {
        itemIndex = -1;
        bool isStackableItemData = targetItemConfig.GetDefaultItemData() is StackableItemDataBase;
        if (isStackableItemData)
        {
            StackableItemDataBase existedItemData = TryGetItem(targetItemConfig.name, out itemIndex) as StackableItemDataBase;
            if (existedItemData != null) // 堆叠
            {
                existedItemData.count += 1;
                return true;
            }
            // 需要空位
            else if (TryGetFirstEmptyIndex(out itemIndex))
            {
                StackableItemDataBase newData = (StackableItemDataBase)targetItemConfig.GetDefaultItemData().Copy();
                newData.count = 1;
                itemList[itemIndex] = newData;
                return true;
            }
        }
        else if (TryGetFirstEmptyIndex(out itemIndex))
        {
            itemList[itemIndex] = targetItemConfig.GetDefaultItemData().Copy();
            return true;
        }
        return false;
    }

    // 检查合成
    public bool CheckCraft(ItemConfigBase targetItem, out bool containUsedWeapon)
    {
        containUsedWeapon = false;
        foreach (KeyValuePair<string, int> item in targetItem.carftConfig.itemDic)
        {
            ItemDataBase itemData = TryGetItem(item.Key, out int itemIndex);
            if (itemData == null) return false;
            if (itemIndex == usedWeaponIndex) containUsedWeapon = true;
            if (itemData is StackableItemDataBase)
            {
                int curr = ((StackableItemDataBase)itemData).count;
                if (curr < item.Value) return false;
            }
        }
        return true;
    }

    // 移除一个格子的物品
    public void RemoveItem(int index)
    {
        itemList[index] = null;
    }

    // 按照数量移除一个格子中的物品
    public void RemoveItem(int itemIndex, int count)
    {
        ItemDataBase itemData = itemList[itemIndex];
        if (itemData == null) return;
        StackableItemDataBase stackableItemData = itemData as StackableItemDataBase;
        if (stackableItemData != null)
        {
            stackableItemData.count -= count;
            if (stackableItemData.count == 0) RemoveItem(itemIndex);
        }
        else RemoveItem(itemIndex);
    }

    // 检查能否添加物品
    public bool CheckAddItem(ItemConfigBase targetItemConfig)
    {
        bool isStackableItemData = targetItemConfig.GetDefaultItemData() is StackableItemDataBase;
        if (isStackableItemData)
        {
            StackableItemDataBase existedItemData = TryGetItem(targetItemConfig.name, out int itemIndex) as StackableItemDataBase;
            return existedItemData != null || TryGetFirstEmptyIndex(out itemIndex);
        }
        else if (TryGetFirstEmptyIndex(out int itemIndex))
        {
            return true;
        }
        return false;
    }
}

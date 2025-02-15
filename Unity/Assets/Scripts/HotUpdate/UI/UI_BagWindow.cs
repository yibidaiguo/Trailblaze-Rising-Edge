using JKFrame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_BagWindow : UI_CustomWindowBase, IItemWindow
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform itemRoot;
    [SerializeField] private Text coinText;
    private string emptySlotPath => ClientUtility.emptySlotPath;
    private List<UI_SlotBase> slotList = new List<UI_SlotBase>();
    private BagData bagData;
    private int usedWeaponIndex;
    public override void Init()
    {
        closeButton.onClick.AddListener(CloseButtonClick);
    }
    private void CloseButtonClick()
    {
        UISystem.Close<UI_BagWindow>();
    }
    public override void OnClose()
    {
        base.OnClose();
        for (int i = 0; i < slotList.Count; i++)
        {
            slotList[i].Destroy();
        }
        slotList.Clear();
        bagData = null;
    }
    public void Show(BagData bagData)
    {
        this.bagData = bagData;
        UpdateCoin(bagData.coinCount);
        for (int i = 0; i < bagData.itemList.Count; i++)
        {
            ItemDataBase itemData = bagData.itemList[i];
            if (itemData != null) slotList.Add(CreateItemSlot(i, itemData));
            else slotList.Add(CreateEmptySlot(i));
        }
        usedWeaponIndex = bagData.usedWeaponIndex;
        UI_WeaponSlot weaponSlot = (UI_WeaponSlot)slotList[usedWeaponIndex];
        weaponSlot.SetUseState(true);
    }

    private UI_SlotBase CreateItemSlot(int index, ItemDataBase itemData)
    {
        ItemConfigBase config = ResSystem.LoadAsset<ItemConfigBase>(itemData.id);
        UI_SlotBase slot = ResSystem.InstantiateGameObject<UI_SlotBase>(config.slotPrefabPath, itemRoot);
        slot.Init(this, itemData, config, index, OnUseItem, OnInteriorDragItem);
        return slot;
    }

    private UI_SlotBase CreateEmptySlot(int index)
    {
        UI_SlotBase slot = ResSystem.InstantiateGameObject<UI_SlotBase>(emptySlotPath, itemRoot);
        slot.Init(this, null, null, index);
        return slot;
    }

    private void OnUseItem(int slotIndex)
    {
        PlayerManager.Instance.UseItem(slotIndex);
    }


    public void UpdateCoin(int value)
    {
        coinText.text = value.ToString();
    }

    public void UpdateItem(int index, ItemDataBase itemData)
    {
        slotList[index].Destroy();  // 回收掉格子
        UI_SlotBase newSlot;
        if (itemData != null) newSlot = CreateItemSlot(index, itemData);
        else newSlot = CreateEmptySlot(index);
        newSlot.transform.SetSiblingIndex(index);
        slotList[index] = newSlot;
        if (index == bagData.usedWeaponIndex) // 武器格子发生了变化
        {
            if (usedWeaponIndex != index)
            {
                UI_WeaponSlot oldWeaponSlot = slotList[usedWeaponIndex] as UI_WeaponSlot;
                if (oldWeaponSlot != null) oldWeaponSlot.SetUseState(false);
            }

            UI_WeaponSlot newWeaponSlot = (UI_WeaponSlot)slotList[index];
            newWeaponSlot.SetUseState(true);
            usedWeaponIndex = index;
        }
    }

    // A一定来自自身的，B不一定
    private void OnInteriorDragItem(UI_SlotBase slotA, UI_SlotBase slotB)
    {
        //  内部交换
        if (slotB.ownerWindow == this)
        {
            NetMessageManager.Instance.SendMessageToServer(MessageType.C_S_BagSwapItem, new C_S_BagSwapItem
            {
                bagIndexA = slotA.dataIndex,
                bagIndexB = slotB.dataIndex,
            });
        }
        // 设置这个格子到快捷栏
        else if (slotB.ownerWindow is UI_ShortcutBarWindow)
        {
            int shortcutBarIndex = UISystem.GetWindow<UI_ShortcutBarWindow>().GetItemIndex(slotB);
            if (shortcutBarIndex != -1)
            {
                NetMessageManager.Instance.SendMessageToServer(MessageType.C_S_ShortcutBarSetItem, new C_S_ShortcutBarSetItem
                {
                    shortcutBarIndex = shortcutBarIndex,
                    bagIndex = slotA.dataIndex,
                });
            }
        }
        // 出售这个物品
        else if (slotB.ownerWindow is UI_ShopWindow)
        {
            if (slotA.dataIndex == PlayerManager.Instance.bagData.usedWeaponIndex)
            {
                UISystem.Show<UI_MessagePopupWindow>().ShowMessageByLocalzationKey(ErrorCode.UsedWeaponCannotSell.ToString(), Color.yellow);
                return;
            }
            NetMessageManager.Instance.SendMessageToServer(MessageType.C_S_BagSellItem, new C_S_BagSellItem
            {
                bagIndex = slotA.dataIndex,
            });
        }
    }
}

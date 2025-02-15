using JKFrame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UI_NPCInteractionMenuWindow : UI_CustomWindowBase
{
    [SerializeField] private Transform itemRoot;
    private List<UI_NPCInteractionMenuWindowItem> itemList = new List<UI_NPCInteractionMenuWindowItem>();

    public override void OnClose()
    {
        base.OnClose();
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].Destroy();
        }
        itemList.Clear();
    }
    public void AddOption(string optionKey, UnityAction onSelected)
    {
        UI_NPCInteractionMenuWindowItem item = CreateItem();
        item.Init(optionKey, onSelected);
        itemList.Add(item);
    }
    private UI_NPCInteractionMenuWindowItem CreateItem()
    {
        return ResSystem.InstantiateGameObject<UI_NPCInteractionMenuWindowItem>(nameof(UI_NPCInteractionMenuWindowItem), itemRoot);
    }
}

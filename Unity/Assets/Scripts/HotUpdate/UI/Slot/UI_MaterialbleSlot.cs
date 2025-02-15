using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_MaterialbleSlot : UI_SlotBase<MaterialData, MaterialConfig>
{
    [SerializeField] private Text countText;
    public override void OnInit()
    {
        base.OnInit();
        SetCount();
    }

    public void SetCount()
    {
        SetCount(itemData.count.ToString(), Color.white);
    }
    public override void SetCount(string countString, Color color)
    {
        countText.text = countString;
        countText.color = color;
    }
}

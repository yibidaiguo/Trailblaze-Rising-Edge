using JKFrame;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemConfigBase : ConfigBase
{
    public int price;
    public ItemCarftConfig carftConfig;
    protected ItemDataBase defaultData;
    public abstract ItemDataBase GetDefaultItemData();

    public string slotPrefabPath;
    public Sprite icon;
    public Dictionary<LanguageType, string> nameDic;
    public Dictionary<LanguageType, string> descriptionDic;
    public string GetName(LanguageType languageType)
    {
        return nameDic[languageType];
    }
    public virtual string GetDescription(LanguageType languageType)
    {
        return descriptionDic[languageType];
    }
    public abstract string GetType(LanguageType languageType);
}

public class ItemCarftConfig
{
    public Dictionary<string, int> itemDic = new Dictionary<string, int>();
}
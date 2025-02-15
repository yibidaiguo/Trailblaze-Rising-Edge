using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ItemConfigImproter
{

    [MenuItem("Project/导入物品表格", priority = 1)]
    public static void Improt()
    {
        string excelPath = Application.dataPath + "/Config/Excel/物品配置.xlsx";
        FileInfo fileInfo = new FileInfo(excelPath);
        using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
        {
            for (int i = 1; i <= 3; i++) // 1:武器 2:消耗品 3:材料
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[i];
                int maxCol = worksheet.Cells.Columns; // 不能完全相信，有可能是空行
                for (int x = 2; x < maxCol; x++) // 第一行是表头
                {
                    string key = worksheet.Cells[x, 1].Text.Trim();
                    if (string.IsNullOrEmpty(key)) break; // 下一张表
                    string chineseName = worksheet.Cells[x, 2].Text.Trim();
                    string englishName = worksheet.Cells[x, 3].Text.Trim();
                    string chineseDescription = worksheet.Cells[x, 4].Text.Trim();
                    string englishDescription = worksheet.Cells[x, 5].Text.Trim();
                    int price = int.Parse(worksheet.Cells[x, 6].Text.Trim());
                    // 合成
                    string carftString = worksheet.Cells[x, 7].Text.Trim();
                    string[] carftSplitStrings = carftString.Split(',');
                    ItemCarftConfig itemCarftConfig = new ItemCarftConfig();
                    for (int s = 0; s < carftSplitStrings.Length - 1; s += 2)
                    {
                        string name = carftSplitStrings[s];
                        int count = int.Parse(carftSplitStrings[s + 1]);
                        itemCarftConfig.itemDic.Add(name, count);
                    }

                    if (i == 1) // 武器
                    {
                        float attackValue = float.Parse(worksheet.Cells[x, 8].Text.Trim());
                        string configPath = $"Assets/Config/Item/Weapon/{key}.asset";
                        string iconPath = $"Assets/Res/Icon/Weapon/{key}.png";
                        string prefab = $"Assets/Prefab/Weapon/{key}.prefab";
                        string slotPrefabPath = "UI_WeaponSlot";
                        WeaponConfig itemConfig = AssetDatabase.LoadAssetAtPath<WeaponConfig>(configPath);
                        bool isCreate = itemConfig == null;
                        if (isCreate) itemConfig = WeaponConfig.CreateInstance<WeaponConfig>();
                        SetConfigCommon(itemConfig, chineseName, englishName, chineseDescription, englishDescription, iconPath, slotPrefabPath, price, itemCarftConfig);
                        itemConfig.attackValue = attackValue;
                        itemConfig.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefab);
                        EditorUtility.SetDirty(itemConfig);
                        if (isCreate) AssetDatabase.CreateAsset(itemConfig, configPath);
                        else AssetDatabase.SaveAssetIfDirty(itemConfig);
                    }
                    else if (i == 2) // 消耗品
                    {
                        float HPRegeneration = float.Parse(worksheet.Cells[x, 8].Text.Trim());
                        string configPath = $"Assets/Config/Item/Consumable/{key}.asset";
                        string iconPath = $"Assets/Res/Icon/Consumable/{key}.png";
                        string slotPrefabPath = "UI_ConsumableSlot";
                        int defaultCountOnShop = int.Parse(worksheet.Cells[x, 9].Text.Trim());

                        ConsumableConfig itemConfig = AssetDatabase.LoadAssetAtPath<ConsumableConfig>(configPath);
                        bool isCreate = itemConfig == null;
                        if (isCreate) itemConfig = ConsumableConfig.CreateInstance<ConsumableConfig>();
                        SetConfigCommon(itemConfig, chineseName, englishName, chineseDescription, englishDescription, iconPath, slotPrefabPath, price, itemCarftConfig);
                        itemConfig.HPRegeneration = HPRegeneration;
                        itemConfig.defaultCountInShop = defaultCountOnShop;
                        EditorUtility.SetDirty(itemConfig);
                        if (isCreate) AssetDatabase.CreateAsset(itemConfig, configPath);
                        else AssetDatabase.SaveAssetIfDirty(itemConfig);
                    }
                    else if (i == 3) // 材料
                    {
                        string configPath = $"Assets/Config/Item/Material/{key}.asset";
                        string iconPath = $"Assets/Res/Icon/Material/{key}.png";
                        string slotPrefabPath = "UI_MaterialSlot";
                        int defaultCountOnShop = int.Parse(worksheet.Cells[x, 8].Text.Trim());

                        MaterialConfig itemConfig = AssetDatabase.LoadAssetAtPath<MaterialConfig>(configPath);
                        bool isCreate = itemConfig == null;
                        if (isCreate) itemConfig = MaterialConfig.CreateInstance<MaterialConfig>();
                        SetConfigCommon(itemConfig, chineseName, englishName, chineseDescription, englishDescription, iconPath, slotPrefabPath, price, itemCarftConfig);
                        itemConfig.defaultCountInShop = defaultCountOnShop;
                        EditorUtility.SetDirty(itemConfig);
                        if (isCreate) AssetDatabase.CreateAsset(itemConfig, configPath);
                        else AssetDatabase.SaveAssetIfDirty(itemConfig);
                    }
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private static void SetConfigCommon(ItemConfigBase itemConfig, string chineseName, string englishName, string chineseDescription, string englishDescription, string iconPath, string slotPrefabPath, int price, ItemCarftConfig itemCarftConfig)
    {
        itemConfig.carftConfig = itemCarftConfig;
        itemConfig.nameDic = new Dictionary<LanguageType, string>()
        {
            { LanguageType.SimplifiedChinese,chineseName},
            { LanguageType.English,englishName},
        };
        itemConfig.descriptionDic = new Dictionary<LanguageType, string>()
        {
            { LanguageType.SimplifiedChinese,chineseDescription},
            { LanguageType.English,englishDescription},
        };
        itemConfig.icon = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
        if (string.IsNullOrEmpty(itemConfig.slotPrefabPath)) // 有可能已经设置过专属预制体则忽视
        {
            itemConfig.slotPrefabPath = slotPrefabPath;
        }
        itemConfig.price = price;
    }
}

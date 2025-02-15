using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class MonsterConfigImproter
{
    [MenuItem("Project/导入怪物表格", priority = 2)]
    public static void Improt()
    {
        string excelPath = Application.dataPath + "/Config/Excel/怪物配置.xlsx";
        FileInfo fileInfo = new FileInfo(excelPath);
        using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[1];
            int maxCol = worksheet.Cells.Columns; // 不能完全相信，有可能是空行
            for (int x = 2; x < maxCol; x++) // 第一行是表头
            {
                string key = worksheet.Cells[x, 1].Text.Trim();
                if (string.IsNullOrEmpty(key)) break; // 下一张表
                string configPath = $"Assets/Config/Monster/{key}.asset";
                MonsterConfig monsterConfig = AssetDatabase.LoadAssetAtPath<MonsterConfig>(configPath);
                bool isCreate = monsterConfig == null;
                if (isCreate) monsterConfig = MonsterConfig.CreateInstance<MonsterConfig>();
                monsterConfig.nameDic = new Dictionary<LanguageType, string>
                {
                    { LanguageType.SimplifiedChinese,worksheet.Cells[x,2].Text.Trim() },
                    { LanguageType.English,worksheet.Cells[x,3].Text.Trim() },
                };
                monsterConfig.maxHP = float.Parse(worksheet.Cells[x, 4].Text.Trim());
                monsterConfig.attackValue = float.Parse(worksheet.Cells[x, 5].Text.Trim());
                monsterConfig.maxIdleTime = float.Parse(worksheet.Cells[x, 6].Text.Trim());
                monsterConfig.maxPatrolTime = float.Parse(worksheet.Cells[x, 7].Text.Trim());
                monsterConfig.searchPlayerRange = float.Parse(worksheet.Cells[x, 8].Text.Trim());
                monsterConfig.pursuitTime = float.Parse(worksheet.Cells[x, 9].Text.Trim());
                monsterConfig.audioGroupIndex = int.Parse(worksheet.Cells[x, 10].Text.Trim());
                monsterConfig.attackRange = float.Parse(worksheet.Cells[x, 11].Text.Trim());
                monsterConfig.attackCD = float.Parse(worksheet.Cells[x, 12].Text.Trim());
                EditorUtility.SetDirty(monsterConfig);
                if (isCreate) AssetDatabase.CreateAsset(monsterConfig, configPath);
                else AssetDatabase.SaveAssetIfDirty(monsterConfig);
            }
        }
    }
}

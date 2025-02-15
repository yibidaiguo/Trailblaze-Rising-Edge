using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DialogConfigImporter
{
    private static string soDirPath = "Assets/Config/Dialog";
    private static string excelDirPath = "Assets/Config/Excel/对话";

    [MenuItem("Project/导入对话", priority = 3)]
    public static void ImprotAll()
    {
        // 遍历全部的excel文件
        string[] filePaths = Directory.GetFiles(excelDirPath, "*.xlsx", SearchOption.AllDirectories);
        foreach (string filePath in filePaths)
        {
            // 过滤临时文件
            if (filePath.Contains("~$")) continue;
            string fullPath = $"{Application.dataPath.Replace("/Assets", "")}/{filePath}";
            ImprotExcel(fullPath);
        }
        AssetDatabase.Refresh();
    }

    private static void ImprotExcel(string excelPath)
    {
        FileInfo fileInfo = new FileInfo(excelPath);
        string configPath = $"{soDirPath}/{Path.GetFileNameWithoutExtension(fileInfo.Name)}.asset";
        DialogConfig dialogConfig = AssetDatabase.LoadAssetAtPath<DialogConfig>(configPath);
        bool create = dialogConfig == null;
        if (create) dialogConfig = ScriptableObject.CreateInstance<DialogConfig>();
        else dialogConfig.stepList.Clear(); ;
        using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[1];
            int maxCol = worksheet.Cells.Columns; // 不能完全相信，有可能是空行
            for (int x = 2; x < maxCol; x++) // 第一行是表头
            {
                string key = worksheet.Cells[x, 1].Text.Trim();
                if (string.IsNullOrEmpty(key)) break;
                DialogStepConfig step = new DialogStepConfig();
                step.player = Convert.ToBoolean(worksheet.Cells[x, 1].Value);
                step.contentDic = new Dictionary<LanguageType, string>
                {
                    { LanguageType.SimplifiedChinese, worksheet.Cells[x, 2].Text.Trim()},
                    { LanguageType.English, worksheet.Cells[x, 3].Text.Trim()},
                };
                dialogConfig.stepList.Add(step);
            }
        }
        if (create) AssetDatabase.CreateAsset(dialogConfig, configPath);
        else
        {
            EditorUtility.SetDirty(dialogConfig);
            AssetDatabase.SaveAssetIfDirty(dialogConfig);
        }
    }
}

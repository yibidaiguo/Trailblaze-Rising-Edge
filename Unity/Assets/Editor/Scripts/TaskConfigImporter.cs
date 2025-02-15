using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
public static class TaskConfigImporter
{
    private static string soDirPath = "Assets/Config/Task";
    private static string excelFilePath = "Assets/Config/Excel/任务.xlsx";
    private static Dictionary<string, Type> allTaskInfoTypeDic;
    private static Dictionary<string, Type> allTaskRewardTypeDic;

    [MenuItem("Project/导入任务", priority = 4)]
    public static void Improt()
    {
        allTaskInfoTypeDic = FindTypes(typeof(TaskInfoBase));
        allTaskRewardTypeDic = FindTypes(typeof(TaskRewardBase));

        FileInfo fileInfo = new FileInfo(excelFilePath);
        using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[1];
            int maxCol = worksheet.Cells.Columns; // 不能完全相信，有可能是空行
            for (int x = 2; x < maxCol; x++) // 第一行是表头
            {
                string key = worksheet.Cells[x, 1].Text.Trim();
                if (string.IsNullOrEmpty(key)) break;
                string configPath = $"{soDirPath}/{key}.asset";
                TaskConfig taskConfig = AssetDatabase.LoadAssetAtPath<TaskConfig>(configPath);
                bool create = taskConfig == null;
                if (create) taskConfig = ScriptableObject.CreateInstance<TaskConfig>();
                taskConfig.nameDic = new Dictionary<LanguageType, string>
                {
                    { LanguageType.SimplifiedChinese, worksheet.Cells[x, 2].Text.Trim()},
                    { LanguageType.English, worksheet.Cells[x, 3].Text.Trim()},
                };
                taskConfig.descriptionDic = new Dictionary<LanguageType, string>
                {
                    { LanguageType.SimplifiedChinese, worksheet.Cells[x, 4].Text.Trim()},
                    { LanguageType.English, worksheet.Cells[x, 5].Text.Trim()},
                };
                taskConfig.nextTaskId = worksheet.Cells[x, 6].Text.Trim();

                taskConfig.taskInfo = ConverTaskInfo(worksheet.Cells[x, 7].Text.Trim());
                taskConfig.taskReward = ConverTaskReward(worksheet.Cells[x, 8].Text.Trim());
                taskConfig.guidePosition = ConverVector3(worksheet.Cells[x, 9].Text.Trim());

                if (create) AssetDatabase.CreateAsset(taskConfig, configPath);
                else
                {
                    EditorUtility.SetDirty(taskConfig);
                    AssetDatabase.SaveAssetIfDirty(taskConfig);
                }
            }
        }
    }

    private static TaskInfoBase ConverTaskInfo(string cellString)
    {
        if (string.IsNullOrEmpty(cellString)) return null;
        string[] infoString = cellString.Split(':');
        if (infoString.Length != 2) Debug.LogError($"任务信息格式不符:{infoString}");
        string typeString = infoString[0];
        string valueString = infoString[1];
        if (allTaskInfoTypeDic.TryGetValue($"{typeString}TaskInfo", out Type type))
        {
            TaskInfoBase taskInfo = (TaskInfoBase)Activator.CreateInstance(type);
            taskInfo.ConverFromString(valueString);
            return taskInfo;
        }
        else
        {
            Debug.LogError($"不能存:{infoString}TaskInfo");
            return null;
        }
    }

    private static TaskRewardBase ConverTaskReward(string cellString)
    {
        if (string.IsNullOrEmpty(cellString)) return null;
        string[] rewardString = cellString.Split(':');
        if (rewardString.Length != 2) Debug.LogError($"奖励格式不符:{rewardString}");
        string typeString = rewardString[0];
        string valueString = rewardString[1];
        if (allTaskRewardTypeDic.TryGetValue($"{typeString}TaskReward", out Type type))
        {
            TaskRewardBase taskReward = (TaskRewardBase)Activator.CreateInstance(type);
            taskReward.ConverFromString(valueString);
            return taskReward;
        }
        else
        {
            Debug.LogError($"不能存:{rewardString}TaskReward");
            return null;
        }
    }

    private static Vector3 ConverVector3(string cellString)
    {
        if (string.IsNullOrEmpty(cellString)) return Vector3.zero;
        string[] valueString = cellString.Split(',');
        if (valueString.Length != 3)
        {
            Debug.LogError($"Vector3格式不符:{valueString}");
            return Vector3.zero;
        }
        float x = float.Parse(valueString[0]);
        float y = float.Parse(valueString[1]);
        float z = float.Parse(valueString[2]);
        return new Vector3(x, y, z);
    }

    private static Dictionary<string, Type> FindTypes(Type baseType)
    {
        Dictionary<string, Type> dic = new Dictionary<string, Type>();
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            Type[] types = assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract).ToArray();
            foreach (Type t in types)
            {
                dic.Add(t.Name, t);
            }
        }
        return dic;
    }

}

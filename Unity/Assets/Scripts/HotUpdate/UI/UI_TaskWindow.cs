using JKFrame;
using System.Collections.Generic;
using UnityEngine;

public class UI_TaskWindow : UI_WindowBase
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform itemRoot;

    private List<UI_TaskWindowItem> itemList = new List<UI_TaskWindowItem>();
    public void Show(TaskDatas taskDatas)
    {
        for (int i = 0; i < taskDatas.tasks.Count; i++)
        {
            UI_TaskWindowItem item = CreateItem();
            TaskData taskData = taskDatas.tasks[i];
            TaskConfig taskConfig = ResSystem.LoadAsset<TaskConfig>(taskData.taskConfigId);
            item.Init(taskConfig, taskData);
            itemList.Add(item);
        }
    }

    public void UpdateTask(int taskIndex, TaskData taskData)
    {
        UI_TaskWindowItem item = itemList[taskIndex];
        TaskConfig taskConfig = ResSystem.LoadAsset<TaskConfig>(taskData.taskConfigId);
        item.Set(taskConfig, taskData);
    }

    public void RemoveTask(int taskIndex)
    {
        GameObject.DestroyImmediate(itemList[taskIndex].gameObject);
        itemList.RemoveAt(taskIndex);
    }

    public void AddTask(TaskData taskData)
    {
        UI_TaskWindowItem item = CreateItem();
        TaskConfig taskConfig = ResSystem.LoadAsset<TaskConfig>(taskData.taskConfigId);
        item.Init(taskConfig, taskData);
        itemList.Add(item);
    }

    private UI_TaskWindowItem CreateItem()
    {
        return GameObject.Instantiate(itemPrefab, itemRoot).GetComponent<UI_TaskWindowItem>();
    }


}

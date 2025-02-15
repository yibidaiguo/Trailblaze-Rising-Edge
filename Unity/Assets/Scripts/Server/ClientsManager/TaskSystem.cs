using JKFrame;
using System;
using Unity.Netcode;

// 负责任务系统的部分
public partial class ClientsManager : SingletonMono<ClientsManager>
{
    public void InitTaskSystem()
    {
        NetMessageManager.Instance.RegisterMessageCallback(MessageType.C_S_GetTaskDatas, OnClientGetTaskDatas);
        NetMessageManager.Instance.RegisterMessageCallback(MessageType.C_S_CompleteDialogTask, OnClientCompleteDialogTask);
        NetMessageManager.Instance.RegisterMessageCallback(MessageType.C_S_AddTask, OnClientAddTask);
    }


    private void OnClientGetTaskDatas(ulong clientID, INetworkSerializable serializable)
    {
        if (clientIDDic.TryGetValue(clientID, out Client client) && client.playerData != null)
        {
            C_S_GetTaskDatas message = (C_S_GetTaskDatas)serializable;
            S_C_GetTaskDatas result = new S_C_GetTaskDatas
            {
                haveData = client.playerData.taskDatas.dataVersion != message.dataVersion
            };
            if (result.haveData) result.taskDatas = client.playerData.taskDatas;
            NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_GetTaskDatas, result, clientID);
        }
    }

    private void OnClientCompleteDialogTask(ulong clientID, INetworkSerializable serializable)
    {
        if (clientIDDic.TryGetValue(clientID, out Client client) && client.playerData != null)
        {
            C_S_CompleteDialogTask message = (C_S_CompleteDialogTask)serializable;
            TaskDatas taskDatas = client.playerData.taskDatas;
            if (message.taskIndex >= 0 && taskDatas.tasks.Count > message.taskIndex) // 有效性
            {
                TaskData taskData = taskDatas.tasks[message.taskIndex];
                // 有这个任务，并且是对话任务
                if (ServerGlobal.Instance.ServerConfig.taskConfigDic.TryGetValue(taskData.taskConfigId, out TaskConfig taskConfig)
                    && taskConfig.taskInfo is DialogTaskInfo dialogTaskInfo)
                {
                    // 完成任务
                    ComleteTask(taskDatas, client, taskConfig, message.taskIndex);
                }
            }
        }
    }

    // 如果当前任务具备下一个任务，应该自动接下下一个任务
    private void CheckTask(Client client, string nextTaskId, TaskDatas taskDatas)
    {
        if (string.IsNullOrEmpty(nextTaskId)) return;
        TaskConfig taskConfig = ServerGlobal.Instance.ServerConfig.taskConfigDic[nextTaskId];
        // 如果下一个任务是收集物品则需要考虑背包的当前进度，如果已经满足则直接下发奖励
        // 这里不考虑装备
        int taskProgress = 0;
        if (taskConfig.taskInfo is CollectItemTaskInfo collectItemTaskInfo)
        {
            BagData bagData = client.playerData.bagData;
            StackableItemDataBase itemData = bagData.TryGetItem(collectItemTaskInfo.targetItemId, out int itemIndex) as StackableItemDataBase;
            if (itemData != null)
            {
                if (CheckCollectItemTask(client, collectItemTaskInfo, taskConfig, itemData, itemIndex))
                {
                    return;
                }
                else
                {
                    taskProgress = itemData.count;
                }
            }
        }

        TaskData newTaskData = new TaskData { taskConfigId = nextTaskId, taskProgress = taskProgress };
        taskDatas.tasks.Add(newTaskData);
        taskDatas.AddDataVersion();
        NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_AddTask, new S_C_AddTask { dataVersion = taskDatas.dataVersion, taskData = newTaskData }, client.clientID);
    }


    private void ComleteTask(TaskDatas taskDatas, Client client, TaskConfig taskConfig, int taskIndex = 1) // taskIndex为-1则表示其是一个不存在的任务
    {
        if (taskIndex != -1)
        {
            taskDatas.AddDataVersion();
            taskDatas.tasks.RemoveAt(taskIndex);
            NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_CompleteTask, new S_C_CompleteTask { taskIndex = taskIndex, dataVersion = taskDatas.dataVersion }, client.clientID);
        }

        IssuseTaskReward(taskConfig, client);
        CheckTask(client, taskConfig.nextTaskId, taskDatas);
    }

    private void IssuseTaskReward(TaskConfig taskConfig, Client client)
    {
        // 奖励
        if (taskConfig.taskReward is CoinTaskReward coinTaskReward)
        {
            BagData bagData = client.playerData.bagData;
            bagData.coinCount += coinTaskReward.count;
            bagData.AddDataVersion();
            NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_UpdateCoinCount, new S_C_UpdateCoinCount { coinCount = bagData.coinCount, bagDataVersion = bagData.dataVersion }, client.clientID);
        }
    }

    private bool CheckCollectItemTask(Client client, CollectItemTaskInfo collectItemTaskInfo, TaskConfig taskConfig, StackableItemDataBase itemData, int itemDataIndex, int taskIndex = -1)
    {
        BagData bagData = client.playerData.bagData;
        TaskDatas taskDatas = client.playerData.taskDatas;
        if (itemData.count >= collectItemTaskInfo.count) // 足够完成
        {
            itemData.count -= collectItemTaskInfo.count;
            bagData.AddDataVersion();
            if (itemData.count == 0) // 任务完成并且物品消耗完了
            {
                bagData.RemoveItem(itemDataIndex);
                itemData = null; // 让下方发送网络消息时为null
            }

            NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_BagUpdateItem,
                new S_C_BagUpdateItem
                {
                    itemIndex = itemDataIndex,
                    bagDataVersion = bagData.dataVersion,
                    newItemData = itemData,
                    itemType = GlobalUtility.GetItemType(itemData),
                    usedWeapon = false
                }, client.clientID);

            ComleteTask(taskDatas, client, taskConfig, taskIndex);
            return true;
        }
        return false;
    }

    private void CheckAllCollectItemTask(Client client)
    {
        TaskDatas taskDatas = client.playerData.taskDatas;
        BagData bagData = client.playerData.bagData;
        for (int i = taskDatas.tasks.Count - 1; i >= 0; i--)
        {
            TaskData taskData = taskDatas.tasks[i];
            TaskConfig taskConfig = ServerGlobal.Instance.ServerConfig.taskConfigDic[taskData.taskConfigId];
            if (taskConfig.taskInfo is CollectItemTaskInfo collectItemTaskInfo)
            {
                StackableItemDataBase itemData = bagData.TryGetItem(collectItemTaskInfo.targetItemId, out int itemDataIndex) as StackableItemDataBase;
                if (itemData != null)
                {
                    // 任务没完成则对比进度变化
                    if (!CheckCollectItemTask(client, collectItemTaskInfo, taskConfig, itemData, itemDataIndex, i)
                        && itemData.count != taskData.taskProgress)
                    {
                        taskData.taskProgress = itemData.count;
                        taskDatas.AddDataVersion();
                        NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_UpdateTask, new S_C_UpdateTask { dataVersion = taskDatas.dataVersion, taskData = taskData, taskIndex = i }, client.clientID);
                    }
                }
                else // 现在没有这个物品，但是当前任务的进度可能不是0
                {
                    if (taskData.taskProgress != 0)
                    {
                        taskData.taskProgress = 0;
                        taskDatas.AddDataVersion();
                        NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_UpdateTask, new S_C_UpdateTask { dataVersion = taskDatas.dataVersion, taskData = taskData, taskIndex = i }, client.clientID);
                    }
                }
            }
        }
    }

    public void CheckAndAddStruckDownTaskProgress(ulong clientID, string monsterId)
    {
        if (clientIDDic.TryGetValue(clientID, out Client client) && client.playerData != null)
        {
            TaskDatas taskDatas = client.playerData.taskDatas;
            for (int i = taskDatas.tasks.Count - 1; i >= 0; i--)
            {
                TaskData taskData = taskDatas.tasks[i];
                TaskConfig taskConfig = ServerGlobal.Instance.ServerConfig.taskConfigDic[taskData.taskConfigId];
                if (taskConfig.taskInfo is StruckDownTaskInfo struckDownTaskInfo && monsterId.Contains(struckDownTaskInfo.targetMonsterKeyword))
                {
                    taskData.taskProgress += 1;
                    if (taskData.taskProgress >= struckDownTaskInfo.count) // 完成任务
                    {
                        ComleteTask(taskDatas, client, taskConfig, i);
                    }
                    else // 更新任务进度
                    {
                        taskDatas.AddDataVersion();
                        NetMessageManager.Instance.SendMessageToClient(MessageType.S_C_UpdateTask, new S_C_UpdateTask { dataVersion = taskDatas.dataVersion, taskData = taskData, taskIndex = i }, clientID);
                    }
                }
            }

        }
    }


    private void OnClientAddTask(ulong clientID, INetworkSerializable serializable)
    {
        if (clientIDDic.TryGetValue(clientID, out Client client) && client.playerData != null)
        {
            C_S_AddTask message = (C_S_AddTask)serializable;
            TaskDatas taskDatas = client.playerData.taskDatas;
            if (taskDatas.Contain(message.taskID)) return; // 不能重复
            CheckTask(client, message.taskID, taskDatas);
        }
    }
}

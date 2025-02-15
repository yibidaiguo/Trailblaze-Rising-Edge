using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using Unity.Netcode;

public class TaskDatas : INetworkSerializable
{
    [BsonIgnore] // 避免保存到数据库中
    public int dataVersion;
    public List<TaskData> tasks = new List<TaskData>();
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        int count = tasks.Count;
        serializer.SerializeValue(ref count);
        for (int i = 0; i < count; i++)
        {
            if (serializer.IsReader) // 反序列化,数据转为对象
            {
                TaskData taskData = new TaskData();
                serializer.SerializeValue(ref taskData);
                tasks.Add(taskData);
            }
            else // 序列化,将对象转为数据
            {
                TaskData taskData = tasks[i];
                serializer.SerializeValue(ref taskData);
            }
        }
    }

    public void AddDataVersion()
    {
        dataVersion += 1;
    }

    public bool Contain(string taskID)
    {
        foreach (var item in tasks)
        {
            if (item.taskConfigId == taskID)
            {
                return true;
            }
        }
        return false;
    }
}

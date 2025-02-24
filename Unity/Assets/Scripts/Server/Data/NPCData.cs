
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

public class CrafterData
{
    [BsonId] 
    public string playerName;
    public string currentTask;
    public int currentDailogIndex;
    public Dictionary<string, Message> aiMessagesDic  { get; set; } = new ();
    public int currentAiDailogIndex;
}

[System.Serializable]
public class Message
{
    public string role; // 角色（system/user/assistant）
    public string content; // 消息内容
}



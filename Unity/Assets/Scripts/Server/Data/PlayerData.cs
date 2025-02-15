using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;
public class PlayerData
{
    [BsonId]
    public string name;
    public string password;
    public CharacterData characterData = new CharacterData();
    public BagData bagData = new BagData();
    public TaskDatas taskDatas = new TaskDatas();
}

public class CharacterData
{
    public Vector3 position;
    public float rotation_Y;
    public string usedWeaponName;
    public float hp;
}

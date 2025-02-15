using JKFrame;
using MongoDB.Driver;
using UnityEngine;
public class DatabaseManager : SingletonMono<DatabaseManager>
{
    [SerializeField] private string connstr = "mongodb://localhost:27017";
    private MongoClient mongoClient;
    private IMongoDatabase mmoDatabase;
    private IMongoCollection<PlayerData> playerDataCollection;
    public void Init()
    {
        // 链接MongoDB
        mongoClient = new MongoClient(connstr);
        // 查找或建立Database,没有会自动创建
        mmoDatabase = mongoClient.GetDatabase("MMONetCode");
        //  查找或建立集合,没有会自动创建
        playerDataCollection = mmoDatabase.GetCollection<PlayerData>("PlayerData");
    }

    public PlayerData GetPlayerData(string playerName)
    {
        PlayerData playerData = playerDataCollection.Find(Builders<PlayerData>.Filter.Eq(nameof(PlayerData.name), playerName)).FirstOrDefault();
        return playerData;
    }

    public void CreatePlayerData(PlayerData playerData)
    {
        playerDataCollection.InsertOne(playerData);
    }

    public void SavePlayerData(PlayerData newPlayerData)
    {
        playerDataCollection.ReplaceOne(Builders<PlayerData>.Filter.Eq(nameof(PlayerData.name), newPlayerData.name), newPlayerData);
    }
}

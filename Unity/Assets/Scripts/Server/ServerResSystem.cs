using UnityEngine;

public static class ServerResSystem
{
    public static ServerConfig serverConfig;
    static ServerResSystem()
    {
        serverConfig = ServerGlobal.Instance.ServerConfig;
    }
    public static NetManager InstantiateNetworkManager()
    {
        GameObject prefab = serverConfig.NetworkManagerPrefab;
        GameObject instance = GameObject.Instantiate(prefab);
        return instance.GetComponent<NetManager>();
    }
    public static GameObject InstantiateServerOnGameScene()
    {
        GameObject prefab = serverConfig.ServerOnGameScenePrefab;
        GameObject instance = GameObject.Instantiate(prefab);
        return instance;
    }
    public static GameObject InstantiateTerrain(string resKey, Transform parent, Vector3 postion)
    {
        GameObject prefab = serverConfig.terrainDic[resKey];
        GameObject instance = GameObject.Instantiate(prefab, postion, Quaternion.identity, parent);
        //instance.GetComponent<Terrain>().enabled = false;
        return instance;
    }

    public static T GetItemConfig<T>(string itemName) where T : ItemConfigBase
    {
        if (serverConfig.itemConfigDic.TryGetValue(itemName, out ItemConfigBase itemConfig))
        {
            return (T)itemConfig;

        }
        return default;
    }
}
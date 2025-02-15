using JKFrame;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.IO;
using Amazon.Runtime.Internal.Transform;

#if UNITY_EDITOR
using UnityEditor;
#endif
[CreateAssetMenu(menuName = "Config/Server/ServerConfig")]
public class ServerConfig : ConfigBase
{
    [Header("通用")]
    public GameObject NetworkManagerPrefab;
    public GameObject ServerOnGameScenePrefab;
    [Header("玩家")]
    public GameObject playerPrefab;
    public Vector3 playerDefaultPostion;
    public int playerDefaultCointCount;
    public float rootMotionMoveSpeedMultiply;
    public float playerAriMoveSpeed;
    public float playerGravity;
    public float playerRotateSpeed;
    public float playerJumpHeightMultiply;
    public float playerMaxHp;
    public float playerBeHitTime;
    public float playerDieTime;
    public float playerRespawnHp;
    [Header("地形")]
    public Dictionary<string, GameObject> terrainDic;
    [Header("物品配置")]
    public Dictionary<string, ItemConfigBase> itemConfigDic;
    [Header("任务配置")]
    public Dictionary<string, TaskConfig> taskConfigDic;
#if UNITY_EDITOR
    [FolderPath] public string terrainFolderPath;
    [Button]
    public void SetTerrainDic()
    {
        if (terrainDic == null) terrainDic = new Dictionary<string, GameObject>();
        terrainDic.Clear();
        string[] files = Directory.GetFiles(terrainFolderPath, "*.prefab");
        for (int i = 0; i < files.Length; i++)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(files[i]);
            terrainDic.Add(prefab.name, prefab);
        }
    }

    [Button]
    public void TestTerrainDic(MapConfig mapConfig) // 方便低配置服务器测试
    {
        int width = (int)(mapConfig.mapSize.x / mapConfig.terrainSize);
        int height = (int)(mapConfig.mapSize.y / mapConfig.terrainSize);
        int testRange = 36;
        Dictionary<string, GameObject> newTerrainDic = new Dictionary<string, GameObject>();
        for (int x = testRange / 2; x < width - testRange / 2; x++)
        {
            for (int y = testRange / 2; y < height - testRange / 2; y++)
            {
                string resyKey = $"{x}_{y}";
                newTerrainDic.Add(resyKey, terrainDic[resyKey]);
            }
        }
        terrainDic = newTerrainDic;
    }

    [FolderPath] public string weaponConfigFolderPath;
    [FolderPath] public string consumableConfigFolderPath;
    [FolderPath] public string materialConfigFolderPath;
    [Button]
    public void SetItemConfigDic()
    {
        if (itemConfigDic == null) itemConfigDic = new Dictionary<string, ItemConfigBase>();
        itemConfigDic.Clear();
        FindItems(weaponConfigFolderPath);
        FindItems(consumableConfigFolderPath);
        FindItems(materialConfigFolderPath);
    }

    private void FindItems(string path)
    {
        string[] files = Directory.GetFiles(path);// 包含*.meta
        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].EndsWith(".meta"))
            {
                ItemConfigBase itemCofnig = AssetDatabase.LoadAssetAtPath<ItemConfigBase>(files[i]);
                itemConfigDic.Add(itemCofnig.name, itemCofnig);
            }
        }
    }


#endif
}
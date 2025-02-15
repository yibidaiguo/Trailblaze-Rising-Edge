using JKFrame;
using Unity.AI.Navigation;
using UnityEngine;

public class ServerMapManager : SingletonMono<ServerMapManager>
{
    [SerializeField] private MapConfig mapConfig;
    [SerializeField] private NavMeshSurface navMeshSurface;
    private MonsterSpawner[] monsterSpawners;
    public void Init()
    {
        // 一次性加载全部地图
        int width = (int)(mapConfig.mapSize.x / mapConfig.terrainSize);
        int height = (int)(mapConfig.mapSize.y / mapConfig.terrainSize);

        int testRange = 36;
        for (int x = testRange / 2; x < width - testRange / 2; x++)
        {
            for (int y = testRange / 2; y < height - testRange / 2; y++)
            {
                Vector2Int resCoord = new Vector2Int(x, y);
                string resyKey = $"{resCoord.x}_{resCoord.y}";
                Vector2Int terrainCoord = resCoord - mapConfig.terrainResKeyCoordOffset;
                Vector3 pos = new Vector3(terrainCoord.x * mapConfig.terrainSize, 0, terrainCoord.y * mapConfig.terrainSize);
                ServerResSystem.InstantiateTerrain(resyKey, transform, pos);
            }
        }
        navMeshSurface.BuildNavMesh();
        monsterSpawners = GetComponentsInChildren<MonsterSpawner>();
        for (int i = 0; i < monsterSpawners.Length; i++)
        {
            monsterSpawners[i].Init();
        }
    }
}

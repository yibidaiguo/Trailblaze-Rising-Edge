using JKFrame;
using System.Collections.Generic;
using UnityEngine;
public class ClientMapManager : SingletonMono<ClientMapManager>
{
    public enum TerrainState
    {
        Request, Enable, Disable
    }

    public class TerrainController
    {
        public Terrain terrain;
        public TerrainState state;
        public float destroyTimer;

        public void Enable()
        {
            if (state != TerrainState.Enable)
            {
                destroyTimer = 0;
                state = TerrainState.Enable;
                if (terrain != null)
                {
                    terrain.gameObject.SetActive(true);
                }
            }
        }
        public void Disable()
        {
            if (state != TerrainState.Disable)
            {
                state = TerrainState.Disable;
                if (terrain != null)
                {
                    terrain.gameObject.SetActive(false);
                }
            }
        }

        public void Load(Vector2Int coord)
        {
            Vector2Int resCoord = coord + Instance.mapConfig.terrainResKeyCoordOffset;
            string resyKey = $"{resCoord.x}_{resCoord.y}";
            state = TerrainState.Request;
            ResSystem.InstantiateGameObjectAsync<Terrain>(resyKey, (terrain) =>
            {
                this.terrain = terrain;
                terrain.basemapDistance = 100;
                terrain.heightmapPixelError = 50;
                terrain.heightmapMaximumLOD = 1;
                terrain.detailObjectDensity = 0.9f;
                terrain.treeDistance = 10;
                terrain.treeCrossFadeLength = 10;
                terrain.treeMaximumFullLODCount = 10;
                terrain.transform.position = new Vector3(coord.x * Instance.mapConfig.terrainSize, 0, coord.y * Instance.mapConfig.terrainSize);
                if (state == TerrainState.Disable)
                {
                    terrain.gameObject.SetActive(false);
                }
            }, Instance.transform, null, false);
        }

        public bool CheckAndDestroy()
        {
            if (state == TerrainState.Disable)
            {
                destroyTimer += Time.deltaTime;
                if (destroyTimer >= Instance.destroyTerrainTime)
                {
                    Destroy();
                    return true;
                }
            }
            return false;
        }


        public void Destroy()
        {
            if (terrain != null)
            {
                ResSystem.UnloadInstance(terrain.gameObject);
            }

            destroyTimer = 0;
            terrain = null;
            this.ObjectPushPool();
        }
    }

    [SerializeField] private MapConfig mapConfig;
    [SerializeField] private new Camera camera;
    public MapConfig MapConfig { get => mapConfig; }
    public float destroyTerrainTime;
    private QuadTree quadTree;
    private Dictionary<Vector2Int, TerrainController> terrainControllDic = new Dictionary<Vector2Int, TerrainController>(300);
    private List<Vector2Int> destroyTerrainCoords = new List<Vector2Int>(100);
    private Plane[] cameraPlanes = new Plane[6];
    private Vector2Int playerTerrainCoord;
    public void Init()
    {
        quadTree = new QuadTree(mapConfig, EnableTerrain, DisableTerrain, CheckVisibility);
    }

    private void Update()
    {
        if (camera == null || quadTree == null) return;
        GeometryUtility.CalculateFrustumPlanes(camera, cameraPlanes);
        quadTree.CheckVisibility();
        playerTerrainCoord = GetTerrainCoordByWolrdPos(camera.transform.position);
        EnableTerrain(playerTerrainCoord);
        foreach (KeyValuePair<Vector2Int, TerrainController> item in terrainControllDic)
        {
            if (item.Value.CheckAndDestroy())
            {
                destroyTerrainCoords.Add(item.Key);
            }
        }
        foreach (Vector2Int item in destroyTerrainCoords)
        {
            terrainControllDic.Remove(item);
        }
        destroyTerrainCoords.Clear();
    }


    private void DisableTerrain(Vector2Int coord)
    {
        if (terrainControllDic.TryGetValue(coord, out TerrainController controller))
        {
            controller.Disable();
        }
    }

    private void EnableTerrain(Vector2Int coord)
    {
        if (!terrainControllDic.TryGetValue(coord, out TerrainController controller))
        {
            controller = ResSystem.GetOrNew<TerrainController>();
            controller.Load(coord);
            terrainControllDic.Add(coord, controller);
        }
        controller.Enable();
    }

    public bool IsLoadingCompleted()
    {
        if (terrainControllDic.Count == 0) return false;
        foreach (TerrainController item in terrainControllDic.Values)
        {
            if (item.terrain == null)
            {
                return false;
            }
        }
        return true;
    }

    private Vector2Int GetTerrainCoordByWolrdPos(Vector3 wolrdPos)
    {
        return new Vector2Int((int)(wolrdPos.x / mapConfig.terrainSize), (int)(wolrdPos.z / mapConfig.terrainSize));
    }

    private Vector3 GetWorldPosByTerrainCoord(Vector2Int coord)
    {
        return new Vector3(coord.x * mapConfig.terrainSize, 0, coord.y * mapConfig.terrainSize);
    }

    private bool CheckVisibility(Bounds bounds)
    {
        // 希望实际的可见范围大一些
        bounds.size *= 2;
        if (GeometryUtility.TestPlanesAABB(cameraPlanes, bounds)) return true;
        // 玩家当前地块附近的地块要显示
        Vector3 boundsCenter = GetWorldPosByTerrainCoord(playerTerrainCoord);
        Bounds playerTerrainBounds = new Bounds(boundsCenter, new Vector3(mapConfig.terrainSize, mapConfig.terrainMaxHeight, mapConfig.terrainSize) * 3);
        return bounds.Intersects(playerTerrainBounds);
    }

#if UNITY_EDITOR
    public bool drawGizmos;
    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            quadTree?.Draw();
        }
    }
#endif
}

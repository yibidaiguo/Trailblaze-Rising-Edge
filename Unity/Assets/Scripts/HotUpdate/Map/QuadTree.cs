using System;
using UnityEngine;

public class QuadTree
{
    private static MapConfig mapConfig;
    private static Action<Vector2Int> onTerrainEanble;
    private static Action<Vector2Int> onTerrainDisable;
    private static Func<Bounds, bool> onCheckVisibility;
    private class Node
    {
        public Bounds bounds;
        private Node leftAndTop;
        private Node rightAndTop;
        private Node leftAndBottom;
        private Node rightAndBottom;
        private bool isTerrain;
        private Vector2Int terrainCoord;
        public Node(Bounds bounds, bool divide)
        {
            this.bounds = bounds;
            isTerrain = CheckTerrain(out terrainCoord);
            if (divide && bounds.size.x > mapConfig.minQuadTreeNodeSize)
            {
                Divide();
            }
        }

        private void Divide()
        {
            float halfSize = bounds.size.x / 2;
            float posOffset = halfSize / 2;
            float halfHeight = mapConfig.terrainMaxHeight / 2;
            Vector3 childSize = new Vector3(halfSize, mapConfig.terrainMaxHeight, halfSize);
            leftAndTop = new Node(new Bounds(new Vector3(bounds.center.x - posOffset, halfHeight, bounds.center.z + posOffset), childSize), true);
            rightAndTop = new Node(new Bounds(new Vector3(bounds.center.x + posOffset, halfHeight, bounds.center.z + posOffset), childSize), true);
            leftAndBottom = new Node(new Bounds(new Vector3(bounds.center.x - posOffset, halfHeight, bounds.center.z - posOffset), childSize), true);
            rightAndBottom = new Node(new Bounds(new Vector3(bounds.center.x + posOffset, halfHeight, bounds.center.z - posOffset), childSize), true);
        }

        private bool CheckTerrain(out Vector2Int coord)
        {
            Vector3 size = bounds.size;
            bool isTerrain = size.x == mapConfig.terrainSize && size.z == mapConfig.terrainSize;
            coord = Vector2Int.zero;
            if (isTerrain)
            {
                coord.x = (int)(bounds.center.x / mapConfig.terrainSize);
                coord.y = (int)(bounds.center.z / mapConfig.terrainSize);
                // 剔除mapSize以外的符合尺寸的terrain
                int maxCoordAbsX = (int)(mapConfig.mapSize.x / mapConfig.terrainSize) / 2;
                int maxCoordAbsY = (int)(mapConfig.mapSize.y / mapConfig.terrainSize) / 2;
                isTerrain = Mathf.Abs(coord.x) < maxCoordAbsX && Mathf.Abs(coord.y) < maxCoordAbsY;
            }
            return isTerrain;
        }


        private bool active = false;

        public void CheckVisibility()
        {
            bool newActiveState = onCheckVisibility(bounds);
            // 原本可见，现在可见
            // 原本不可见，现在可见
            // if ((active && newActiveState) || (!active && newActiveState))
            if (newActiveState)
            {
                if (isTerrain) onTerrainEanble.Invoke(terrainCoord);
                else
                {
                    leftAndTop?.CheckVisibility();
                    rightAndTop?.CheckVisibility();
                    leftAndBottom?.CheckVisibility();
                    rightAndBottom?.CheckVisibility();
                }
            }
            // 原本可见，现在不可见
            else if (active && !newActiveState)
            {
                Disable();
            }
            active = newActiveState;
        }

        public void Disable()
        {
            leftAndTop?.Disable();
            rightAndTop?.Disable();
            leftAndBottom?.Disable();
            rightAndBottom?.Disable();
            if (isTerrain)
            {
                onTerrainDisable.Invoke(terrainCoord);
            }
        }


#if UNITY_EDITOR
        public void Draw()
        {
            Gizmos.color = isTerrain ? Color.green : Color.white;
            Gizmos.DrawWireCube(bounds.center, bounds.size * 1);
            Gizmos.color = Color.white;
            leftAndTop?.Draw();
            rightAndTop?.Draw();
            leftAndBottom?.Draw();
            rightAndBottom?.Draw();
        }
#endif
    }

    private Node rootNode;
    public QuadTree(MapConfig config, Action<Vector2Int> terrainEanble, Action<Vector2Int> terrainDisable, Func<Bounds, bool> checkVisibility)
    {
        mapConfig = config;
        onTerrainEanble = terrainEanble;
        onTerrainDisable = terrainDisable;
        onCheckVisibility = checkVisibility;
        Bounds rootBounds = new Bounds(new Vector3(0, mapConfig.terrainMaxHeight / 2, 0), new Vector3(mapConfig.quadTreeSize, mapConfig.terrainMaxHeight, mapConfig.quadTreeSize));
        rootNode = new Node(rootBounds, true);
    }

    public void CheckVisibility()
    {
        rootNode.CheckVisibility();
    }

#if UNITY_EDITOR
    public void Draw()
    {
        rootNode?.Draw();
    }
#endif
}

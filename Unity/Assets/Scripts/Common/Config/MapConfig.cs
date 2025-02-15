using JKFrame;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/MapConfig")]
public class MapConfig : ConfigBase
{
    // 四叉树的尺寸，300 * 4 * 4 * 4 = 192000
    public float quadTreeSize = 19200;
    public Vector2 mapSize = new Vector2(12000, 12000);
    public Vector2Int terrainResKeyCoordOffset;
    public float terrainSize = 300;
    public float minQuadTreeNodeSize = 300;
    public float terrainMaxHeight = 200f;

    private void OnValidate()
    {
        terrainResKeyCoordOffset = new Vector2Int((int)(mapSize.x / terrainSize / 2), (int)(mapSize.y / terrainSize / 2));
    }
}

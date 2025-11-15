using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TilePrefabPair
{
    public TileBase tile;      // 代表 Prefab 的 Tile
    public GameObject prefab;  // 需要生成的 Prefab
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TilemapSpawner : MonoBehaviour
{
    public Tilemap tilemap;  
    public TilePrefabData tilePrefabData;
    void Start()
    {
        SpawnPrefabs();
    }

    void SpawnPrefabs()
    {
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile == null) continue; // 略過空白 Tile

            foreach (TilePrefabPair pair in tilePrefabData.TilePrefabPairs)
            {
                if (tile == pair.tile && pair.prefab != null)
                {
                    var ob = Instantiate(pair.prefab, transform);
                    ob.transform.position = tilemap.GetCellCenterWorld(pos);
                    break; // 避免重複匹配
                }
            }
        }
        
        tilemap.gameObject.SetActive(false);
    }
}

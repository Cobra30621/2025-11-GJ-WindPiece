using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TilemapSpawner : MonoBehaviour
{
    public List<Tilemap> tilemaps;  
    public TilePrefabData tilePrefabData;
    void Start()
    {
        SpawnPrefabs();
    }

    public void SpawnPrefabs()
    {
        foreach (var map in tilemaps)
        {
            if (map == null) continue;

            BoundsInt bounds = map.cellBounds;

            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                TileBase tile = map.GetTile(pos);
                if (tile == null) 
                    continue;

                // 找到對應 Prefab
                foreach (TilePrefabPair pair in tilePrefabData.TilePrefabPairs)
                {
                    if (tile == pair.tile && pair.prefab != null)
                    {
                        GameObject ob = Instantiate(pair.prefab, transform);
                        ob.transform.position = map.GetCellCenterWorld(pos);
                        break; // 避免重複匹配
                    }
                }
            }

            // 若要生完隱藏 Tilemap
            map.gameObject.SetActive(false);
        }
    }
}

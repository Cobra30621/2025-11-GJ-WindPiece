using Core.Utils;
using UnityEngine.Tilemaps;

namespace Core.Board
{
    [System.Serializable]
    public class TileTypePair
    {
        public TileBase tile;      // 代表 Prefab 的 Tile
        public TileType type;  // 代表 Tile 的 Type
    }   
}


using System;
using System.Collections.Generic;
using Core.Pieces;
using Core.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Board
{
    public class BoardManager : MonoBehaviour
    {
        public static BoardManager Instance { get; private set; }
        
        public Tilemap groundTilemap; // 用於視覺
        public TileTypeData tileTypeData; // 紀錄類別
        public Vector2Int size = new Vector2Int(8, 8);

        public Vector3 spawnOffset;   // 新增：生成偏移量
        
        [ShowInInspector]
        private Dictionary<Vector2Int, TileCell> cells = new Dictionary<Vector2Int, TileCell>();

        void Awake()
        {
            Instance = this;
        }

        public void GenerateBoard(Tilemap tilemap)
        {
            groundTilemap = tilemap;
            
            InitializeEmptyBoard();
            ReadTilemapToDict();
        }

        public void InitializeEmptyBoard()
        {
            cells.Clear();
            for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                var p = new Vector2Int(x, y);
                cells[p] = new TileCell(p, TileType.Empty);
            }
        }

        public void ReadTilemapToDict()
        {
            cells.Clear();

            // Tilemap 的起點（全取正方向座標）
            BoundsInt bounds = groundTilemap.cellBounds;

            foreach (var pos in bounds.allPositionsWithin)
            {
                TileBase tile = groundTilemap.GetTile(pos);
                if (tile == null) continue; // 略過空白 Tile

                // 轉為簡單的 Vector2Int 方便你拿來當 dict key
                Vector2Int gridPos = new Vector2Int(pos.x, pos.y);
                foreach (TileTypePair pair in tileTypeData.TileTypePairs)
                {
                    if (tile == pair.tile)
                    {
                        TileType type = pair.type;
                        var cell = new TileCell(gridPos,type);
                        cells[gridPos] = cell;
                        break; // 避免重複匹配
                    }
                }          
            }
            foreach (var kvp in cells)
            {
                Debug.Log($"Cell[{kvp.Key.x}, {kvp.Key.y}] = {kvp.Value.Type}");
            }
        }
        public bool IsInside(Vector2Int p) => cells.ContainsKey(p);
        public TileCell GetCell(Vector2Int p) => cells.ContainsKey(p) ? cells[p] : null;

        public bool IsEmpty(Vector2Int p)
        {
            var c = GetCell(p);
            return c != null && c.OccupiedPiece == null && c.Type != TileType.Obstacle;
        }

/// <summary>
        /// 是障礙物
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool ISObstacle(Vector2Int p)
        {
            var c = GetCell(p);
            if (c != null)
            {
                if (c.Type == TileType.Obstacle)
                {
                    return true;
                }
                else
                {
                    return c.OccupiedPiece != null && c.OccupiedPiece.Config.isObstacle;
                }
            }

            return false;
        }

        public bool IsHole(Vector2Int p)
        {
            var c = GetCell(p);
            if (c == null)
            {
                return true;
            }
            else
            {
                return c.Type == TileType.Hole;
            }
        }

        public Vector3 GridToWorld(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x, gridPos.y, 0f) + spawnOffset;
        }

        
        public bool PlacePiece(Piece piece, Vector2Int pos)
        {
            var c = GetCell(pos);
            if (c == null || c.Type == TileType.Obstacle || c.OccupiedPiece != null) return false;
            c.OccupiedPiece = piece;
            piece.Position = pos;
            return true;
        }

        public void RemovePiece(Piece piece)
        {
            var c = GetCell(piece.Position);
            if (c != null && c.OccupiedPiece == piece)
            {
                c.OccupiedPiece = null;
            }
            
        }
        
        public bool TryWorldToGrid(Vector3 worldPos, out Vector2Int gridPos)
        {
            gridPos = new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
            Debug.Log($"worldPos {worldPos} gridPos {gridPos}");
            return true;
        }

        public bool CanPlaceAt(Vector2Int pos)
        {
            return true; // TODO: check tile is empty
        }

        public IEnumerable<TileCell> AllCells() => cells.Values;
    }
}
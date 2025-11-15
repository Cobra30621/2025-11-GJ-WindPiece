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
        
        public Tilemap groundTilemap;
        public TileTypeData tileTypeData;
        public Vector2Int size = new Vector2Int(8, 8);

        public Vector3 spawnOffset;

        [ShowInInspector]
        private Dictionary<Vector2Int, TileCell> cells = new Dictionary<Vector2Int, TileCell>();

        // -------------------------
        // 🔥 PieceRegistry 被整合
        // -------------------------
        private List<Piece> pieces = new List<Piece>();


        void Awake()
        {
            Instance = this;
        }

        // ========================
        //  Board 基礎初始化
        // ========================
        
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

            BoundsInt bounds = groundTilemap.cellBounds;

            foreach (var pos in bounds.allPositionsWithin)
            {
                TileBase tile = groundTilemap.GetTile(pos);
                if (tile == null) continue;

                Vector2Int gridPos = new Vector2Int(pos.x, pos.y);

                foreach (TileTypePair pair in tileTypeData.TileTypePairs)
                {
                    if (tile == pair.tile)
                    {
                        cells[gridPos] = new TileCell(gridPos, pair.type);
                        break;
                    }
                }
            }

            foreach (var kvp in cells)
            {
                Debug.Log($"Cell[{kvp.Key.x}, {kvp.Key.y}] = {kvp.Value.Type}");
            }
        }

        // ========================
        //  基礎查詢
        // ========================

        public bool IsInside(Vector2Int p) => cells.ContainsKey(p);
        public TileCell GetCell(Vector2Int p) => cells.ContainsKey(p) ? cells[p] : null;

        public bool CanAddPiece(Vector2Int p)
        {
            var c = GetCell(p);
            // 要有 Cell
            if (c == null)
            {
                return false;
            }

            // 要是空的
            if (c.Type != TileType.Empty)
            {
                return false;
            }

            // Cell 上不能有東西
            if (c.OccupiedPiece != null)
            {
                return false;
            }

            return true;
        }


        public bool CanMove(Vector2Int p)
        {
            // 如果是洞可以移動
            if (IsHole(p))
            {
                return true;
            }

            // 如果是空的，可以移動
            if (IsEmpty(p))
            {
                return true;
            }
            
            // 如果是障礙物，不能移動
            if (ISObstacle(p))
            {
                return false;
            }

            // 如果是棋子，不能移動
            if (IsPiece(p))
            {
                return false;
            }

            
            return true;
        }
        
        
        
        
        public bool IsEmpty(Vector2Int p)
        {
            var c = GetCell(p);
            return c != null && c.OccupiedPiece == null;
        }

        public bool ISObstacle(Vector2Int p)
        {
            var c = GetCell(p);
            if (c == null) return false;

            return c.OccupiedPiece != null && c.OccupiedPiece.Config.isObstacle;
        }

        public bool IsHole(Vector2Int p)
        {
            var c = GetCell(p);
            if (c == null) return true;
            return c.Type == TileType.Hole;
        }

        public bool IsPiece(Vector2Int p)
        {
            var c = GetCell(p);
            if (c == null) return false;
            
            return c.OccupiedPiece != null && !c.OccupiedPiece.Config.isObstacle;
        }



        public Vector3 GridToWorld(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x, gridPos.y, 0f) + spawnOffset;
        }

        public bool TryWorldToGrid(Vector3 worldPos, out Vector2Int gridPos)
        {
            gridPos = new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
            Debug.Log($"worldPos {worldPos} gridPos {gridPos}");
            return true;
        }

        public IEnumerable<TileCell> AllCells() => cells.Values;

        // ================================
        // 🔥 Piece 管理（原 PieceRegistry）
        // ================================

        /// <summary>
        /// 新增棋子，並放入 TileCell
        /// </summary>
        public bool AddPiece(Piece piece, Vector2Int pos)
        {
            if (!CanAddPiece(pos))
                return false;
            
            var cell = GetCell(pos);
            cell.OccupiedPiece = piece;
            piece.Position = pos;

            pieces.Add(piece);

            return true;
        }

        /// <summary>
        /// 移除棋子（包括清除 Cell 與摧毀 Obj）
        /// </summary>
        public void RemovePiece(Piece piece)
        {
            var cell = GetCell(piece.Position);
            if (cell != null && cell.OccupiedPiece == piece)
                cell.OccupiedPiece = null;

            pieces.Remove(piece);

            Destroy(piece.gameObject);
        }

        /// <summary>
        /// 判斷格子是否被棋子占據
        /// </summary>
        public bool IsOccupiedPiece(Vector2Int pos)
        {
            return GetPieceAt(pos) != null;
        }

        /// <summary>
        /// 取得特定格子的棋子
        /// </summary>
        public Piece GetPieceAt(Vector2Int pos)
        {
            var cell = GetCell(pos);
            if (cell == null) return null;
            return cell.OccupiedPiece;
        }

        /// <summary>
        /// 取得所有活著的棋子
        /// </summary>
        public List<Piece> GetAllPieces()
        {
            // 回傳 shallow copy 避免外部修改
            return new List<Piece>(pieces);
        }

        /// <summary>
        /// 用於棋子移動時更新 Grid 資訊
        /// </summary>
        public void MovePiece(Piece piece, Vector2Int newPos)
        {
            var oldCell = GetCell(piece.Position);
            var newCell = GetCell(newPos);

            if (oldCell != null && oldCell.OccupiedPiece == piece)
                oldCell.OccupiedPiece = null;

            if (newCell != null)
                newCell.OccupiedPiece = piece;

            piece.Position = newPos;
        }
    }
}

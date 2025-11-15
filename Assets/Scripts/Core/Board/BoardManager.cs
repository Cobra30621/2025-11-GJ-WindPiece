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
        private List<Piece> pieces = new List<Piece>();

        void Awake() => Instance = this;

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
                cells[new Vector2Int(x, y)] = new TileCell(new Vector2Int(x, y), TileType.Empty);
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
                Debug.Log($"Cell[{kvp.Key.x}, {kvp.Key.y}] = {kvp.Value.Type}");
        }

        // ========================
        //  基礎查詢
        // ========================

        public bool IsInside(Vector2Int p) => cells.ContainsKey(p);
        public TileCell GetCell(Vector2Int p) => cells.TryGetValue(p, out var cell) ? cell : null;
        public IEnumerable<TileCell> AllCells() => cells.Values;

        public List<Piece> AllEnemies()
        {
            return pieces.FindAll(p => p is EnemyPiece);
        }

        // ========================
        //  Cell 狀態判斷
        // ========================

        public bool IsHole(Vector2Int p)
        {
            return GetCellState(p) == CellState.Hole;
        }
        
        public bool CanMove(Vector2Int p)
        {
            TileCell c = GetCell(p);
            if (c == null) return true;

            return c.CanMove();
        }
        
        public CellState GetCellState(Vector2Int p)
        {
            TileCell c = GetCell(p);
            if (c == null) return CellState.Hole;

            return c.GetCellState();
        }


        public bool CanAddPiece(Vector2Int p) => GetCellState(p) == CellState.Empty;

        

        // ========================
        //  坐標轉換
        // ========================

        public Vector3 GridToWorld(Vector2Int gridPos) => new Vector3(gridPos.x, gridPos.y, 0f) + spawnOffset;

        public bool TryWorldToGrid(Vector3 worldPos, out Vector2Int gridPos)
        {
            gridPos = new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
            return true;
        }

        // ================================
        //  Piece 管理
        // ================================

        public bool AddPiece(Piece piece, Vector2Int pos)
        {
            if (!CanAddPiece(pos)) return false;

            var cell = GetCell(pos);
            cell.OccupiedPiece = piece;
            piece.Position = pos;
            pieces.Add(piece);

            return true;
        }

        public void RemovePiece(Piece piece)
        {
            var cell = GetCell(piece.Position);
            if (cell?.OccupiedPiece == piece) cell.OccupiedPiece = null;
            pieces.Remove(piece);
            Destroy(piece.gameObject);
        }

        public bool IsOccupiedPiece(Vector2Int pos) => GetPieceAt(pos) != null;

        public Piece GetPieceAt(Vector2Int pos) => GetCell(pos)?.OccupiedPiece;

        public List<Piece> GetAllPieces() => new List<Piece>(pieces);

        public void MovePiece(Piece piece, Vector2Int newPos)
        {
            var oldCell = GetCell(piece.Position);
            var newCell = GetCell(newPos);

            if (oldCell?.OccupiedPiece == piece) oldCell.OccupiedPiece = null;
            if (newCell != null) newCell.OccupiedPiece = piece;

            piece.Position = newPos;
        }
    }
}

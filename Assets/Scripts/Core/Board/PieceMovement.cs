using System;
using System.Collections;
using System.Collections.Generic;
using Core.Pieces;
using Core.Utils;
using Core.Wind;
using UnityEngine;

namespace Core.Board
{
    public class PieceMovement : MonoBehaviour
    {
        public static PieceMovement Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private BoardManager board => BoardManager.Instance;
        
        private DeathRattleQueue deathQueue = new DeathRattleQueue();

        
        // ============================================================
        // 1) 主入口：依據指定來源棋子 → 局部風影響
        // ============================================================
        public List<PieceMoveResult> ResolveWindAndGetMoves(Piece source)
        {
            var moves = new List<PieceMoveResult>();

            // 來源棋子吹風（只吹範圍內的棋子）
            EnqueueWindFrom(source, moves);
            
            PieceMovement.Instance.HandleDeathQueue(moves);
            return moves;
        }

        // ============================================================
        // 2) 局部風吹：依據來源棋子風向 → 吹影響範圍內的棋子（非全場）
        // ============================================================
        private void EnqueueWindFrom(Piece source, List<PieceMoveResult> moves)
        {
            if (!(source is WindPiece wp))
                return;

            Vector2Int windDir = UtilsTool.DirectionToVector2Int(wp.Config.windDirection);
            Vector2Int origin = source.Position;

            var allPieces = BoardManager.Instance.GetAllPieces();

            foreach (var piece in allPieces)
            {
                if (piece == source)
                    continue;

                if (piece.Config.isObstacle)
                    continue;

                // 方向範圍判定
                bool inRange =
                    (windDir == Vector2Int.right && piece.Position.x >= origin.x) ||
                    (windDir == Vector2Int.left  && piece.Position.x <= origin.x) ||
                    (windDir == Vector2Int.up    && piece.Position.y >= origin.y) ||
                    (windDir == Vector2Int.down  && piece.Position.y <= origin.y);

                if (!inRange)
                    continue;

                ApplyPieceMove(piece, windDir, moves);

            }
        }
        
        public List<PieceMoveResult> ResolveEnemyAndGetMoves()
        {
            var moves = new List<PieceMoveResult>();

            // 來源棋子吹風（只吹範圍內的棋子）
            foreach (var cell in board.AllCells())
            {
                var p = cell.OccupiedPiece as EnemyPiece;
                if (p == null) continue;
                Vector2Int dir = UtilsTool.DirectionToVector2Int(p.MoveDirection);

                ApplyPieceMove(p, dir, moves);
            }
            
            HandleDeathQueue(moves);
            return moves;
        }

        
        
        // ============================================================
        // 共用風吹邏輯：移動、掉落、阻擋
        // ============================================================

        public bool ApplyPieceMove(Piece piece, Vector2Int windDir, List<PieceMoveResult> moves)
        {
            Vector2Int targetPos = piece.Position + windDir;
            if (piece.Config.isObstacle)
            {
                return false;
            }
            
            if (!board.CanMove(targetPos))
                return false;
            
            // 已經掉落的棋子無法移動
            if (piece.IsFalling)
                return false;

            
            bool isFalling = board.GetCellState(targetPos) == BoardManager.CellState.Hole;
            
            moves.Add(new PieceMoveResult
            {
                piece = piece,
                from = piece.Position,
                to = targetPos,
                isFalling = isFalling
            });

            board.MovePiece(piece, targetPos);

            if (isFalling)
                RegisterFallingPiece(piece);

            return true;
        }
        
        // ============================================================
        // 亡語註冊
        // ============================================================
        public void RegisterFallingPiece(Piece p)
        {
            p.IsFalling = true;
            deathQueue.Add(p);
        }


        public void HandleDeathQueue(List<PieceMoveResult> moves)
        {
            // 若有掉落 → 亡語風吹（全場）
            while (!deathQueue.IsEmpty)
            {
                var deadPiece = deathQueue.Pop();
                ResolveGlobalWind(deadPiece, moves);
            }
        }
        
        // ============================================================
        // 3) 亡語風吹：整個場上所有棋子都吹 1 格
        // ============================================================
        private void ResolveGlobalWind(Piece sourceOfDeath, List<PieceMoveResult> moves)
        {
            if (!(sourceOfDeath is WindPiece wp))
                return;

            Vector2Int windDir = UtilsTool.DirectionToVector2Int(wp.Config.windDirection);

            var allPieces = BoardManager.Instance.GetAllPieces();

            foreach (var piece in allPieces)
            {
                Debug.Log($"Global wind on {piece.name}");

                PieceMovement.Instance.ApplyPieceMove(piece, windDir, moves);

            }
        }
    }
}
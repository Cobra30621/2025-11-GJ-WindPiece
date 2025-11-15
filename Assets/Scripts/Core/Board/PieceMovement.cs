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
        private BoardManager board => BoardManager.Instance;
        private DeathRattleQueue deathQueue = new DeathRattleQueue();

        private void Awake()
        {
            Instance = this;
        }

        // ============================================================
        // 公共方法：取得 Wind 或 Enemy 移動結果
        // ============================================================
        public List<PieceMoveResult> ResolveWindMoves(Piece source)
        {
            var moves = new List<PieceMoveResult>();
            if (source is WindPiece)
                EnqueueDirectionalMoves(GetAllPiecesExcept(source), source.Position, GetWindDirection(source), moves);

            HandleDeathQueue(moves);
            return moves;
        }

        public List<PieceMoveResult> ResolveEnemyMoves()
        {
            var moves = new List<PieceMoveResult>();
            foreach (var piece in board.AllEnemies())
            {
                if (piece is EnemyPiece enemy)
                    ApplyPieceMove(enemy, UtilsTool.DirectionToVector2Int(enemy.MoveDirection), moves);
            }

            HandleDeathQueue(moves);
            return moves;
        }

        // ============================================================
        // 共用邏輯：計算移動、掉落、阻擋
        // ============================================================
        public bool ApplyPieceMove(Piece piece, Vector2Int dir, List<PieceMoveResult> moves)
        {
            if (piece.Config.isObstacle || piece.IsFalling)
                return false;

            Vector2Int targetPos = piece.Position + dir;
            if (!board.CanMove(targetPos))
                return false;

            bool isFalling = board.IsHole(targetPos);

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

        public void RegisterFallingPiece(Piece p)
        {
            p.IsFalling = true;
            deathQueue.Add(p);
        }

        // ============================================================
        // 死亡棋子亡語處理
        // ============================================================
        private void HandleDeathQueue(List<PieceMoveResult> moves)
        {
            while (!deathQueue.IsEmpty)
            {
                var deadPiece = deathQueue.Pop();
                if (deadPiece is WindPiece)
                    EnqueueGlobalWind(deadPiece, moves);
            }
        }

        private void EnqueueGlobalWind(Piece sourceOfDeath, List<PieceMoveResult> moves)
        {
            Vector2Int windDir = GetWindDirection(sourceOfDeath);
            foreach (var piece in GetAllPieces())
            {
                ApplyPieceMove(piece, windDir, moves);
            }
        }

        // ============================================================
        // 工具方法
        // ============================================================
        private Vector2Int GetWindDirection(Piece piece)
        {
            if (piece is WindPiece wp)
                return UtilsTool.DirectionToVector2Int(wp.Config.windDirection);
            return Vector2Int.zero;
        }

        private IEnumerable<Piece> GetAllPieces() => board.GetAllPieces();

        private IEnumerable<Piece> GetAllPiecesExcept(Piece exclude)
        {
            foreach (var p in board.GetAllPieces())
                if (p != exclude && !p.Config.isObstacle)
                    yield return p;
        }

        private void EnqueueDirectionalMoves(IEnumerable<Piece> pieces, Vector2Int origin, Vector2Int dir, List<PieceMoveResult> moves)
        {
            foreach (var piece in pieces)
            {
                if (!IsInDirectionRange(piece.Position, origin, dir))
                    continue;

                ApplyPieceMove(piece, dir, moves);
            }
        }

        private bool IsInDirectionRange(Vector2Int pos, Vector2Int origin, Vector2Int dir)
        {
            if (dir == Vector2Int.right) return pos.x >= origin.x;
            if (dir == Vector2Int.left)  return pos.x <= origin.x;
            if (dir == Vector2Int.up)    return pos.y >= origin.y;
            if (dir == Vector2Int.down)  return pos.y <= origin.y;
            return false;
        }
    }
}

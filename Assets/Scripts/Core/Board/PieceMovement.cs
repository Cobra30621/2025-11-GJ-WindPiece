using System.Collections.Generic;
using System.Linq;
using Core.Pieces;
using Core.Utils;
using Core.Wind;
using UnityEngine;

namespace Core.Board
{
    public partial class PieceMovement : MonoBehaviour
    {
        public static PieceMovement Instance { get; private set; }
        private BoardManager board => BoardManager.Instance;
        private DeathRattleQueue deathQueue = new DeathRattleQueue();

        private void Awake()
        {
            Instance = this;
        }

        // ============================================================
        // Movement Event
        // ============================================================

        // ============================================================
        // 對外介面：風 / 敵人移動
        // ============================================================
        public List<MovementEvent> ResolveWindMoves(Piece source)
        {
            var events = new List<MovementEvent>();
            Vector2Int dir = GetWindDirection(source);

            var evt = new MovementEvent(source, dir, true);
            events.Add(evt);

            if (source is WindPiece)
            {
                var pieces = GetAllPiecesExcept(source).ToList();
                EnqueueDirectionalMoves(pieces, source.Position, dir, evt.moves);
            }

            // 包含因死亡觸發的所有事件
            events.AddRange(HandleDeathQueue());

            return events;
        }

        public List<MovementEvent> ResolveEnemyMoves()
        {
            var events = new List<MovementEvent>();

            // 主事件（敵人群體移動）
            var mainEvt = new MovementEvent(null, Vector2Int.zero, false);
            events.Add(mainEvt);

            foreach (var piece in board.AllEnemies())
            {
                if (piece is EnemyPiece enemy && enemy.canMove)
                {
                    Vector2Int dir = UtilsTool.DirectionToVector2Int(enemy.MoveDirection);
                    ApplyPieceMove(enemy, dir, mainEvt.moves);
                }
            }

            events.AddRange(HandleDeathQueue());
            return events;
        }

        // ============================================================
        // 移動邏輯
        // ============================================================
        public bool ApplyPieceMove(Piece piece, Vector2Int dir, List<PieceMoveResult> moves)
        {
            if (piece.Config.isObstacle || piece.IsFalling)
                return false;

            Vector2Int targetPos = piece.Position + dir;
            // Debug.Log($"Apply Move {piece.name} from {piece.Position} add {dir} {board.CanMove(targetPos)}");
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
        // 死亡亡語 -> GlobalWind
        // ============================================================
        private List<MovementEvent> HandleDeathQueue()
        {
            var resultEvents = new List<MovementEvent>();

            while (!deathQueue.IsEmpty)
            {
                var deadPiece = deathQueue.Pop();

                if (deadPiece is WindPiece || deadPiece is EnemyPiece)
                {
                    var globalEvt = TriggerGlobalWind(deadPiece);
                    resultEvents.Add(globalEvt);
                }
            }

            return resultEvents;
        }

        private MovementEvent TriggerGlobalWind(Piece sourceOfDeath)
        {
            Vector2Int windDir = GetWindDirection(sourceOfDeath);

            var evt = new MovementEvent(sourceOfDeath, windDir, true);

            var pieces = GetAllPieces().ToList();
            PieceSorter.SortByDirection(pieces, windDir);

            // Debug.Log($"Trigger Global Move {sourceOfDeath.name}");
            foreach (var piece in pieces)
            {
                // Debug.Log($"Move {piece.name}: {piece.Position} + {windDir}");
                ApplyPieceMove(piece, windDir, evt.moves);
            }

            return evt;
        }

        // ============================================================
        // 工具方法
        // ============================================================
        private Vector2Int GetWindDirection(Piece piece)
        {
            if (piece is WindPiece wp)
                return UtilsTool.DirectionToVector2Int(wp.Config.windDirection);
            if (piece is EnemyPiece ep)
                return UtilsTool.DirectionToVector2Int(ep.MoveDirection);
            return Vector2Int.zero;
        }

        private IEnumerable<Piece> GetAllPieces() => board.GetAllPieces();

        private IEnumerable<Piece> GetAllPiecesExcept(Piece exclude)
        {
            foreach (var p in board.GetAllPieces())
                if (p != exclude && !p.Config.isObstacle)
                    yield return p;
        }

        private void EnqueueDirectionalMoves(List<Piece> pieces, Vector2Int origin, Vector2Int dir, List<PieceMoveResult> moves)
        {
            PieceSorter.SortByDirection(pieces, dir);

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
        
        public static List<PieceMoveResult> FlattenMovementEvents(List<MovementEvent> events)
        {
            var allMoves = new List<PieceMoveResult>();
            foreach (var evt in events)
            {
                allMoves.AddRange(evt.moves);
            }
            return allMoves;
        }
    }
}

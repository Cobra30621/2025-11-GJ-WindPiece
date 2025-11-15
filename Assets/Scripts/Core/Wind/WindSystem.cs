using System.Collections.Generic;
using Core.Board;
using Core.Pieces;
using Core.Utils;
using UnityEngine;

namespace Core.Wind
{
    /// <summary>
    /// 提供兩種介面：
    /// - ResolveWindAndGetMoves(Piece source): 回傳要執行的 moves 列表（供 animator 播放）
    /// - (內部) 處理掉落與亡語隊列
    /// </summary>
    public class WindSystem : MonoBehaviour
    {
        public BoardManager board;
        private DeathRattleQueue deathQueue = new DeathRattleQueue();

        // 主介面：接收來源棋子，回傳整套 move list（不直接執行）
        public List<PieceMoveResult> ResolveWindAndGetMoves(Piece source)
        {
            var moves = new List<PieceMoveResult>();
            // 1) 初次來源觸發
            EnqueueWindFrom(source, moves);

            // 2) 處理掉落觸發的亡語（依 spawn order）
            while (!deathQueue.IsEmpty)
            {
                var p = deathQueue.Pop();
                EnqueueWindFrom(p, moves);
            }

            return moves;
        }

        private void EnqueueWindFrom(Piece source, List<PieceMoveResult> moves)
        {
            // 1) 找受影響的 pieces（簡單直線範圍）
            if (!(source is WindPiece wp)) return;
            Vector2Int dir = Utils.UtilsTool.DirectionToVector2Int(wp.WindDirection);

            for (int r = 1; r <= wp.WindRange; r++)
            {
                Vector2Int cellPos = source.Position + dir * r;
                var cell = board.GetCell(cellPos);
                if (cell == null) break;
                if (cell.Type == TileType.Obstacle) break;
                if (cell.OccupiedPiece != null)
                {
                    // 計算最終位移：一直推到下一格或停在障礙/邊界
                    Vector2Int dest = cellPos;
                    while (true)
                    {
                        Vector2Int next = dest + dir;
                        var nextCell = board.GetCell(next);
                        if (nextCell == null) // 掉出場外 -> mark falling and remove
                        {
                            var move = new PieceMoveResult{ piece = cell.OccupiedPiece, from = cellPos, to = next, isFalling = true };
                            moves.Add(move);
                            // update board state now: remove piece
                            board.RemovePiece(cell.OccupiedPiece);
                            deathQueue.Add(cell.OccupiedPiece);
                            break;
                        }
                        if (nextCell.Type == TileType.Obstacle) break;
                        if (nextCell.OccupiedPiece != null) { dest = next; continue; }
                        // empty -> move into it
                        var move2 = new PieceMoveResult{ piece = cell.OccupiedPiece, from = cellPos, to = next, isFalling = false };
                        moves.Add(move2);
                        // perform logical move in board
                        board.RemovePiece(cell.OccupiedPiece);
                        board.PlacePiece(cell.OccupiedPiece, next);
                        break;
                    }
                }
            }
        }

        // 外部也可直接註冊掉落 pieces
        public void RegisterFallingPiece(Piece p) => deathQueue.Add(p);
    }
}

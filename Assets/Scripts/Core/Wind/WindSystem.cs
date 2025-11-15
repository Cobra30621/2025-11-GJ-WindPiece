using System.Collections.Generic;
using Core.Board;
using Core.Pieces;
using Core.Utils;
using UnityEngine;

namespace Core.Wind
{
    public class WindSystem : MonoBehaviour
    {
        public BoardManager board;
        private DeathRattleQueue deathQueue = new DeathRattleQueue();

        /// <summary>
        /// 主介面：解析風效果，回傳每個棋子移動結果
        /// </summary>
        public List<PieceMoveResult> ResolveWindAndGetMoves(Piece source)
        {
            var moves = new List<PieceMoveResult>();

            EnqueueWindFrom(source, moves);

            // 處理掉落亡語
            while (!deathQueue.IsEmpty)
            {
                var p = deathQueue.Pop();
                EnqueueWindFrom(p, moves);
            }

            return moves;
        }

        /// <summary>
        /// 依照來源棋子，計算哪些棋子會被吹動
        /// </summary>
        private void EnqueueWindFrom(Piece source, List<PieceMoveResult> moves)
        {
            if (!(source is WindPiece wp)) return;

            Vector2Int windDir = Utils.UtilsTool.DirectionToVector2Int(wp.Config.windDirection);
            Vector2Int origin = source.Position;

            var allPieces = PieceRegistry.Instance.GetAllPieces();

            foreach (var piece in allPieces)
            {
                if (piece == source) continue; // 不吹自己
                if (piece.Config.isObstacle) continue; // 障礙物不會被吹

                bool inRange = false;
                bool isFalling = false;

                // 判定棋子是否在風範圍
                if (windDir == Vector2Int.right && piece.Position.x >= origin.x) inRange = true;
                else if (windDir == Vector2Int.left && piece.Position.x <= origin.x) inRange = true;
                else if (windDir == Vector2Int.up && piece.Position.y >= origin.y) inRange = true;
                else if (windDir == Vector2Int.down && piece.Position.y <= origin.y) inRange = true;
                
                if (!inRange) continue;
                
                

                // 計算下一格位置 (一次吹一格)
                Vector2Int targetPos = piece.Position + windDir;
                var targetCell = board.GetCell(targetPos);
         
                
                // 掉入洞
                // TODO 判定放在
                Debug.Log($"get cell {targetPos}, {board.GetCell(targetPos)}");

                if (board.IsHole(targetPos))
                {
                    isFalling = true;
                }
                // 移動判定
                else if ( board.ISObstacle(targetPos))
                {
                    // 無法移動，跳過
                    continue;
                }
                
                // 建立移動結果
                var moveResult = new PieceMoveResult
                {
                    piece = piece,
                    from = piece.Position,
                    to = targetPos,
                    isFalling = isFalling
                };
                moves.Add(moveResult);

                // 更新棋盤狀態
                board.RemovePiece(piece);
                board.PlacePiece(piece, targetPos);
            }
        }

        /// <summary>
        /// 外部可用：將掉落棋子註冊進亡語隊列
        /// </summary>
        public void RegisterFallingPiece(Piece p) => deathQueue.Add(p);
    }
}

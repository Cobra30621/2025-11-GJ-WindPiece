using System.Collections.Generic;
using Core.Board;
using Core.Pieces;
using UnityEngine;

namespace Core.Wind
{
    /// <summary>
    /// 複製 board state（淺或深複製），在副本上執行 ResolveWindAndGetMoves 的邏輯，回傳模擬出的 moves → 用於 UI 預覽
    /// </summary>
    public class PreviewSimulator
    {
        private BoardManager board;

        public PreviewSimulator(BoardManager board)
        {
            this.board = board;
        }

        public SimulationResult SimulatePlacePiece(Piece prefab, Vector2Int pos)
        {
            // TODO: 實際實作需要深複製 board 與 pieces
            // 這裡回傳空結果作為骨架
            return new SimulationResult { Moves = new List<PieceMoveResult>() };
        }
    }

    public class SimulationResult
    {
        public List<PieceMoveResult> Moves;
    }
}
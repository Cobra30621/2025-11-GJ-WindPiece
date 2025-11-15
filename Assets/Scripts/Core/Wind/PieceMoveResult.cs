using Core.Pieces;
using UnityEngine;

namespace Core.Wind
{
    public struct PieceMoveResult
    {
        public Piece piece;
        public Vector2Int from;
        public Vector2Int to;
        public bool isFalling;
    }
}
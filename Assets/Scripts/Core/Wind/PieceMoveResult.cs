using System;
using Core.Pieces;
using UnityEngine;

namespace Core.Wind
{
    [Serializable]
    public struct PieceMoveResult
    {
        public Piece piece;
        public Vector2Int from;
        public Vector2Int to;
        public bool isFalling;
    }
}
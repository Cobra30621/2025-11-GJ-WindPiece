using System;
using System.Collections.Generic;
using Core.Pieces;
using Core.Wind;
using UnityEngine;

namespace Core.Board
{
    [Serializable]
    public class MovementEvent
    {
        public Piece source;
        public Vector2Int direction;
        public List<PieceMoveResult> moves = new List<PieceMoveResult>();
        public bool IsWind;
        
        
        public MovementEvent(Piece s, Vector2Int dir, bool isWind)
        {
            source = s;
            direction = dir;
            IsWind = isWind;
        }
    }
}
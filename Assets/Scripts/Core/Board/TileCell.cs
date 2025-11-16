using System;
using Core.Pieces;
using Core.Utils;
using UnityEngine;

namespace Core.Board
{
    [Serializable]
    public class TileCell
    {
        public TileType Type;
        public Piece OccupiedPiece; // null if none
        public Vector2Int Pos;

        public TileCell(Vector2Int pos, TileType type)
        {
            Pos = pos;
            Type = type;
            OccupiedPiece = null;
        }
        
        public bool CanAddPiece()
        {
            var state = GetCellState();
            return state  == CellState.Empty;
        }
        
        public bool CanMove()
        {
            var state = GetCellState();
            return state  == CellState.Empty || 
                   state  == CellState.Hole;
        }
        

        public CellState GetCellState()
        {
            if (Type == TileType.Hole)
            {
                return CellState.Hole;
            }
            
            if (OccupiedPiece != null)
            {
                if (OccupiedPiece.Config.isObstacle)
                {
                    return CellState.Obstacle;
                }
                else
                {
                    if (OccupiedPiece.IsFalling)
                    {
                        return CellState.Empty;
                    }
                    return CellState.Piece;
                }
            }
            

            return CellState.Empty;
        }


        public bool IsHole()
        {
            return Type == TileType.Hole;
        }
    }
}
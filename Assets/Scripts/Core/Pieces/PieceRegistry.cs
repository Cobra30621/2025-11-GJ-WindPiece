using System.Collections.Generic;
using UnityEngine;

namespace Core.Pieces
{
    public class PieceRegistry : MonoBehaviour
    {
        public static PieceRegistry Instance;
        private void Awake() => Instance = this;

        private List<Piece> pieces = new List<Piece>();

        public void AddPiece(Piece piece) => pieces.Add(piece);
        public void RemovePiece(Piece piece)
        {
            pieces.Remove(piece);
            Destroy(piece.gameObject);
        }

        public bool IsOccupied(Vector2Int gridPos)
        {
            foreach (var piece in pieces)
                if (piece.Position == gridPos) return true;
            return false;
        }

        public List<Piece> GetAllPieces() => new List<Piece>(pieces);
    }
}
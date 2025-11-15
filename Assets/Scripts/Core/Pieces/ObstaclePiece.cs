using Core.Utils;

namespace Core.Pieces
{
    public class ObstaclePiece : Piece{
        void Awake()
        {
           PieceType = PieceType.Obstacle;
        }
        
    }
}
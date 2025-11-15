using Core.Utils;

namespace Core.Pieces
{
    public class EnemyPiece : global::Core.Pieces.Piece
    {
        public Direction MoveDirection = Direction.Left;

        void Awake()
        {
            PieceType = PieceType.Enemy;
        }

        // Enemy movement is simple linear move executed by GameManager/Enemy system
    }
}
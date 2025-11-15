using Core.Utils;

namespace Core.Pieces
{
    public class WindPiece : Piece
    {
        public Direction WindDirection = Direction.Right;
        public int WindRange = 3;

        void Awake()
        {
            PieceType = PieceType.Wind;
        }

        public override void OnActivate()
        {
            // 具體推動行為由 WindSystem 處理
        }

        public override void OnDeathRattle()
        {
            // 亡語也會觸發風
            OnActivate();
        }
    }
}
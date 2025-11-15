using Core.GameFlow;
using Core.Utils;
using Core.Wind;
using Sirenix.OdinInspector;

namespace Core.Pieces
{
    public class WindPiece : Piece
    {
        void Awake()
        {
            PieceType = PieceType.Wind;
        }

        [Button]
        public override void OnActivate()
        {
           
        }

        public override void OnDeathRattle()
        {
            // 亡語也會觸發風
            OnActivate();
        }
    }
}
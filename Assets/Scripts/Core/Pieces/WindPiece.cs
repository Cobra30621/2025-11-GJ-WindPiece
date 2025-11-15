using Core.GameFlow;
using Core.Utils;
using Core.Wind;
using Sirenix.OdinInspector;

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

        [Button]
        public override void OnActivate()
        {
            // 具體推動行為由 WindSystem 處理
            // var moves = GameManager.Instance.windSystem.ResolveWindAndGetMoves(this);
            //
            //
            // // moves 可直接傳給動畫系統或預覽系統
            // foreach(var move in moves)
            // {
            //     move.piece.MoveToWorld(board.GridToWorld(move.to));
            // }
        }

        public override void OnDeathRattle()
        {
            // 亡語也會觸發風
            OnActivate();
        }
    }
}
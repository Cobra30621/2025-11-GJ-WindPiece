using System;
using Core.Pieces;

namespace Core.Utils
{
    public static class GameEventBus
    {
        public static Action<Piece> OnPiecePlaced;
        public static Action<Piece> OnPieceRemoved;
        public static Action OnTurnStart_Player;
        public static Action OnTurnStart_Enemy;
        public static Action OnWindStart;
        public static Action OnWindEnd;
        public static Action OnPreviewUpdated;
    }
}
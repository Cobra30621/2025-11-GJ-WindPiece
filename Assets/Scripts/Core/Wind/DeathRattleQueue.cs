using System.Collections.Generic;
using Core.Pieces;

namespace Core.Wind
{
    public class DeathRattleQueue
    {
        private List<Piece> list = new List<Piece>();

        public void Add(Piece p)
        {
            if (p == null) return;
            list.Add(p);
            // keep spawn-order ascending so earlier spawned triggers earlier
            list.Sort((a, b) => a.SpawnOrder.CompareTo(b.SpawnOrder));
        }

        public Piece Pop()
        {
            if (list.Count == 0) return null;
            var p = list[0];
            list.RemoveAt(0);
            return p;
        }

        public bool IsEmpty => list.Count == 0;
    }
}
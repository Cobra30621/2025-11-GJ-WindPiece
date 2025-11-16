using System.Collections.Generic;
using Core.Pieces;
using Core.Utils;
using UnityEngine;

namespace Core.Board
{
    public static class PieceSorter
    {
        /// <summary>
        /// 根據方向 dir 對 Piece List 進行排序
        /// Left  → x 小→大
        /// Right → x 大→小
        /// Up    → y 大→小
        /// Down  → y 小→大
        /// </summary>
        public static void SortByDirection(List<Piece> pieces, Vector2Int dir)
        {
            if (dir == Vector2Int.left){pieces.Sort((a, b) => a.Position.x.CompareTo(b.Position.x));}
            if (dir == Vector2Int.right){pieces.Sort((a, b) => b.Position.x.CompareTo(a.Position.x));}
            if (dir == Vector2Int.up){pieces.Sort((a, b) => b.Position.y.CompareTo(a.Position.y));}
            if (dir == Vector2Int.down){pieces.Sort((a, b) => a.Position.y.CompareTo(b.Position.y));}
            
        }
    }
}
using UnityEngine;

namespace Core.Utils
{
    public static class UtilsTool
    {
        public static Vector2Int DirectionToVector2Int(Direction d)
        {
            switch (d)
            {
                case Direction.Up: return new Vector2Int(0, 1);
                case Direction.Down: return new Vector2Int(0, -1);
                case Direction.Left: return new Vector2Int(-1, 0);
                case Direction.Right: return new Vector2Int(1, 0);
                default: return Vector2Int.zero;
            }
        }
    }
}
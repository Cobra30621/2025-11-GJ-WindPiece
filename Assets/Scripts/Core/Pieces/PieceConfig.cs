using Core.Utils;
using UnityEngine;

namespace Game.Core.Pieces
{
    [CreateAssetMenu(fileName = "PieceConfig", menuName = "Game/PieceConfig")]
    public class PieceConfig : ScriptableObject
    {
        public string pieceName;
        public GameObject prefab;

        public Direction windDirection;
        public int windPower = 1;

        public bool isObstacle;
    }
}
using Core.Utils;
using UnityEngine;

namespace Game.Core.Pieces
{
    [CreateAssetMenu(fileName = "PieceConfig", menuName = "Game/PieceConfig")]
    public class PieceConfig : ScriptableObject
    {
        public string pieceName;
        public string description;
        public Sprite image;
        public GameObject prefab;

        public Direction windDirection;
        
        public bool isObstacle;
    }
}
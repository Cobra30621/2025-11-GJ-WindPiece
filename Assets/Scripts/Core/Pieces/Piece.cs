using Core.Utils;
using Game.Core.Pieces;
using UnityEngine;

namespace Core.Pieces
{
    public abstract class Piece : MonoBehaviour
    {
        public PieceType PieceType;
        public Vector2Int Position { get; set; }
        public int SpawnOrder { get; set; } // 用於亡語優先序
        public bool IsFalling { get; set; } = false;
        
        public PieceConfig Config { get; private set; }
 
        public void Init(PieceConfig config, Vector2Int pos)
        {
            Config = config;
            Position = pos;
        }
        

        // Called when the piece triggers its main effect (e.g., wind)
        public virtual void OnActivate() { }

        // Called when piece falls into hole (亡語)
        public virtual void OnDeathRattle() { }

        // Optional: visual helper to move to grid pos
        public virtual void MoveToWorld(Vector3 worldPos)
        {
            transform.position = worldPos;
        }
    }
}
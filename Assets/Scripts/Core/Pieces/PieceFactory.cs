using Core.GameFlow;
using Game.Core.Pieces;
using UnityEngine;

namespace Core.Pieces
{
    public class PieceFactory : MonoBehaviour
    {
        
        public Vector3 spawnOffset;   // 新增：生成偏移量

         [SerializeField] private Transform spawnPieceTrans;

        public Piece Spawn(PieceConfig config,  Vector2Int pos)
        {
            var worldPos = GameManager.Instance.board.GridToWorld(pos);
            worldPos += spawnOffset;
            var go = Object.Instantiate(config.prefab, spawnPieceTrans);
            go.transform.position = worldPos;

            var piece = go.GetComponent<Piece>();
            piece.Init(config, pos);
            return piece;
        }
    }
}
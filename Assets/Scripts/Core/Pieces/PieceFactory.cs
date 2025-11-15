using Core.GameFlow;
using Core.Utils;
using Game.Core.Pieces;
using UnityEngine;

namespace Core.Pieces
{
    public class PieceFactory : MonoBehaviour
    {
        
         [SerializeField] private Transform spawnPieceTrans;

        public Piece Spawn(PieceConfig config,  Vector2Int pos)
        {
            var worldPos = GameManager.Instance.board.GridToWorld(pos);

            var go = Object.Instantiate(config.prefab, spawnPieceTrans);
            go.transform.position = worldPos;

            var piece = go.GetComponent<Piece>();
            piece.Init(config, pos);
            
            GameManager.Instance.board.PlacePiece(piece, pos);   
            PieceRegistry.Instance.AddPiece(piece);
            
            return piece;
        }
    }
}
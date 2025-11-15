using System;
using Core.Board;
using Core.GameFlow;
using Core.Utils;
using Game.Core.Pieces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Pieces
{
    public class PieceFactory : MonoBehaviour
    {
        public static PieceFactory Instance;

        private void Awake()
        {
            Instance = this;
        }


        [SerializeField] private Transform spawnPieceTrans;

        public Piece Spawn(PieceConfig config,  Vector2Int pos)
        {
            return Spawn(config.prefab, config, pos);
        }
        
        public Piece Spawn(GameObject prefab, PieceConfig config,  Vector2Int pos)
        {
            var worldPos = BoardManager.Instance.GridToWorld(pos);

            var go = Object.Instantiate(prefab, spawnPieceTrans);
            go.transform.position = worldPos;

            var piece = go.GetComponent<Piece>();
            piece.Init(config, pos);
            
            BoardManager.Instance.AddPiece(piece, pos);   
            
            return piece;
        }
    }
}
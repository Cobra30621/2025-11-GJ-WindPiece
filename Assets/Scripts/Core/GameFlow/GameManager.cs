using System.Collections;
using System.Collections.Generic;
using Core.Board;
using Core.Pieces;
using Core.Utils;
using Core.Wind;
using Game.Core.Pieces;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.GameFlow
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public GameState CurrentState { get; private set; } = GameState.Init;

        public PieceAnimator animator;
        public PieceFactory pieceFactory;
        
        
        private int spawnCounter = 0;
          
        public List<PieceMoveResult> moves = new List<PieceMoveResult>();

        void Awake()
        {
            Instance = this;
        }

        [Button]
        void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            CurrentState = GameState.PlayerTurn;
            GameEventBus.OnTurnStart_Player?.Invoke();
        }

        public void StartPlayerTurn()
        {
            CurrentState = GameState.PlayerTurn;
            GameEventBus.OnTurnStart_Player?.Invoke();
        }

        
        public bool PlacePiece(PieceConfig config, Vector2Int pos)
        {
            if (CurrentState != GameState.PlayerTurn) return false;

            // Instantiate & place
            var piece = pieceFactory.Spawn(config, pos);
            piece.SpawnOrder = ++spawnCounter;
            

            GameEventBus.OnPiecePlaced?.Invoke(piece);
            
            StartCoroutine(HandlePlaceAndResolve(piece));

            return true;
        }
        

        IEnumerator HandlePlaceAndResolve(Piece placed)
        {
            yield return HandleWind(placed);
            
            placed.OnActivate();

            // 檢查勝負
            CheckLevelEnd();

            // 下一階段：敵方回合
            StartCoroutine(HandleEnemyTurn());
        }

        public IEnumerator HandleWind(Piece piece)
        {
            CurrentState = GameState.Animating;
            GameEventBus.OnWindStart?.Invoke();

            // windSystem.Resolve returns sequence of moves to play
            moves = PieceMovement.Instance.ResolveWindMoves(piece);
         
            yield return animator.PlayMoves(moves);

            GameEventBus.OnWindEnd?.Invoke();
        }
        
        

        IEnumerator HandleEnemyTurn()
        {
            CurrentState = GameState.EnemyTurn;
            GameEventBus.OnTurnStart_Enemy?.Invoke();

            moves = PieceMovement.Instance.ResolveEnemyMoves();
            yield return animator.PlayMoves(moves);

            // 检查胜负并回到玩家回合
            CheckLevelEnd();
            StartPlayerTurn();
        }

        public void CheckLevelEnd()
        {
            // 簡單示例：若場上沒有 Enemy 則過關
            foreach (var c in BoardManager.Instance.AllCells())
                if (c.OccupiedPiece != null && c.OccupiedPiece.PieceType == PieceType.Enemy)
                    return;
            CurrentState = GameState.Win;
        }
    }
}

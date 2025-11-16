using System;
using System.Collections;
using System.Collections.Generic;
using Core.Board;
using Core.Input;
using Core.Pieces;
using Core.Utils;
using Core.Wind;
using Game.Core.Pieces;
using Sirenix.OdinInspector;
using UI.Main;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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
          
        public List<MovementEvent> moveEvents = new List<MovementEvent>();

        
        public event Action<PieceConfig> OnPiecePlaced;
        
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
            StartPlayerTurn();
        }

        public void StartPlayerTurn()
        {
            CurrentState = GameState.PlayerTurn;
            PieceSelectionManager.Instance.SetLockMode(InputLockMode.Unlock);
            GameEventBus.OnTurnStart_Player?.Invoke();
        }

        
        public bool PlacePiece(PieceConfig config, Vector2Int pos)
        {
            if (CurrentState != GameState.PlayerTurn) return false;

            if (!BoardManager.Instance.CanAddPiece(pos)) return false;
            
            // Lock Player 
            PieceSelectionManager.Instance.SetLockMode(InputLockMode.LockPlacement);
            
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
            moveEvents = PieceMovement.Instance.ResolveWindMoves(piece);
            var moves = PieceMovement.FlattenMovementEvents(moveEvents);
            
            yield return animator.PlayMoves(moves);

            GameEventBus.OnWindEnd?.Invoke();
        }
        
        

        IEnumerator HandleEnemyTurn()
        {
            yield return new WaitForSeconds(0.5f);
            
            CurrentState = GameState.EnemyTurn;
            GameEventBus.OnTurnStart_Enemy?.Invoke();

            moveEvents = PieceMovement.Instance.ResolveEnemyMoves();
            var moves = PieceMovement.FlattenMovementEvents(moveEvents);
            
            yield return animator.PlayMoves(moves);

            // 检查胜负并回到玩家回合
            CheckLevelEnd();
            CheckLose();
            
            StartPlayerTurn();
        }



        public void CheckLose()
        {
            // 沒有棋子可以下就輸了
            if (PieceSelectorUI.Instance.IsPieceEmpty)
            {
                CurrentState = GameState.Lose;
                LoseUIManager.Instance.ShowLose();
            }
        }
        
        public void CheckLevelEnd()
        {
            // 簡單示例：若場上沒有 Enemy 則過關
            foreach (var c in BoardManager.Instance.AllCells())
                if (c.OccupiedPiece != null && c.OccupiedPiece.PieceType == PieceType.Enemy)
                    return;
            CurrentState = GameState.Win;
            
            // 顯示 UI
            WinUIManager.Instance.ShowWin();

            Debug.Log("YOU WIN!");
        }
        
        public void RestartLevel()
        {
            Scene current = SceneManager.GetActiveScene();
            SceneManager.LoadScene(current.buildIndex);
        }

        public void LoadNextLevel()
        {
            Scene current = SceneManager.GetActiveScene();
            int nextIndex = current.buildIndex + 1;

            if (nextIndex >= SceneManager.sceneCountInBuildSettings)
                nextIndex = 0; // 沒有下一關 → 回第一關

            SceneManager.LoadScene(nextIndex);
        }
    }
}

using Core.Board;
using Core.GameFlow;
using UnityEngine;

namespace Core.Stage
{
    public class StageManager : MonoBehaviour
    {
        public static StageManager instance;
        
        public Stage currentStage;
        
        private void Awake()
        {
            instance = this;
        }
        
        private void Start()
        {
            LoadStage();
        }
        
        private void LoadStage()
        {
            if (currentStage == null)
            {
                Debug.LogError("No stage loaded");
                return;
            }

            ChessInputController.Instance.boardTilemap = currentStage.boardTilemap;
            BoardManager.Instance.GenerateBoard(currentStage.boardTilemap);
            currentStage.TilemapSpawner.SpawnPrefabs();
        }
    }
}
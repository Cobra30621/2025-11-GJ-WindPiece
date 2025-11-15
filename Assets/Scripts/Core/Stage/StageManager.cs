using Core.Board;
using Core.GameFlow;
using UnityEngine;

namespace Core.Stage
{
    public class StageManager : MonoBehaviour
    {
        public static StageManager instance;

        public Stage currentStageInstance;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            LoadStage();
        }

        /// <summary>
        /// 載入目前設定的 stagePrefab
        /// </summary>
        private void LoadStage()
        {
            // 設定 Board
            ChessInputController.Instance.boardTilemap = currentStageInstance.boardTilemap;
            BoardManager.Instance.GenerateBoard(currentStageInstance.boardTilemap);

            // 生成 Stage 上的 Prefabs
            currentStageInstance.TilemapSpawner.SpawnPrefabs();
        }
    }
}

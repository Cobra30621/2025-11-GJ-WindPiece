using Core.Board;
using Core.GameFlow;
using UnityEngine;

namespace Core.Stage
{
    public class StageManager : MonoBehaviour
    {
        public static StageManager instance;

        [Header("Stage Prefab")]
        public Stage stagePrefab; // Inspector 可設定的 Prefab

        private Stage currentStageInstance;

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
            if (stagePrefab == null)
            {
                Debug.LogError("No stage prefab assigned");
                return;
            }

            SpawnStage(stagePrefab);
        }

        /// <summary>
        /// 設定 Stage Prefab 並立即生成
        /// </summary>
        /// <param name="newPrefab">要設定的 Stage Prefab</param>
        public void SetStagePrefab(Stage newPrefab)
        {
            if (newPrefab == null)
            {
                Debug.LogError("Stage prefab cannot be null");
                return;
            }

            stagePrefab = newPrefab;
            SpawnStage(stagePrefab);
        }

        /// <summary>
        /// 生成 Stage 實例並初始化
        /// </summary>
        /// <param name="prefab">要生成的 Stage Prefab</param>
        private void SpawnStage(Stage prefab)
        {
            // 刪除舊 Stage
            if (currentStageInstance != null)
            {
                Destroy(currentStageInstance.gameObject);
            }

            // 生成新的 Stage
            currentStageInstance = Instantiate(prefab, transform);
            currentStageInstance.name = prefab.name;

            // 設定 Board
            ChessInputController.Instance.boardTilemap = currentStageInstance.boardTilemap;
            BoardManager.Instance.GenerateBoard(currentStageInstance.boardTilemap);

            // 生成 Stage 上的 Prefabs
            currentStageInstance.TilemapSpawner.SpawnPrefabs();
        }
    }
}

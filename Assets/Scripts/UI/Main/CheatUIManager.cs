using Core.GameFlow;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UI.Main
{
    public class CheatUIManager : MonoBehaviour
    {
        public static CheatUIManager Instance;

        [Header("Cheat UI Panel")]
        public GameObject cheatPanel;

        [Header("Buttons")]
        public UnityEngine.UI.Button restartButton;
        public UnityEngine.UI.Button nextLevelButton;
        public UnityEngine.UI.Button addPieceButton;
        public UnityEngine.UI.Button winButton;
        public UnityEngine.UI.Button applyJumpButton;

        [Header("Input Field")]
        public UnityEngine.UI.InputField jumpLevelInput;

        private void Awake()
        {
            Instance = this;

            if (cheatPanel != null)
                cheatPanel.SetActive(false);

            // 綁定事件
            restartButton?.onClick.AddListener(OnRestartClicked);
            nextLevelButton?.onClick.AddListener(OnNextLevelClicked);
            addPieceButton?.onClick.AddListener(OnAddPieceClicked);
            winButton?.onClick.AddListener(OnWinClicked);
            applyJumpButton?.onClick.AddListener(OnJumpClicked);
        }

        private void Update()
        {
            // 按 M 鍵顯示/隱藏
            // 按 M 鍵顯示/隱藏（新 Input System）
            if (Keyboard.current != null) 
            {
                if( Keyboard.current.mKey.wasPressedThisFrame)
                    Toggle();;
                
                if(Keyboard.current.rKey.wasPressedThisFrame)
                    OnRestartClicked();
            }
        }

        public void Toggle()
        {
            bool active = !cheatPanel.activeSelf;
            cheatPanel.SetActive(active);
        }

        // ============================================================
        // 按鈕功能實作
        // ============================================================

        public void OnRestartClicked()
        {
            GameManager.Instance.RestartLevel();
        }

        private void OnNextLevelClicked()
        {
            GameManager.Instance.LoadNextLevel();
            
        }
        
        private void OnAddPieceClicked()
        {
            PieceSelectorUI.Instance.AddAllPiece();
            // PlacementPreviewManager.Instance.AddPiece();
        }

        private void OnWinClicked()
        {
            WinUIManager.Instance.ShowWin();
        }

        private void OnJumpClicked()
        {
            if (int.TryParse(jumpLevelInput.text, out int level))
            {
                SceneManager.LoadScene(level);
            }
            else
            {
                Debug.LogWarning("跳關輸入錯誤：請輸入有效整數");
            }
        }
    }
}

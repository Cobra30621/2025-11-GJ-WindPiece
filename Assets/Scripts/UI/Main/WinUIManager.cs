using Core.GameFlow;
using UnityEngine;

namespace UI.Main
{
    public class WinUIManager : MonoBehaviour
    {
        public static WinUIManager Instance;

        [Header("Win UI Panel")]
        public GameObject winPanel;
        public UnityEngine.UI.Button restartButton;
        public UnityEngine.UI.Button nextLevelButton;

        private void Awake()
        {
            Instance = this;

            if (winPanel != null)
                winPanel.SetActive(false);

            // 綁事件
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);

            if (nextLevelButton != null)
                nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        }

        public void ShowWin()
        {
            winPanel.SetActive(true);
        }

        private void OnRestartClicked()
        {
            GameManager.Instance.RestartLevel();
        }

        private void OnNextLevelClicked()
        {
            GameManager.Instance.LoadNextLevel();
        }
    }
}
using Core.Audio;
using Core.GameFlow;
using UnityEngine;

namespace UI.Main
{
    public class LoseUIManager : MonoBehaviour
    {
        public static LoseUIManager Instance;

        [Header("Lose UI Panel")]
        public GameObject losePanel;
        public UnityEngine.UI.Button restartButton;

        private void Awake()
        {
            Instance = this;

            if (losePanel != null)
                losePanel.SetActive(false);

            // 綁事件
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);
        }

        /// <summary>
        /// 顯示輸掉 UI
        /// </summary>
        public void ShowLose()
        {
            SFXManager.Instance.PlaySFX(SFXType.Lose);
            if (losePanel != null)
                losePanel.SetActive(true);
        }

        private void OnRestartClicked()
        {
            GameManager.Instance.RestartLevel();
        }

        /// <summary>
        /// 隱藏 UI（可選）
        /// </summary>
        public void Hide()
        {
            if (losePanel != null)
                losePanel.SetActive(false);
        }
    }
}
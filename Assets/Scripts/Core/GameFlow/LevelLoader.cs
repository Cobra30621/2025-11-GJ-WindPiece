using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private string skipSceneName = "CleanScene"; // 要跳過的場景

    public void LoadNextLevel()
    {
        int total = SceneManager.sceneCountInBuildSettings;
        int currentIndex = SceneManager.GetActiveScene().buildIndex;

        // 最多跑 total 次，避免無限迴圈
        for (int i = 1; i <= total; i++)
        {
            int nextIndex = (currentIndex + i) % total;

            string nextScenePath = SceneUtility.GetScenePathByBuildIndex(nextIndex);
            string nextSceneName = System.IO.Path.GetFileNameWithoutExtension(nextScenePath);

            // 如果下一個不是要跳過的場景 → 載入
            if (nextSceneName != skipSceneName)
            {
                SceneManager.LoadScene(nextIndex);
                return;
            }
        }

        Debug.LogWarning("找不到可載入的下一個場景（全部都被跳過？）");
    }
}
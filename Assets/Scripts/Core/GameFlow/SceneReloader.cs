using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.GameFlow
{
    public class SceneReloader : MonoBehaviour
    {
        private static string sceneToReload;

        /// <summary>
        /// 由外部呼叫：重新載入當前場景
        /// </summary>
        public static void ReloadCurrentScene()
        {
            sceneToReload = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene("CleanScene"); // 先進入空場景
        }

        /// <summary>
        /// 在 CleanScene 中自動載入原場景
        /// </summary>
        private void Start()
        {
            // 如果有指定要重新載入的場景
            if (sceneToReload != null && SceneManager.GetActiveScene().name == "CleanScene")
            {
                StartCoroutine(LoadOriginalScene());
            }
        }

        private IEnumerator LoadOriginalScene()
        {
            yield return new WaitForEndOfFrame();  
            SceneManager.LoadScene(sceneToReload);
            sceneToReload = null;
        }
    }
}
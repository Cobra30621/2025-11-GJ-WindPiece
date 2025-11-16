using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private string titleSceneName = "Title"; // 回到的主選單場景名

    void Update()
    {
        // 按空白鍵回主選單
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(titleSceneName);
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UI.Main
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] GameObject titleScreen;        // 封面（Press Space / Click）
        [SerializeField] DialogueManager dialogue;      // 對話系統
        [SerializeField] string nextScene = "Level1";   // 要載入的關卡

        private bool hasStartedDialogue = false;

        void Start()
        {
            dialogue.Close(); // 一開始先關掉對話框

            // ⭐訂閱對話結束事件
            dialogue.OnDialogueFinished += LoadNextScene;
        }

        void Update()
        {
            if (!hasStartedDialogue &&
                (Keyboard.current.spaceKey.wasPressedThisFrame || 
                 Mouse.current.leftButton.wasPressedThisFrame))
            {
                StartDialogue();
            }
        }

        void StartDialogue()
        {
            hasStartedDialogue = true;

            titleScreen.SetActive(false); // 隱藏封面
            dialogue.Show();              // 顯示對話框
        }

        void LoadNextScene()
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}

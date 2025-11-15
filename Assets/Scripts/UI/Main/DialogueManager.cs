using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject panel;    // DialoguePanel
    [SerializeField] TMP_Text dialogueText;   // 顯示文字
    public string[] dialogue;   // 對話內容
    private int index = 0;
    private bool isShowing = false;
    void Start() // test
    {
        string[] lines =
        {
            "hi",
            "hallo",
            "world"
        };

        Show(lines);
    }
    void Update()
    {
        if (!isShowing) return;

        // 左鍵、空白鍵 或 Enter 下一句
        if (Mouse.current.leftButton.wasPressedThisFrame ||
            Keyboard.current.spaceKey.wasPressedThisFrame ||
            Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Next();
        }
    }

    public void Show(string[] lines)
    {
        dialogue = lines;
        index = 0;
        isShowing = true;

        panel.SetActive(true);
        dialogueText.text = dialogue[index];
    }

    public void Next()
    {
        index++;

        if (index >= dialogue.Length)
        {
            Close();
            return;
        }

        dialogueText.text = dialogue[index];
    }

    public void Close()
    {
        isShowing = false;
        panel.SetActive(false);
    }
}

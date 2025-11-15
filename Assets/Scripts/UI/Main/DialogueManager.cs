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
            "你是一個魔法使，但在科技發展的比魔法更方便的時代裡，你的工作並不多，甚至前幾年疫情時差點餓死在家裡。",
            "直至前陣子，一個古老的空島突然出現在城市邊緣，所有考古學家迫不及待登上島。",
            "在島上，他們真的發現了一片遺跡。根據調查，是一個以風屬性魔法所搭建的千年老城",
            "這裡面除了有遺跡的石塊，還有含有魔法石的石頭。以普通人對魔法的了解並不能驅動裡面的石塊、法陣等。",
            "這時，身為全國極少數專精魔法的人之一，你自然被指名前去進一步調查……"
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

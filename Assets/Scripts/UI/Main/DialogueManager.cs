using Core.Input;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;
using Core.Audio;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject panel;    // DialoguePanel
    [SerializeField] TMP_Text dialogueText;   // 顯示文字
    public string[] dialogue;   // 對話內容
    private int index = 0;
    private bool isShowing = false;
    public event Action OnDialogueFinished; // 對話播放完畢
    void Start() // test
    {
        if (dialogue.Length != 0)
        {
            Show(); // 若對話不為空              
        }
        else
        {
            Close();
        }
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

    public void Show()
    {
        if (PieceSelectionManager.Instance != null)
        {
            PieceSelectionManager.Instance.SetLockMode(InputLockMode.Unlock);
        }
        index = 0;
        isShowing = true;

        panel.SetActive(true);
        dialogueText.text = dialogue[index];
        SFXManager.Instance.PlaySFX(SFXType.Talk);
    }

    public void Next()
    {
        index++;

        if (index >= dialogue.Length)
        {
            Close();
            return;
        }

        SFXManager.Instance.PlaySFX(SFXType.Talk);
        dialogueText.text = dialogue[index];
    }

    public void Close()
    {
        if (PieceSelectionManager.Instance != null)
        {
            PieceSelectionManager.Instance.SetLockMode(InputLockMode.Unlock);
        }
        OnDialogueFinished?.Invoke();
        isShowing = false;
        panel.SetActive(false);
    }
}

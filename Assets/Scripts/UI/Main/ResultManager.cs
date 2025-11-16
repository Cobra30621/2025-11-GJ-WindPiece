using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SpriteGroup
{
    public Sprite[] frames; // 你可在 Inspector 為每組放 4 張
}public class ResultManager : MonoBehaviour
{
    [SerializeField] private string titleSceneName = "Title";
    [SerializeField] private Image display;
    [SerializeField] private List<SpriteGroup> groups;

    [SerializeField] private float frameTime = 0.1f;
    [SerializeField] private float groupDuration = 1f;

    void Start()
    {
        StartCoroutine(PlayAllGroups());
    }

    IEnumerator PlayAllGroups()
    {
        foreach (var group in groups)
        {
            yield return StartCoroutine(PlayOneGroup(group));
        }

        SceneManager.LoadScene(titleSceneName);
    }

    IEnumerator PlayOneGroup(SpriteGroup group)
    {
        Sprite[] frames = group.frames;   // 取出這組的 4 張圖

        if (frames == null || frames.Length == 0)
            yield break;

        float timer = 0f;
        int index = 0;

        while (timer < groupDuration)
        {
            display.sprite = frames[index];
            index = (index + 1) % frames.Length;

            yield return new WaitForSeconds(frameTime);
            timer += frameTime;
        }
    }
}

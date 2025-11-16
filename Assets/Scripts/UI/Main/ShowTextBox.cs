using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.Main
{
    public class ShowTextBox : MonoBehaviour
    {
        public static ShowTextBox Instance;

        [Header("UI References")]
        public CanvasGroup panelGroup; // 控制淡入淡出
        public TextMeshProUGUI textLabel;
        public float fadeDuration = 0.3f;
        public float showDuration = 0.8f;

        private void Awake()
        {
            Instance = this;

            if (panelGroup != null)
            {
                panelGroup.alpha = 0f;
                panelGroup.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 對外呼叫入口：StartCoroutine(ShowTextBox.Instance.Show("玩家回合"));
        /// </summary>
        public IEnumerator Show(string message)
        {
            if (panelGroup == null || textLabel == null)
                yield break;

            textLabel.text = message;
            panelGroup.gameObject.SetActive(true);

            // 淡入
            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                panelGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
                yield return null;
            }
            panelGroup.alpha = 1;

            // 停留
            yield return new WaitForSeconds(showDuration);

            // 淡出
            t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                panelGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
                yield return null;
            }
            panelGroup.alpha = 0;

            panelGroup.gameObject.SetActive(false);
        }
    }
}
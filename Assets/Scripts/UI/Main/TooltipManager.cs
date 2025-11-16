using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Main
{
    public class TooltipManager : MonoBehaviour
    {
        public static TooltipManager Instance;

        [Header("Tooltip UI")]
        public RectTransform tooltipRoot;
        public TextMeshProUGUI tooltipText;
        public CanvasGroup canvasGroup;

        [Header("Settings")]
        public Vector2 offset = new Vector2(20f, -20f);
        public float fadeSpeed = 10f;

        private bool isShowing = false;

        private void Awake()
        {
            Instance = this;
            tooltipRoot.gameObject.SetActive(false);
            canvasGroup.alpha = 0;
        }

        private void Update()
        {
            if (!isShowing) return;

            // Tooltip 跟隨滑鼠
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)tooltipRoot.parent,
                Mouse.current.position.ReadValue(),
                null,
                out pos
            );
            tooltipRoot.anchoredPosition = pos + offset;
        }

        public void Show(string message)
        {
            isShowing = true;

            tooltipText.text = message;
            tooltipRoot.gameObject.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }

        public void Hide()
        {
            isShowing = false;

            StopAllCoroutines();
            StartCoroutine(FadeOut());
        }

        private System.Collections.IEnumerator FadeIn()
        {
            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }

        private System.Collections.IEnumerator FadeOut()
        {
            while (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }

            tooltipRoot.gameObject.SetActive(false);
        }
    }
}

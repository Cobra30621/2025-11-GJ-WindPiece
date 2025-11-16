using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace UI.Main
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [TextArea]
        public string message;

        private Coroutine showCoroutine;

        public void OnPointerEnter(PointerEventData eventData)
        {
            // 如果已經在等待，就不重新啟動
            if (showCoroutine == null)
            {
                showCoroutine = StartCoroutine(ShowAfterDelay());
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // 離開時取消等待
            if (showCoroutine != null)
            {
                StopCoroutine(showCoroutine);
                showCoroutine = null;
            }

            TooltipManager.Instance.Hide();
        }

        private IEnumerator ShowAfterDelay()
        {
            yield return new WaitForSeconds(0.5f);
            TooltipManager.Instance.Show(message);

            // 觸發後清空 coroutine，避免誤判
            showCoroutine = null;
        }
    }
}
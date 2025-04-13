using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class NotificationItem : MonoBehaviour
{
    public TextMeshProUGUI notificationTitle;
    public TextMeshProUGUI notificationDetial;
    public CanvasGroup canvasGroup;

    public void Initialize(string notificationTitleIn, string notificationDetailIn, float displayTime)
    {
        notificationTitle.text = notificationTitleIn;
        notificationDetial.text = notificationDetailIn;
        StartCoroutine(FadeRoutine(displayTime));
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

    }

    private IEnumerator FadeRoutine(float delay)
    {
        canvasGroup.alpha = 0;
        // 淡入
        for (float t = 0; t < 1f; t += Time.deltaTime * 3f)
        {
            canvasGroup.alpha = t;
            yield return null;
        }
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(delay);

        // 淡出
        for (float t = 1f; t > 0; t -= Time.deltaTime * 2f)
        {
            canvasGroup.alpha = t;
            yield return null;
        }
        Destroy(gameObject);
    }
}

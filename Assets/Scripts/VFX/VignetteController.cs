using System.Collections;
using UnityEngine;

public class VignetteController : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f; // Arranca apagado
    }

    public void TriggerVignette(bool open, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(open ? VignetteFadeIn(duration) : VignetteFadeOut(duration));
    }

    private IEnumerator VignetteFadeIn(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator VignetteFadeOut(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}

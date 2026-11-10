using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMessageManager : Singleton<GameMessageManager>
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;

    [Header("Banner")]
    [SerializeField] private RectTransform bannerRect;

    [Header("Timing")]
    [Min(0f)] [SerializeField] private float fadeInDuration = 0.3f;
    [Min(0f)] [SerializeField] private float displayDuration = 3.5f;
    [Min(0f)] [SerializeField] private float fadeOutDuration = 0.5f;

    [Header("Typewriter")]
    [SerializeField] private TypewriterEffect typewriter;

    [Header("Keyword Animation Handler")]
    [SerializeField] private KeywordSequenceController keywordSequenceHandler;

    private Coroutine _activeCoroutine;
    private System.Action _pendingOnClosed;

    public bool IsOpen => canvasGroup.blocksRaycasts;

    private void Start()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        bannerRect.gameObject.SetActive(false);
    }

    public void Show(GameMessage message, float? customDisplayTime = null, bool dismissOnClick = false, System.Action onClosed = null) =>
        Show(message.title ?? string.Empty, message.body ?? string.Empty, customDisplayTime, dismissOnClick, onClosed);

    public void Show(string title, string body, float? customDisplayTime = null, bool dismissOnClick = false, System.Action onClosed = null)
    {
        if (_activeCoroutine != null)
        {
            StopCoroutine(_activeCoroutine);
            FlushPendingOnClosed();
        }

        _pendingOnClosed = onClosed;
        _activeCoroutine = StartCoroutine(ShowRoutine(title, body, customDisplayTime ?? displayDuration, dismissOnClick));
    }

    private void FlushPendingOnClosed()
    {
        System.Action callback = _pendingOnClosed;
        _pendingOnClosed = null;
        callback?.Invoke();
    }

    private IEnumerator ShowRoutine(string title, string body, float duration, bool dismissOnClick)
    {
        List<string> keywords = KeywordTagParser.Extract(body, out string cleanText);

        if (titleText != null) titleText.text = title;
        bodyText.text = cleanText;
        bodyText.maxVisibleCharacters = 0;
        canvasGroup.blocksRaycasts = true;
        bannerRect.gameObject.SetActive(true);

        yield return Fade(0f, 1f, fadeInDuration);
        yield return typewriter.Play(bodyText, dismissOnClick ? IsBannerClicked : null);

        if (dismissOnClick) yield return WaitForBannerClick();
        else yield return new WaitForSeconds(duration);

        if (keywords.Count == 0 || keywordSequenceHandler == null)
        {
            yield return Fade(1f, 0f, fadeOutDuration);
        }
        else
        {
            yield return keywordSequenceHandler.PlaySequence(
                bodyText, 
                bannerRect.parent, 
                keywords, 
                onBannerFadeRequested: () => StartCoroutine(Fade(1f, 0f, fadeOutDuration))
            );
        }

        bannerRect.gameObject.SetActive(false);
        canvasGroup.blocksRaycasts = false;
        _activeCoroutine = null;
        FlushPendingOnClosed();
    }

    private IEnumerator WaitForBannerClick()
    {
        yield return null;
        while (!IsBannerClicked())
            yield return null;
    }

    private bool IsBannerClicked()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || !mouse.leftButton.wasPressedThisFrame) return false;
        return RectTransformUtility.RectangleContainsScreenPoint(bannerRect, mouse.position.ReadValue());
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (duration <= 0f) { canvasGroup.alpha = to; yield break; }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}

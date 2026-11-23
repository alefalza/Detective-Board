// Pantalla en negro con un texto que aparece solo la primera vez que se abre el menú en la sesión.
// Al tocar cualquier tecla, clic o la pantalla, se desvanece y deja ver el menú.
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class IntroScreen : MonoBehaviour
{
    [SerializeField] private IntroScreenSettings settings;

    [Header("Referencias de UI")]
    [Tooltip("Fondo negro a pantalla completa, encima del menú.")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private TMP_Text promptText;
    [Tooltip("Logo que aparece entre el texto y el menú. Opcional: si queda vacío, se saltea ese paso.")]
    [SerializeField] private CanvasGroup logo;

    // Se mantiene mientras el juego esté abierto: al volver del epílogo, el menú aparece directo
    private static bool _shownThisSession;

    // Con Enter Play Mode Options (sin recarga de dominio), el estático no se reinicia solo
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSession() => _shownThisSession = false;

    private void Start()
    {
        if (_shownThisSession)
        {
            gameObject.SetActive(false);
            return;
        }

        _shownThisSession = true;
        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true; // el menú de abajo no recibe clics

        bodyText.text = settings.Text;
        bodyText.alpha = 0f;
        promptText.text = settings.PromptText;
        promptText.alpha = 0f;
        if (logo != null) logo.alpha = 0f;

        Coroutine bodyFadeIn = StartCoroutine(FadeText(bodyText, 0f, 1f, settings.TextDelay, settings.TextFadeInDuration));

        yield return new WaitForSecondsRealtime(settings.PromptDelay);

        // Solo se puede continuar con el aviso ya visible
        float blinkTime = 0f;
        while (!WasAnyInputPressed())
        {
            blinkTime += Time.unscaledDeltaTime;
            promptText.alpha = GetPromptAlpha(blinkTime);
            yield return null;
        }

        // Salida: se apaga el aviso, se desvanece el texto, aparece y se va el logo, y por último el fondo
        promptText.alpha = 0f;

        StopCoroutine(bodyFadeIn);
        yield return FadeText(bodyText, bodyText.alpha, 0f, 0f, settings.BodyFadeOutDuration);

        if (logo != null)
        {
            yield return new WaitForSecondsRealtime(settings.LogoDelay);
            yield return FadeGroup(logo, 0f, 1f, settings.LogoFadeInDuration);
            yield return new WaitForSecondsRealtime(settings.LogoHoldDuration);
            yield return FadeGroup(logo, 1f, 0f, settings.LogoFadeOutDuration);
        }

        yield return new WaitForSecondsRealtime(settings.BlackHoldDuration);
        yield return FadeGroup(canvasGroup, 1f, 0f, settings.FadeOutDuration);

        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }

    private float GetPromptAlpha(float time)
    {
        if (settings.PromptBlinkSpeed <= 0f) return 1f;

        float pulse = (Mathf.Cos(time * settings.PromptBlinkSpeed * Mathf.PI) + 1f) * 0.5f;
        return Mathf.Lerp(settings.PromptMinAlpha, 1f, pulse);
    }

    private static IEnumerator FadeText(TMP_Text label, float from, float to, float delay, float duration)
    {
        if (delay > 0f) yield return new WaitForSecondsRealtime(delay);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            label.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        label.alpha = to;
    }

    private static IEnumerator FadeGroup(CanvasGroup group, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        group.alpha = to;
    }

    // Cualquier tecla, botón del mouse o toque de pantalla
    private static bool WasAnyInputPressed()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.anyKey.wasPressedThisFrame) return true;

        Mouse mouse = Mouse.current;
        if (mouse != null && (mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame)) return true;

        Touchscreen touch = Touchscreen.current;
        return touch != null && touch.primaryTouch.press.wasPressedThisFrame;
    }
}

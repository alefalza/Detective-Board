// Prólogo al empezar la partida: la escena arranca en negro, se escribe el texto introductorio,
// se desvanece y después se revela el juego. Mientras dura, no se puede interactuar.
using System.Collections;
using UnityEngine;

public class PrologueScreen : BlackScreenSequence
{
    [Header("Prólogo")]
    [SerializeField] private PrologueSettings settings;
    [Tooltip("Desactivar para probar la escena sin el prólogo.")]
    [SerializeField] private bool playOnStart = true;

    [Header("Ambiente")]
    [Tooltip("Sonidos de la escena que arrancan con el fundido al juego (ej. reloj, ventilador, radio). Sin Play On Awake.")]
    [SerializeField] private AudioSource[] ambienceSources;

    private void Start()
    {
        if (!playOnStart)
        {
            SetBackground(0f, false);
            foreach (AudioSource source in ambienceSources)
            {
                if (source != null) source.Play();
            }
            return;
        }

        SetBackground(1f, true);
        GameViewManager.Instance.BeginSequence();
        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        yield return new WaitForSeconds(settings.TextDelay);
        yield return PlayText(settings.Text, settings.Voice, settings.HoldDuration);
        yield return FadeOutText(settings.TextFadeDuration);
        yield return new WaitForSeconds(settings.BlackHoldDuration);

        // El ambiente sube junto con la imagen
        foreach (AudioSource source in ambienceSources)
        {
            if (source != null) StartCoroutine(FadeInAudio(source, settings.BackgroundFadeDuration, settings.BackgroundFadeCurve));
        }

        yield return FadeBackground(0f, settings.BackgroundFadeDuration, settings.BackgroundFadeCurve);

        SetBackground(0f, false);
        GameViewManager.Instance.EndSequence();
    }

    // Sigue la misma curva que la imagen, para que el sonido aparezca junto con la escena
    private static IEnumerator FadeInAudio(AudioSource source, float duration, AnimationCurve curve)
    {
        float targetVolume = source.volume;
        float elapsed = 0f;

        source.volume = 0f;
        source.Play();

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            source.volume = targetVolume * curve.Evaluate(Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        source.volume = targetVolume;
    }
}

using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Fundido a negro y epílogo según el tono elegido.
/// </summary>
public class EndingScreen : MonoBehaviour
{
    [SerializeField] private ArticleFinaleSettings settings;
    [Tooltip("Fondo negro a pantalla completa, encima de todo el HUD.")]
    [SerializeField] private CanvasGroup canvasGroup;
    [Tooltip("Su TextMeshPro debe arrancar vacío en el editor.")]
    [SerializeField] private TypewriterText epilogueText;
    [SerializeField] private AudioSource voiceSource;
    [Tooltip("Sonidos de la escena que se apagan durante el fundido a negro (ej. la radio).")]
    [SerializeField] private AudioSource[] fadeOutSources;

    private const string FadeTweenName = "EndingFade";

    private TMP_Text _epilogueLabel;

    private void Awake() => _epilogueLabel = epilogueText.GetComponent<TMP_Text>();

    private void Start()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public void Play(ToneData tone) => StartCoroutine(PlayRoutine(tone));

    private IEnumerator PlayRoutine(ToneData tone)
    {
        canvasGroup.blocksRaycasts = true;

        foreach (AudioSource source in fadeOutSources)
        {
            if (source != null) StartCoroutine(FadeOutAudio(source, settings.EndingFadeDuration));
        }

        iTween.ValueTo(gameObject, iTween.Hash(
            "name", FadeTweenName,
            "from", 0f,
            "to", 1f,
            "time", settings.EndingFadeDuration,
            "ignoretimescale", true,
            "onupdate", nameof(OnFadeUpdate)
        ));

        yield return new WaitForSeconds(settings.EndingFadeDuration + settings.EpilogueTextDelay);

        if (tone.EpilogueVoice != null) voiceSource.PlayOneShot(tone.EpilogueVoice);
        epilogueText.Play(tone.EpilogueText);

        // Queda en pantalla unos segundos y se desvanece en el negro
        yield return new WaitWhile(() => epilogueText.IsPlaying);
        yield return new WaitForSeconds(settings.EpilogueHoldDuration);
        yield return FadeOutText(settings.EpilogueTextFadeDuration);
    }

    private IEnumerator FadeOutText(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            _epilogueLabel.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        _epilogueLabel.alpha = 0f;
    }

    private void OnFadeUpdate(float alpha) => canvasGroup.alpha = alpha;

    // Baja el volumen a 0 junto con el fundido a negro y detiene la fuente
    private static IEnumerator FadeOutAudio(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }
}

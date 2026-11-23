/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: fundido a negro, epílogo según el tono y vuelta al menú.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingScreen : BlackScreenSequence
{
    [Header("Epilogue")]
    [SerializeField] private ArticleFinaleSettings settings;
    [SerializeField] private AudioSource[] fadeOutSources;
    [SerializeField] private string menuSceneName = "TitleScreenScene";

    private void Start() => SetBackground(0f, false);

    public void Play(ToneData tone) => StartCoroutine(PlayRoutine(tone));

    private IEnumerator PlayRoutine(ToneData tone)
    {
        canvasGroup.blocksRaycasts = true;

        foreach (AudioSource source in fadeOutSources)
        {
            if (source != null) StartCoroutine(FadeOutAudio(source, settings.EndingFadeDuration));
        }

        yield return FadeBackground(1f, settings.EndingFadeDuration);
        yield return new WaitForSeconds(settings.EpilogueTextDelay);

        yield return PlayText(tone.EpilogueText, tone.EpilogueVoice, settings.EpilogueHoldDuration);
        yield return FadeOutText(settings.EpilogueTextFadeDuration);

        yield return new WaitForSeconds(settings.ReturnToMenuDelay);
        SceneManager.LoadScene(menuSceneName, LoadSceneMode.Single);
    }

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

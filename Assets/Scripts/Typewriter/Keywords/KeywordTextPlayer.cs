/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: reproduce un texto con efecto de máquina de escribir y luego su secuencia de keywords.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeywordTextPlayer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text text;
    [SerializeField] private RectTransform chipParent;
    [SerializeField] private KeywordSequenceController keywordSequence;

    [Header("Typewriter")]
    [SerializeField] private TypewriterEffect typewriter;

    [Header("Timing")]
    [Min(0f)] [SerializeField] private float holdDuration = 2f;
    [SerializeField] private bool fadeOutOnEnd = false;
    [Min(0f)] [SerializeField] private float fadeOutDuration = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource onKeywordsPopAudioSource;

    private Coroutine _routine;

    public bool IsPlaying => _routine != null;

    public void Play(string taggedBody, Action onFinished = null, Func<bool> skipRequested = null, Action onTypingFinished = null)
    {
        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(PlayRoutine(taggedBody ?? string.Empty, onFinished, skipRequested, onTypingFinished));
    }

    private IEnumerator PlayRoutine(string taggedBody, Action onFinished, Func<bool> skipRequested, Action onTypingFinished)
    {
        List<string> keywords = KeywordTagParser.Extract(taggedBody, out string cleanText);

        text.text = cleanText;
        text.alpha = 1f;
        text.maxVisibleCharacters = 0;

        yield return typewriter.Play(text, skipRequested);
        onTypingFinished?.Invoke();

        yield return new WaitForSeconds(holdDuration);

        if (keywords.Count == 0 || keywordSequence == null)
        {
            if (fadeOutOnEnd) yield return FadeOut();
        }
        else
        {
            if (onKeywordsPopAudioSource != null) onKeywordsPopAudioSource.Play();

            yield return keywordSequence.PlaySequence(
                text,
                chipParent,
                keywords,
                onBannerFadeRequested: () =>
                {
                    if (fadeOutOnEnd) StartCoroutine(FadeOut());
                }
            );
        }

        _routine = null;
        onFinished?.Invoke();
    }

    private IEnumerator FadeOut()
    {
        if (fadeOutDuration <= 0f) { text.alpha = 0f; yield break; }

        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            text.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            yield return null;
        }
        text.alpha = 0f;
    }
}

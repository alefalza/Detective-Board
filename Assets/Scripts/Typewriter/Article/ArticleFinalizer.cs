/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: pasa en limpio el artículo resuelto, permite elegir el tono y lo manda a imprenta.
 */

using System.Collections;
using UnityEngine;

public class ArticleFinalizer : MonoBehaviour
{
    [SerializeField] private ArticleFinaleSettings settings;
    [SerializeField] private ArticleData article;
    [SerializeField] private ArticleValidator validator;

    [Header("Paper")]
    [SerializeField] private RectTransform paper;
    [SerializeField] private RectTransform articleContent;
    [SerializeField] private TypewriterText articleText;
    [SerializeField] private GameObject slotsContainer;
    [SerializeField] private TypewriterPanels_UI panelsUI;
    [SerializeField] private AudioSource impactAudioSource;

    [Header("Close")]
    [SerializeField] private ToneSelector_UI toneSelector;
    [SerializeField] private EndingScreen endingScreen;

    private const string ContentTweenName = "ArticleContentScale";
    private const string ImpactTweenName = "ArticleImpactPunch";

    private ToneData _selectedTone;

    private void OnEnable()
    {
        validator.Solved += OnSolved;
        toneSelector.ToneSelected += SelectTone;
        toneSelector.PrintRequested += OnPrintRequested;
    }

    private void OnDisable()
    {
        validator.Solved -= OnSolved;
        toneSelector.ToneSelected -= SelectTone;
        toneSelector.PrintRequested -= OnPrintRequested;
    }

    private void OnSolved() => StartCoroutine(CleanCopyRoutine());

    private IEnumerator CleanCopyRoutine()
    {
        GameViewManager.Instance.BeginSequence();

        yield return new WaitForSeconds(settings.CleanCopyDelay);

        panelsUI.Slide(false);

        Vector3 restScale = articleContent.localScale;
        yield return ScaleContent(restScale * settings.LiftScale, settings.LiftDuration, settings.LiftEase);

        slotsContainer.SetActive(false);
        _selectedTone = article.DefaultTone;
        articleText.Show(article.GetText(_selectedTone));

        yield return ScaleContent(restScale, settings.SlamDuration, settings.SlamEase);
        PlayImpact();

        toneSelector.Show(_selectedTone);
    }

    private IEnumerator ScaleContent(Vector3 target, float time, iTween.EaseType ease)
    {
        iTween.ScaleTo(articleContent.gameObject, iTween.Hash(
            "name", ContentTweenName,
            "scale", target,
            "time", time,
            "easetype", ease,
            "islocal", true,
            "ignoretimescale", true
        ));

        yield return new WaitForSecondsRealtime(time);
        articleContent.localScale = target;
    }

    private void PlayImpact()
    {
        iTween.PunchScale(paper.gameObject, iTween.Hash(
            "name", ImpactTweenName,
            "amount", settings.ImpactPunchAmount,
            "time", settings.ImpactPunchTime,
            "ignoretimescale", true
        ));

        if (settings.ImpactClip != null) impactAudioSource.PlayOneShot(settings.ImpactClip);
    }

    private void SelectTone(ToneData tone)
    {
        _selectedTone = tone;
        articleText.Play(article.GetText(tone));
    }

    private void OnPrintRequested() => StartCoroutine(PrintRoutine());

    private IEnumerator PrintRoutine()
    {
        toneSelector.Lock();
        articleText.Complete();

        yield return new WaitForSeconds(settings.PrintDelay);

        endingScreen.Play(_selectedTone);
    }
}

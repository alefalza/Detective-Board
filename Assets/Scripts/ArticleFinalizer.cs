using System.Collections;
using UnityEngine;

/// <summary>
/// Cierre del artículo: al resolverse lo pasa en limpio, deja elegir el tono y lo manda a imprenta.
/// </summary>
public class ArticleFinalizer : MonoBehaviour
{
    [SerializeField] private ArticleFinaleSettings settings;
    [SerializeField] private ArticleData article;
    [SerializeField] private ArticleValidator validator;

    [Header("Papel")]
    [Tooltip("Hoja sobre la que cae el artículo: recibe el golpe del impacto.")]
    [SerializeField] private RectTransform paper;
    [Tooltip("Padre del texto y de los slots: es lo que se levanta y cae.")]
    [SerializeField] private RectTransform articleContent;
    [Tooltip("TypewriterText sobre el texto del artículo.")]
    [SerializeField] private TypewriterText articleText;
    [Tooltip("Contenedor de los slots del artículo: se oculta, con sus chips, al pasar en limpio.")]
    [SerializeField] private GameObject slotsContainer;
    [SerializeField] private TypewriterPanels_UI panelsUI;
    [SerializeField] private AudioSource impactAudioSource;

    [Header("Cierre")]
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
        // Con el artículo resuelto no se sale del Paper: la secuencia bloquea todo cambio de vista
        GameViewManager.Instance.BeginSequence();

        // Deja terminar el drop del último chip antes de animar
        yield return new WaitForSeconds(settings.CleanCopyDelay);

        panelsUI.Slide(false);

        // Se levanta junto con los chips
        Vector3 restScale = articleContent.localScale;
        yield return ScaleContent(restScale * settings.LiftScale, settings.LiftDuration, settings.LiftEase);

        // En el punto más alto, las keywords se funden con el texto
        slotsContainer.SetActive(false);
        _selectedTone = article.DefaultTone;
        articleText.Show(article.GetText(_selectedTone));

        // Y cae de golpe sobre la hoja
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

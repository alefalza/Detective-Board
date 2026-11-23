/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: selector de tono con sus sellos y el botón de imprenta.
 */

using System;
using UnityEngine;

public class ToneSelector_UI : MonoBehaviour
{
    [SerializeField] private ArticleFinaleSettings settings;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private ButtonPivot_HUD printButton;

    private const string FadeTweenName = "ToneSelectorFade";

    private ToneStamp_UI[] _stamps;
    private ToneData _currentTone;

    public event Action<ToneData> ToneSelected;
    public event Action PrintRequested;

    private void Awake()
    {
        _stamps = GetComponentsInChildren<ToneStamp_UI>(true);
        foreach (ToneStamp_UI stamp in _stamps) stamp.Clicked += OnStampClicked;

        canvasGroup.alpha = 0f;
        SetInteractive(false);
    }

    public void Show(ToneData initialTone)
    {
        Highlight(initialTone);
        SetInteractive(true);
        printButton.SetVisible(true);

        iTween.StopByName(gameObject, FadeTweenName);
        iTween.ValueTo(gameObject, iTween.Hash(
            "name", FadeTweenName,
            "from", canvasGroup.alpha,
            "to", 1f,
            "time", settings.SelectorFadeDuration,
            "ignoretimescale", true,
            "onupdate", nameof(OnFadeUpdate)
        ));
    }

    public void Lock()
    {
        SetInteractive(false);
        printButton.SetVisible(false);
    }

    public void OnPrintClicked()
    {
        if (canvasGroup.interactable) PrintRequested?.Invoke();
    }

    private void OnStampClicked(ToneStamp_UI stamp)
    {
        if (!canvasGroup.interactable || stamp.Tone == _currentTone) return;

        Highlight(stamp.Tone);
        ToneSelected?.Invoke(stamp.Tone);
    }

    private void Highlight(ToneData tone)
    {
        _currentTone = tone;
        foreach (ToneStamp_UI stamp in _stamps) stamp.SetSelected(stamp.Tone == tone);
    }

    private void SetInteractive(bool interactive)
    {
        canvasGroup.interactable = interactive;
        canvasGroup.blocksRaycasts = interactive;
    }

    private void OnFadeUpdate(float alpha) => canvasGroup.alpha = alpha;

    private void OnDestroy()
    {
        foreach (ToneStamp_UI stamp in _stamps) stamp.Clicked -= OnStampClicked;
    }
}

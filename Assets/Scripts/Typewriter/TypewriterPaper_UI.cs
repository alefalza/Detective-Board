/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: anima la apertura y el cierre de la hoja central.
 */

using UnityEngine;

public class TypewriterPaper_UI : MonoBehaviour
{
    [SerializeField] private RectTransform centerPanelRect;

    [Header("Paper Canvas Group")]
    [SerializeField] private CanvasGroup centerCanvasGroup;

    [SerializeField] private float animationDuration = 0.5f;

    [Header("Paper Anim Settings")]
    [SerializeField] private iTween.EaseType fadeInEase = iTween.EaseType.easeInCubic;
    [SerializeField] private iTween.EaseType fadeOutEase = iTween.EaseType.easeOutCubic;
    [SerializeField] private iTween.EaseType scaleInEase = iTween.EaseType.easeInCubic;
    [SerializeField] private iTween.EaseType scaleOutEase = iTween.EaseType.easeOutCubic;
    [SerializeField] private Vector3 centerOpenScale = Vector3.one;
    [SerializeField] private Vector3 centerClosedScale = Vector3.zero;

    private const string CenterTweenName = "TypewriterCenterFade";
    private const string CenterScaleTweenName = "TypewriterCenterScale";

    public void ScaleCenter(bool open)
    {
        if (centerPanelRect == null) return;

        iTween.StopByName(centerPanelRect.gameObject, CenterScaleTweenName);

        iTween.ScaleTo(centerPanelRect.gameObject, iTween.Hash(
            "name", CenterScaleTweenName,
            "scale", open ? centerOpenScale : centerClosedScale,
            "time", animationDuration,
            "easetype", open ? scaleInEase : scaleOutEase,
            "islocal", true
        ));
    }

    public void FadeCenter(bool open)
    {
        if (centerCanvasGroup == null) return;

        iTween.StopByName(centerCanvasGroup.gameObject, CenterTweenName);

        if (open)
        {
            centerCanvasGroup.interactable = true;
            centerCanvasGroup.blocksRaycasts = true;
        }

        iTween.ValueTo(centerCanvasGroup.gameObject, iTween.Hash(
            "name", CenterTweenName,
            "from", centerCanvasGroup.alpha,
            "to", open ? 1f : 0f,
            "time", animationDuration,
            "easetype", open ? fadeInEase : fadeOutEase,
            "onupdate", nameof(OnCenterFadeUpdate),
            "onupdatetarget", gameObject,
            "oncomplete", open ? nameof(OnCenterFadeInComplete) : nameof(OnCenterFadeOutComplete),
            "oncompletetarget", gameObject
        ));
    }

    private void OnCenterFadeUpdate(float alpha)
    {
        centerCanvasGroup.alpha = alpha;
    }

    private void OnCenterFadeInComplete()
    {
        centerCanvasGroup.alpha = 1f;
    }

    private void OnCenterFadeOutComplete()
    {
        centerCanvasGroup.alpha = 0f;
        centerCanvasGroup.interactable = false;
        centerCanvasGroup.blocksRaycasts = false;
    }
}

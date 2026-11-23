/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: ajusta con un fade las luces 2D de la escena según la vista activa.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnvironmentLightingManager : Singleton<EnvironmentLightingManager>
{
    [Header("Corkboard Lights")]
    [SerializeField] private Light2D corkboardMainLight;
    [SerializeField] private float corkboardLightTargetIntensity = 0.8f;

    [Header("Desk Lights")]
    [SerializeField] private Light2D deskLampSpotlight;
    [SerializeField] private float deskLampTargetIntensity = 1.3f;
    [SerializeField] private Light2D deskPaperBounceLight;
    [SerializeField] private float deskPaperBounceTargetIntensity = 0.3f;

    [Header("Transition Settings")]
    [SerializeField] private float transitionDuration = 1.0f;

    private Coroutine _currentFadeCoroutine;

    private void Start()
    {
        SetLightingForCorkboard(instant: true);
    }

    public void SetLightingForCorkboard(bool instant = false)
    {
        StartLightingTransition(corkboardLightTargetIntensity, 0.2f, 0.05f, instant);
    }

    public void SetLightingForTypewriter(bool instant = false)
    {
        StartLightingTransition(0.3f, deskLampTargetIntensity, 0.05f, instant);
    }
    
    public void SetLightingForPaper(bool instant = false)
    {
        StartLightingTransition(0.3f, 0.2f, deskPaperBounceTargetIntensity, instant);
    }

    private void StartLightingTransition(float targetCorkIntensity, float targetDeskIntensity, float targetPaperIntensity, bool instant)
    {
        if (_currentFadeCoroutine != null) StopCoroutine(_currentFadeCoroutine);

        if (instant)
        {
            if (corkboardMainLight != null) corkboardMainLight.intensity = targetCorkIntensity;
            if (deskLampSpotlight != null) deskLampSpotlight.intensity = targetDeskIntensity;
            if (deskPaperBounceLight != null) deskPaperBounceLight.intensity = targetPaperIntensity;
        }
        else
        {
            _currentFadeCoroutine = StartCoroutine(FadeLightingRoutine(targetCorkIntensity, targetDeskIntensity, targetPaperIntensity));
        }
    }

    private IEnumerator FadeLightingRoutine(float targetCork, float targetDesk, float targetPaper)
    {
        float elapsed = 0f;
        float startCork = corkboardMainLight != null ? corkboardMainLight.intensity : 0f;
        float startDesk = deskLampSpotlight != null ? deskLampSpotlight.intensity : 0f;
        float startPaper = deskPaperBounceLight != null ? deskPaperBounceLight.intensity : 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);

            if (corkboardMainLight != null)
                corkboardMainLight.intensity = Mathf.Lerp(startCork, targetCork, t);

            if (deskLampSpotlight != null)
                deskLampSpotlight.intensity = Mathf.Lerp(startDesk, targetDesk, t);

            if (deskPaperBounceLight != null)
                deskPaperBounceLight.intensity = Mathf.Lerp(startPaper, targetPaper, t);

            yield return null;
        }
    }
}

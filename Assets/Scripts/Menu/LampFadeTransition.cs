/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: al apretar “Jugar”, hace parpadear la lámpara del menú, apaga la pantalla cerrándose hacia ella y carga la escena de juego.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LampFadeTransition : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image blackoutImage;

    [Header("Settings")]
    [SerializeField] private Vector2 lampCenter = new Vector2(0.266f, 0.87f);
    [SerializeField] private float startRadius = 1.8f;
    [Range(0f, 1f)]
    [SerializeField] private float softness = 0.35f;
    [SerializeField] private float fadeDuration = 1.2f;

    [Header("Flicker")]
    [SerializeField] private bool useFlicker = true;
    [SerializeField] private int flickerCount = 3;
    [SerializeField] private Vector2 flickerDimRange = new Vector2(0.4f, 0.85f);
    [SerializeField] private Vector2 flickerOffTime = new Vector2(0.03f, 0.08f);
    [SerializeField] private Vector2 flickerOnTime = new Vector2(0.05f, 0.15f);
    [SerializeField] private string nextSceneName = "GameScene";

    private static readonly int CenterId = Shader.PropertyToID("_Center");
    private static readonly int RadiusId = Shader.PropertyToID("_Radius");
    private static readonly int SoftnessId = Shader.PropertyToID("_Softness");
    private static readonly int DimId = Shader.PropertyToID("_Dim");

    private Material runtimeMaterial;
    private bool isTransitioning;

    private void Awake()
    {
        runtimeMaterial = new Material(blackoutImage.material);
        blackoutImage.material = runtimeMaterial;

        runtimeMaterial.SetVector(CenterId, lampCenter);
        runtimeMaterial.SetFloat(SoftnessId, softness);
        runtimeMaterial.SetFloat(RadiusId, startRadius);
        runtimeMaterial.SetFloat(DimId, 0f);

        blackoutImage.raycastTarget = false;
    }

    public void PlayGameButton()
    {
        if (isTransitioning) return;

        isTransitioning = true;
        blackoutImage.raycastTarget = true;
        StartCoroutine(LampOffRoutine());
    }

    private IEnumerator LampOffRoutine()
    {
        if (useFlicker)
        {
            yield return FlickerRoutine();
        }

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);

            t = t * t;

            runtimeMaterial.SetFloat(RadiusId, Mathf.Lerp(startRadius, 0f, t));

            yield return null;
        }

        runtimeMaterial.SetFloat(RadiusId, 0f);

        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator FlickerRoutine()
    {
        for (int i = 0; i < flickerCount; i++)
        {
            runtimeMaterial.SetFloat(DimId, Random.Range(flickerDimRange.x, flickerDimRange.y));
            yield return new WaitForSeconds(Random.Range(flickerOffTime.x, flickerOffTime.y));

            runtimeMaterial.SetFloat(DimId, 0f);
            yield return new WaitForSeconds(Random.Range(flickerOnTime.x, flickerOnTime.y));
        }
    }

    private void OnDestroy()
    {
        if (runtimeMaterial != null) Destroy(runtimeMaterial);
    }
}

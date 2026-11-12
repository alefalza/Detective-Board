using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LampFadeTransition : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Image full-screen (stretch), color negro, con el material UI/LampBlackout")]
    [SerializeField] private Image blackoutImage;

    [Header("Configuración")]
    [Tooltip("Posición de la lámpara en UV de pantalla (0,0 = abajo-izq / 1,1 = arriba-der)")]
    [SerializeField] private Vector2 lampCenter = new Vector2(0.266f, 0.87f);
    [Tooltip("Radio inicial. Debe ser suficiente para que el agujero cubra toda la pantalla")]
    [SerializeField] private float startRadius = 1.8f;
    [Tooltip("Ancho del borde difuso como proporción del radio (0-1)")]
    [Range(0f, 1f)]
    [SerializeField] private float softness = 0.35f;
    [SerializeField] private float fadeDuration = 1.2f;

    [Header("Flicker")]
    [SerializeField] private bool useFlicker = true;
    [SerializeField] private int flickerCount = 3;
    [Tooltip("Cuánto se oscurece la luz en cada parpadeo (0 = nada, 1 = negro)")]
    [SerializeField] private Vector2 flickerDimRange = new Vector2(0.4f, 0.85f);
    [Tooltip("Duración de cada apagón (segundos)")]
    [SerializeField] private Vector2 flickerOffTime = new Vector2(0.03f, 0.08f);
    [Tooltip("Pausa con luz entre parpadeos (segundos)")]
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
        // Instancia propia para no modificar el asset del material en disco
        runtimeMaterial = new Material(blackoutImage.material);
        blackoutImage.material = runtimeMaterial;

        runtimeMaterial.SetVector(CenterId, lampCenter);
        runtimeMaterial.SetFloat(SoftnessId, softness);
        runtimeMaterial.SetFloat(RadiusId, startRadius); // Arranca con la pantalla totalmente visible
        runtimeMaterial.SetFloat(DimId, 0f);

        blackoutImage.raycastTarget = false;
    }

    /// <summary>
    /// Método que vinculas al evento OnClick() de tu botón Jugar
    /// </summary>
    public void PlayGameButton()
    {
        if (isTransitioning) return;

        isTransitioning = true;
        blackoutImage.raycastTarget = true; // Bloquea clics durante la transición
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

            // Curva suave (Ease In) para que acelere al apagarse como un tubo viejo
            t = t * t;

            runtimeMaterial.SetFloat(RadiusId, Mathf.Lerp(startRadius, 0f, t));

            yield return null;
        }

        runtimeMaterial.SetFloat(RadiusId, 0f); // Pantalla 100% negra

        // Cargar la siguiente escena una vez oscurecido por completo
        SceneManager.LoadScene(nextSceneName);
    }

    /// <summary>
    /// Parpadeos cortos antes de apagarse, como un tubo que falla
    /// </summary>
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

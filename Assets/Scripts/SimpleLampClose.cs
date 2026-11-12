using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleLampClose : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí la UI Image negra")]
    [SerializeField] private RectTransform blackPanel;

    [Header("Configuración")]
    [SerializeField] private float duration = 1.0f;
    [SerializeField] private string nextScene = "GameScene";

    private void Start()
    {
        if (blackPanel != null)
        {
            // Arranca invisible (escala en 0 o transparente)
            // O si prefieres que empiece cubriendo todo y se abra:
            blackPanel.localScale = Vector3.zero; 
        }
    }

    public void TriggerLampOff()
    {
        StartCoroutine(CloseRoutine());
    }

    private IEnumerator CloseRoutine()
    {
        float elapsed = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one; // Escala 1 cubre toda la pantalla

        // Si quieres que la pantalla se VUELVA NEGRA contrayéndose hacia la lámpara:
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Curva de aceleración para que parezca un golpe de luz
            t = t * t; 

            blackPanel.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        blackPanel.localScale = endScale;

        // Carga el juego
        SceneManager.LoadScene(nextScene);
    }
}

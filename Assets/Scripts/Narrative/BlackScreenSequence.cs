// Base para las pantallas de pleno negro con texto (prólogo y epílogo): fundidos del fondo y del texto,
// y escritura con efecto de máquina de escribir y voz en off opcional.
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class BlackScreenSequence : MonoBehaviour
{
    [Header("Pantalla")]
    [Tooltip("Fondo negro a pantalla completa, encima de todo el HUD.")]
    [SerializeField] protected CanvasGroup canvasGroup;
    [Tooltip("Su TextMeshPro debe estar en el mismo objeto y arrancar vacío en el editor.")]
    [FormerlySerializedAs("epilogueText")]
    [SerializeField] protected TypewriterText typewriterText;
    [SerializeField] protected AudioSource voiceSource;

    private TMP_Text _label;

    protected virtual void Awake() => _label = typewriterText.Text;
    protected void SetBackground(float alpha, bool blockInput)
    {
        canvasGroup.alpha = alpha;
        canvasGroup.blocksRaycasts = blockInput;
    }

    // Sin curva, el fundido es lineal. La curva va de 0 a 1 en ambos ejes: tiempo y avance del fundido
    protected IEnumerator FadeBackground(float to, float duration, AnimationCurve curve = null)
    {
        float from = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = Mathf.LerpUnclamped(from, to, curve != null ? curve.Evaluate(t) : t);
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    // Escribe el texto (con voz si hay) y lo deja en pantalla 'holdDuration' segundos
    protected IEnumerator PlayText(string content, AudioClip voice, float holdDuration)
    {
        _label.alpha = 1f;

        if (voice != null) voiceSource.PlayOneShot(voice);
        typewriterText.Play(content);

        yield return new WaitWhile(() => typewriterText.IsPlaying);
        yield return new WaitForSeconds(holdDuration);
    }

    protected IEnumerator FadeOutText(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            _label.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        _label.alpha = 0f;
    }
}

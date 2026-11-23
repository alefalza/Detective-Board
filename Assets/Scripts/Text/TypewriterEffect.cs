using System;
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Efecto de máquina de escribir sobre cualquier TMP_Text (TextMeshProUGUI o TextMeshPro).
/// Serializable: se puede embeber como campo en cualquier MonoBehaviour y configurar desde el inspector.
/// </summary>
[Serializable]
public class TypewriterEffect
{
    [Min(0f)] [SerializeField] private float charDelay = 0.03f;
    [Range(0f, 1f)] [SerializeField] private float charDelayJitter = 0.5f;
    [Min(0f)] [SerializeField] private float punctuationPause = 0.25f;

    /// <summary>
    /// Revela el texto caracter por caracter. El texto ya debe estar asignado.
    /// Si se pasa skipRequested, se saltea la animación cuando devuelve true.
    /// Si se pasa onCharRevealed, se invoca con cada caracter que aparece (ej. para el sonido de tecla).
    /// </summary>
    public IEnumerator Play(TMP_Text text, Func<bool> skipRequested = null, Action<char> onCharRevealed = null)
    {
        text.maxVisibleCharacters = 0;
        text.ForceMeshUpdate();
        int total = text.textInfo.characterCount;

        // Evita que el mismo click que abrió el mensaje lo saltee
        if (skipRequested != null) yield return null;

        for (int visible = 1; visible <= total; visible++)
        {
            text.maxVisibleCharacters = visible;
            char c = text.textInfo.characterInfo[visible - 1].character;
            onCharRevealed?.Invoke(c);

            float delay = GetDelayFor(c);
            float elapsed = 0f;
            while (elapsed < delay)
            {
                if (skipRequested != null && skipRequested())
                {
                    text.maxVisibleCharacters = total;
                    yield break;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }

    private float GetDelayFor(char c)
    {
        float delay = charDelay * UnityEngine.Random.Range(1f - charDelayJitter, 1f + charDelayJitter);
        if (".,;:!?".IndexOf(c) >= 0) delay += punctuationPause;
        return delay;
    }
}

using System;
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Componente listo para usar: agregalo junto a un TextMeshProUGUI o TextMeshPro
/// y llamá a Play(texto) desde cualquier script o UnityEvent.
/// </summary>
public class TypewriterText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private TypewriterEffect effect;
    private Coroutine _routine;

    public bool IsPlaying => _routine != null;

    public void Play(string content, Func<bool> skipRequested = null)
    {
        if (text == null)
        {
            Debug.LogError("[TypewriterText] Text field is null.");
            return;
        }

        Stop();
        text.text = content;
        text.maxVisibleCharacters = 0;
        _routine = StartCoroutine(Run(skipRequested));
    }

    /// <summary>Muestra el texto completo, sin animación.</summary>
    public void Show(string content)
    {
        Stop();
        text.text = content;
        text.maxVisibleCharacters = int.MaxValue;
    }

    /// <summary>Corta la animación y muestra el texto completo.</summary>
    public void Complete()
    {
        Stop();
        text.maxVisibleCharacters = int.MaxValue;
    }

    private void Stop()
    {
        if (_routine == null) return;
        StopCoroutine(_routine);
        _routine = null;
    }

    private IEnumerator Run(Func<bool> skipRequested)
    {
        yield return effect.Play(text, skipRequested);
        _routine = null;
    }
}

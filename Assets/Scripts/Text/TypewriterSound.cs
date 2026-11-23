// Sonido de tecla para el efecto de máquina de escribir: suena con cada caracter, pero con un intervalo
// mínimo entre sonidos, así el ritmo es creíble aunque el texto vaya rápido y respeta las pausas.
using UnityEngine;

public class TypewriterSound : MonoBehaviour
{
    [SerializeField] private TypewriterSoundSettings settings;
    [SerializeField] private AudioSource audioSource;

    private float _lastPlayTime = float.NegativeInfinity;
    private int _lastClipIndex = -1;

    // Lo llama TypewriterText con cada caracter que aparece
    public void OnCharRevealed(char c)
    {
        if (char.IsWhiteSpace(c)) return;
        if (Time.unscaledTime - _lastPlayTime < settings.MinInterval) return;

        AudioClip clip = PickClip();
        if (clip == null) return;

        _lastPlayTime = Time.unscaledTime;
        audioSource.pitch = settings.RandomPitch;
        audioSource.PlayOneShot(clip, settings.RandomVolume);
    }

    // Golpe de carro al terminar de escribir
    public void PlayEnd()
    {
        if (settings.EndClip == null) return;

        audioSource.pitch = 1f;
        audioSource.PlayOneShot(settings.EndClip, settings.EndVolume);
    }

    // Al azar entre los clips, sin repetir el anterior si hay otra opción
    private AudioClip PickClip()
    {
        AudioClip[] clips = settings.KeyClips;
        if (clips == null || clips.Length == 0) return null;

        int index = Random.Range(0, clips.Length);
        if (clips.Length > 1 && index == _lastClipIndex) index = (index + 1) % clips.Length;

        _lastClipIndex = index;
        return clips[index];
    }
}

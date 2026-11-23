// Clips y ritmo del sonido de tecla de la máquina de escribir.
using UnityEngine;

[CreateAssetMenu(fileName = "TypewriterSoundSettings", menuName = "Detective/Typewriter Sound Settings")]
public class TypewriterSoundSettings : ScriptableObject
{
    [Header("Teclas")]
    [Tooltip("Sonidos de tecla. Se elige uno al azar por golpe.")]
    [SerializeField] private AudioClip[] keyClips;
    [Tooltip("Segundos mínimos entre golpes de tecla, aunque el texto aparezca más rápido.")]
    [Min(0.01f)] [SerializeField] private float minInterval = 0.08f;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.95f, 1.05f);
    [SerializeField] private Vector2 volumeRange = new Vector2(0.8f, 1f);

    [Header("Cierre")]
    [Tooltip("Opcional. Suena al terminar de escribir el texto (ej. carro o campanita).")]
    [SerializeField] private AudioClip endClip;
    [Range(0f, 1f)] [SerializeField] private float endVolume = 1f;

    public AudioClip[] KeyClips => keyClips;
    public float MinInterval => minInterval;
    public float RandomPitch => Random.Range(pitchRange.x, pitchRange.y);
    public float RandomVolume => Random.Range(volumeRange.x, volumeRange.y);
    public AudioClip EndClip => endClip;
    public float EndVolume => endVolume;
}

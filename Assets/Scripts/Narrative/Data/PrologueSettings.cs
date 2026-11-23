// Texto, voz en off y tiempos del prólogo.
using UnityEngine;

[CreateAssetMenu(fileName = "PrologueSettings", menuName = "Detective/Prologue Settings")]
public class PrologueSettings : ScriptableObject
{
    [Header("Contenido")]
    [TextArea(4, 10)] [SerializeField] private string text;
    [Tooltip("Opcional. Voz en off que acompaña al texto.")]
    [SerializeField] private AudioClip voice;

    [Header("Tiempos")]
    [Tooltip("Segundos en negro antes de que empiece el texto.")]
    [Min(0f)] [SerializeField] private float textDelay = 1f;
    [Tooltip("Segundos entre que termina de escribirse el texto y empieza a desvanecerse.")]
    [Min(0f)] [SerializeField] private float holdDuration = 4f;
    [Min(0.01f)] [SerializeField] private float textFadeDuration = 1.5f;
    [Tooltip("Segundos en negro, ya sin texto, antes de empezar a mostrar la escena.")]
    [Min(0f)] [SerializeField] private float blackHoldDuration = 2f;
    [Tooltip("Duración del fundido del negro al juego.")]
    [Min(0.01f)] [SerializeField] private float backgroundFadeDuration = 2f;

    public string Text => text;
    public AudioClip Voice => voice;
    public float TextDelay => textDelay;
    public float HoldDuration => holdDuration;
    public float TextFadeDuration => textFadeDuration;
    [Tooltip("Avance del fundido en el tiempo. La curva por defecto mantiene el negro al principio y revela la escena hacia el final.")]
    [SerializeField] private AnimationCurve backgroundFadeCurve = new AnimationCurve(
        new Keyframe(0f, 0f, 0f, 0f),
        new Keyframe(0.6f, 0.3f, 1.2f, 1.2f),
        new Keyframe(1f, 1f, 2.2f, 2.2f));

    public float BlackHoldDuration => blackHoldDuration;
    public AnimationCurve BackgroundFadeCurve => backgroundFadeCurve;
    public float BackgroundFadeDuration => backgroundFadeDuration;
}

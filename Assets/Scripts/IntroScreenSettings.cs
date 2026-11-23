// Texto y tiempos de la pantalla en negro que aparece al abrir el juego.
using UnityEngine;

[CreateAssetMenu(fileName = "IntroScreenSettings", menuName = "Detective/Intro Screen Settings")]
public class IntroScreenSettings : ScriptableObject
{
    [Header("Contenido")]
    [TextArea(3, 8)] [SerializeField] private string text;
    [SerializeField] private string promptText = "[Presiona cualquier tecla para continuar]";

    [Header("Tiempos")]
    [Min(0f)] [SerializeField] private float textDelay = 0.5f;
    [Min(0.01f)] [SerializeField] private float textFadeInDuration = 1.5f;
    [Tooltip("Segundos desde que arranca la pantalla hasta que aparece el aviso para continuar.")]
    [Min(0f)] [SerializeField] private float promptDelay = 4f;
    [Tooltip("Velocidad del parpadeo del aviso. 0 = sin parpadeo.")]
    [Min(0f)] [SerializeField] private float promptBlinkSpeed = 2f;
    [Range(0f, 1f)] [SerializeField] private float promptMinAlpha = 0.35f;

    [Header("Salida (al presionar)")]
    [Tooltip("Duración del fundido del texto principal, una vez apagado el aviso.")]
    [Min(0.01f)] [SerializeField] private float bodyFadeOutDuration = 2.5f;

    [Header("Logo (entre el texto y el menú)")]
    [Tooltip("Segundos en negro entre que se va el texto y empieza a aparecer el logo.")]
    [Min(0f)] [SerializeField] private float logoDelay = 0.5f;
    [Min(0.01f)] [SerializeField] private float logoFadeInDuration = 1.5f;
    [Tooltip("Segundos que el logo queda en pantalla.")]
    [Min(0f)] [SerializeField] private float logoHoldDuration = 2f;
    [Min(0.01f)] [SerializeField] private float logoFadeOutDuration = 1.5f;

    [Header("Fondo")]
    [Tooltip("Segundos en negro, ya sin logo, antes de mostrar el menú.")]
    [Min(0f)] [SerializeField] private float blackHoldDuration = 0.5f;
    [Tooltip("Duración del fundido del fondo al menú.")]
    [Min(0.01f)] [SerializeField] private float fadeOutDuration = 1.5f;

    public string Text => text;
    public string PromptText => promptText;
    public float TextDelay => textDelay;
    public float TextFadeInDuration => textFadeInDuration;
    public float PromptDelay => promptDelay;
    public float PromptBlinkSpeed => promptBlinkSpeed;
    public float PromptMinAlpha => promptMinAlpha;
    public float BodyFadeOutDuration => bodyFadeOutDuration;
    public float LogoDelay => logoDelay;
    public float LogoFadeInDuration => logoFadeInDuration;
    public float LogoHoldDuration => logoHoldDuration;
    public float LogoFadeOutDuration => logoFadeOutDuration;
    public float BlackHoldDuration => blackHoldDuration;
    public float FadeOutDuration => fadeOutDuration;
}

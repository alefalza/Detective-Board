using UnityEngine;
using EaseType = iTween.EaseType;

[CreateAssetMenu(fileName = "SceneryViewerSettings", menuName = "Detective/Scenery Viewer Settings")]
public class SceneryViewerSettings : ScriptableObject
{
    [Header("Inspectables nuevos")]
    [Tooltip("Duración del fade in de un inspectable recién desbloqueado.")]
    [Min(0.01f)] [SerializeField] private float revealDuration = 1f;
    [Tooltip("Suena cuando empieza a aparecer un inspectable desbloqueado.")]
    [SerializeField] private AudioClip revealClip;

    [Header("Ver a través")]
    [Tooltip("Radio del agujero, en unidades del mundo.")]
    [Min(0.01f)] [SerializeField] private float peekRadius = 0.8f;
    [Tooltip("Qué parte del radio se difumina (0 = borde duro, 1 = todo difuminado).")]
    [Range(0f, 1f)] [SerializeField] private float peekSoftness = 0.5f;
    [Tooltip("Opacidad de la imagen dentro del agujero.")]
    [Range(0f, 1f)] [SerializeField] private float peekMinAlpha = 0.15f;
    [Min(0.01f)] [SerializeField] private float peekFadeDuration = 0.2f;

    [Header("Ventana de detalles")]
    [Min(0.01f)] [SerializeField] private float windowDuration = 0.2f;
    [SerializeField] private EaseType windowOpenEase = EaseType.easeOutBack;
    [SerializeField] private EaseType windowCloseEase = EaseType.easeInBack;
    [Tooltip("Texto del botón si la pista todavía no se archivó.")]
    [SerializeField] private string archiveLabel = "Archivar";
    [Tooltip("Texto del botón si ya se archivó o el inspectable no tiene pista.")]
    [SerializeField] private string closeLabel = "Cerrar";
    [Tooltip("Suena al archivar una pista.")]
    [SerializeField] private AudioClip archiveClip;
    [Tooltip("Suena al archivar una pista.")]

    public float RevealDuration => revealDuration;
    public AudioClip RevealClip => revealClip;
    public float PeekRadius => peekRadius;
    public float PeekSoftness => peekSoftness;
    public float PeekMinAlpha => peekMinAlpha;
    public float PeekFadeDuration => peekFadeDuration;
    public float WindowDuration => windowDuration;
    public EaseType WindowOpenEase => windowOpenEase;
    public EaseType WindowCloseEase => windowCloseEase;
    public string ArchiveLabel => archiveLabel;
    public string CloseLabel => closeLabel;
    public AudioClip ArchiveClip => archiveClip;
}

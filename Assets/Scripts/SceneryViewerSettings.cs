using UnityEngine;
using EaseType = iTween.EaseType;

[CreateAssetMenu(fileName = "SceneryViewerSettings", menuName = "Detective/Scenery Viewer Settings")]
public class SceneryViewerSettings : ScriptableObject
{
    [Header("Inspectables nuevos")]
    [Tooltip("Duración del fade in de un inspectable recién desbloqueado.")]
    [Min(0.01f)] [SerializeField] private float revealDuration = 1f;

    [Header("Ventana de detalles")]
    [Min(0.01f)] [SerializeField] private float windowDuration = 0.2f;
    [SerializeField] private EaseType windowOpenEase = EaseType.easeOutBack;
    [SerializeField] private EaseType windowCloseEase = EaseType.easeInBack;

    public float RevealDuration => revealDuration;
    public float WindowDuration => windowDuration;
    public EaseType WindowOpenEase => windowOpenEase;
    public EaseType WindowCloseEase => windowCloseEase;
}

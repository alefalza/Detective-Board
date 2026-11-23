/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: aparición de inspectables, efecto de ver a través y ventana de detalles.
 */

using UnityEngine;
using EaseType = iTween.EaseType;

[CreateAssetMenu(fileName = "SceneryViewerSettings", menuName = "Detective/Scenery Viewer Settings")]
public class SceneryViewerSettings : ScriptableObject
{
    [Header("New Inspectables")]
    [Min(0.01f)] [SerializeField] private float revealDuration = 1f;
    [SerializeField] private AudioClip revealClip;

    [Header("Seethrough")]
    [Min(0.01f)] [SerializeField] private float peekRadius = 0.8f;
    [Range(0f, 1f)] [SerializeField] private float peekSoftness = 0.5f;
    [Range(0f, 1f)] [SerializeField] private float peekMinAlpha = 0.15f;
    [Min(0.01f)] [SerializeField] private float peekFadeDuration = 0.2f;

    [Header("Window")]
    [Min(0.01f)] [SerializeField] private float windowDuration = 0.2f;
    [SerializeField] private EaseType windowOpenEase = EaseType.easeOutBack;
    [SerializeField] private EaseType windowCloseEase = EaseType.easeInBack;
    [SerializeField] private string archiveLabel = "Archivar";
    [SerializeField] private string closeLabel = "Cerrar";
    [SerializeField] private AudioClip archiveClip;

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

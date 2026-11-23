/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: pista del tablero, con sus datos, arrastre, pin, invalidación y feedback.
 */

using UnityEngine;

[RequireComponent(typeof(EvidenceItemView), typeof(EvidenceOverlapResolver))]
public class EvidenceItem : BaseDraggable
{
    [Header("Data")]
    [SerializeField] private EvidenceData data;

    [Header("Pin")]
    [SerializeField] private Transform pinTransform;
    [SerializeField] private float pinRadius = 0.4f;
    [SerializeField] private float pinHoverScale = 1.25f;
    [SerializeField] private float pinHoverTime = 0.12f;
    [SerializeField] private iTween.EaseType pinHoverEase = iTween.EaseType.easeOutQuad;

    [Header("Audio")]
    [SerializeField] private AudioSource onDragEndAudioSource;

    private const string PinTweenName = "EvidencePinHover";

    private EvidenceItemView _view;
    private Vector3 _pinBaseScale;
    private EvidenceOverlapResolver _overlapResolver;

    public EvidenceData Data => data;
    public bool IsInvalidated { get; private set; }

    protected override bool CanDrag => !_view.IsSpawnPunching;

    private void Awake()
    {
        _view = GetComponent<EvidenceItemView>();
        _overlapResolver = GetComponent<EvidenceOverlapResolver>();
        _overlapResolver.Moved += NotifyMoved;
        if (pinTransform != null) _pinBaseScale = pinTransform.localScale;
    }

    private void Start()
    {
        if (data != null) SetupFromData(data);
    }

    public void SetupFromData(EvidenceData newData)
    {
        data = newData;
        data.ApplyTo(_view);
    }

    public void InvalidateEvidence()
    {
        IsInvalidated = true;
        _view.ShowInvalidated();

        Debug.Log($"¡La evidencia {data.title} ha sido DESMENTIDA!");
    }

    public void BringToFront() => _view.BringToFront();

    public void MarkValidated(Vector3 punchAmount, float punchTime, Color color)
        => _view.PlayValidatedFeedback(punchAmount, punchTime, color);

    public void PlaySpawnFeedback(Vector3 punchAmount, float punchTime)
        => _view.PlaySpawnFeedback(punchAmount, punchTime);

    public void ResolveOverlap() => _overlapResolver.Resolve();

    protected override void OnDragStarted()
    {
        _overlapResolver.CancelSlide();
        _view.BringToFront();
        _view.Lift();
    }

    protected override void OnDragMoved()
    {
        _view.UpdateTilt();
        NotifyMoved();
    }

    protected override void OnDragEnded()
    {
        _view.Release();

        _overlapResolver.Resolve();

        if (onDragEndAudioSource != null) onDragEndAudioSource.Play();
    }

    public void ShowGroupCounter(string text, Color color) => _view.ShowGroupCounter(text, color);
    public void HideGroupCounter() => _view.HideGroupCounter();
    public void PlayShake(Vector3 amount, float time) => _view.PlayShake(amount, time);

    public void SetPinHovered(bool hovered)
    {
        if (pinTransform == null) return;

        iTween.StopByName(pinTransform.gameObject, PinTweenName);
        iTween.ScaleTo(pinTransform.gameObject, iTween.Hash(
            "name", PinTweenName,
            "scale", hovered ? _pinBaseScale * pinHoverScale : _pinBaseScale,
            "time", pinHoverTime,
            "easetype", pinHoverEase,
            "ignoretimescale", true
        ));
    }

    public bool IsClickingPin(Vector3 worldMousePos)
    {
        if (pinTransform == null) return false;
        return Vector2.Distance(worldMousePos, pinTransform.position) <= pinRadius;
    }

    public Vector3 GetPinPosition()
    {
        return pinTransform != null ? pinTransform.position : transform.position;
    }

    private void NotifyMoved() => CorkboardManager.Instance?.OnItemMoved(this);
}

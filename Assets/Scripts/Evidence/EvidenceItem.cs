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

    private DropZone _currentDropZone;
    private DropZone _hoveredDropZone;

    public EvidenceData Data => data;
    public bool IsInvalidated { get; private set; }

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

    // ── Datos ────────────────────────────────────────────────────────

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

    // ── Drag (hooks de BaseDraggable) ────────────────────────────────

    protected override void OnDragStarted()
    {
        _overlapResolver.CancelSlide();
        _view.BringToFront();
        _view.Lift();

        if (_currentDropZone != null)
        {
            _currentDropZone.Vacate();
            _currentDropZone = null;
        }
    }

    protected override void OnDragMoved()
    {
        _view.UpdateTilt();
        NotifyMoved();
    }

    protected override void OnDragEnded()
    {
        _view.Release();

        if (_hoveredDropZone != null && !_hoveredDropZone.IsOccupied)
            SnapToZone(_hoveredDropZone);
        else if (_currentDropZone != null)
            SnapToZone(_currentDropZone);
        else
            _overlapResolver.Resolve();

        if (onDragEndAudioSource != null) onDragEndAudioSource.Play();
    }

    // ── Drop zones ───────────────────────────────────────────────────

    public void SnapToZone(DropZone zone)
    {
        _currentDropZone = zone;
        zone.Occupy(this);
        transform.position = zone.transform.position;
        NotifyMoved();
    }

    public void SetHoveredZone(DropZone zone) => _hoveredDropZone = zone;
    public void ClearHoveredZone(DropZone zone) { if (_hoveredDropZone == zone) _hoveredDropZone = null; }

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

    // ── Consultas ────────────────────────────────────────────────────

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

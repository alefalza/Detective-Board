using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class KeywordChipUI : BaseDraggable,
    IPointerDownHandler, IPointerUpHandler, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Referencias Visuales")]
    [SerializeField] private TextMeshProUGUI keywordText;

    [Header("Arrastre")]
    [SerializeField] private bool draggable = true;

    [Header("Settings")]
    [SerializeField] private iTween.EaseType moveEaseType = iTween.EaseType.easeInCubic;
    [SerializeField] private iTween.EaseType scaleEaseType = iTween.EaseType.easeInCubic;
    [SerializeField] private float finalScaleFactor = 0.3f;
    [SerializeField] private float tiltAmount = 12f; // Grados máximos de inclinación durante el vuelo

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip tackSound;

    private Action _onAppearComplete;
    private Action _onFlyToHUDComplete;

    private CanvasGroup _canvasGroup;
    private KeywordSlot _currentSlot;

    // Origen del arrastre, para volver si no se suelta en un slot libre
    private Transform _originParent;
    private int _originSiblingIndex;
    private Vector2 _originAnchoredPosition;
    private KeywordSlot _originSlot;

    // Chip que se está arrastrando ahora (null si ninguno). Lo consultan sistemas que reaccionan a clicks.
    public static KeywordChipUI Dragged { get; private set; }

    public RectTransform RectTransform => (RectTransform)transform;
    public string Keyword { get; private set; }

    protected override bool CanDrag => draggable;

    private void Awake() => _canvasGroup = GetComponent<CanvasGroup>();

    private void OnDestroy()
    {
        if (Dragged == this) Dragged = null;
        if (CursorManager.Instance != null) CursorManager.Instance.Release(this);
    }

    public void Setup(string text)
    {
        Keyword = text;
        if (keywordText != null) keywordText.text = text;
    }

    // ── Arrastre ─────────────────────────────────────────────────────

    // Eventos de UI: solo traducen al API común
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) BeginDrag(eventData.position);
    }

    // Sin umbral: el movimiento empieza en cuanto se mueve el mouse, sin saltito inicial
    public void OnInitializePotentialDrag(PointerEventData eventData) => eventData.useDragThreshold = false;

    public void OnDrag(PointerEventData eventData) => Drag(eventData.position);

    public void OnPointerUp(PointerEventData eventData)
    {
        // Sin movimiento no hay OnEndDrag, así que se cierra acá.
        // Con movimiento lo cierra OnEndDrag, que corre después del OnDrop del slot.
        if (eventData.button == PointerEventData.InputButton.Left && !eventData.dragging) EndDrag();
    }

    public void OnEndDrag(PointerEventData eventData) => EndDrag();

    protected override void OnDragStarted()
    {
        _originParent = transform.parent;
        _originSiblingIndex = transform.GetSiblingIndex();
        _originAnchoredPosition = RectTransform.anchoredPosition;
        _originSlot = _currentSlot;

        if (_currentSlot != null)
        {
            _currentSlot.Vacate();
            _currentSlot = null;
        }

        // Sale del layout/slot y se dibuja por encima de todo
        transform.SetParent(GetComponentInParent<Canvas>().rootCanvas.transform, true);
        transform.SetAsLastSibling();

        _canvasGroup.blocksRaycasts = false; // permite que el slot de abajo reciba el drop
        Dragged = this;
        CursorManager.Instance.Request(this, GameCursorType.Grab);
    }

    protected override void OnDragEnded()
    {
        _canvasGroup.blocksRaycasts = true;
        Dragged = null;
        CursorManager.Instance.Release(this);

        // OnDrop del slot corre antes que esto: si no hay slot, vuelve al origen
        if (_currentSlot == null) ReturnToOrigin();
    }

    public void SnapToSlot(KeywordSlot slot)
    {
        _currentSlot = slot;
        slot.Occupy(this);
        transform.SetParent(slot.transform, false);
        RectTransform.anchoredPosition = Vector2.zero;
    }

    private void ReturnToOrigin()
    {
        transform.SetParent(_originParent, true);
        transform.SetSiblingIndex(_originSiblingIndex);
        RectTransform.anchoredPosition = _originAnchoredPosition;

        if (_originSlot != null)
        {
            _currentSlot = _originSlot;
            _originSlot.Occupy(this);
        }
    }

    // ── Animaciones ──────────────────────────────────────────────────

    public void PlayAppearAnimation(float duration, Action onCompleteCallback = null)
    {
        draggable = false; // los chips de la animación de deducción no se arrastran

        if (audioSource != null && tackSound != null)
        {
            audioSource.PlayOneShot(tackSound);
        }

        _onAppearComplete = onCompleteCallback;

        iTween.PunchScale(gameObject, iTween.Hash(
            "amount", new Vector3(0.3f, 0.3f, 0f),
            "time", duration,
            "oncomplete", nameof(OnAppearCompleteInternal),
            "oncompletetarget", gameObject,
            "ignoretimescale", true
        ));
    }

    private void OnAppearCompleteInternal()
    {
        _onAppearComplete?.Invoke();
        _onAppearComplete = null;
    }

    /// <summary>
    /// Ejecuta el viaje hacia el HUD, achicamiento, inclinación (tilt) y destrucción al llegar.
    /// </summary>
    public void PlayFlyToHUDAnimation(Vector3 targetWorldPosition, float duration, Action onCompleteCallback = null)
    {
        _onFlyToHUDComplete = onCompleteCallback;

        // 1. Calcular dirección horizontal para determinar hacia dónde inclinar
        float deltaX = targetWorldPosition.x - transform.position.x;
        float targetTiltZ = Mathf.Sign(deltaX) * -tiltAmount; // Inclina en dirección opuesta/favor del avance

        // 2. Mover hacia la posición del HUD
        iTween.MoveTo(gameObject, iTween.Hash(
            "position", targetWorldPosition,
            "time", duration,
            "easetype", moveEaseType,
            "ignoretimescale", true
        ));

        // 3. Aplicar Tilt (Rotación Z)
        iTween.RotateTo(gameObject, iTween.Hash(
            "rotation", new Vector3(0f, 0f, targetTiltZ),
            "time", duration,
            "easetype", moveEaseType,
            "ignoretimescale", true
        ));

        // 4. Reducir escala simultáneamente y notificar al finalizar
        iTween.ScaleTo(gameObject, iTween.Hash(
            "scale", Vector3.one * finalScaleFactor,
            "time", duration,
            "easetype", scaleEaseType,
            "ignoretimescale", true,
            "oncomplete", nameof(OnFlyToHUDCompleteInternal),
            "oncompletetarget", gameObject
        ));
    }

    private void OnFlyToHUDCompleteInternal()
    {
        _onFlyToHUDComplete?.Invoke();
        _onFlyToHUDComplete = null;
        Destroy(gameObject);
    }
}

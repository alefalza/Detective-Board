/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: ficha de keyword que se arrastra entre slots y anima su aparición y su vuelo al HUD.
 */

using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class KeywordChip_UI : BaseDraggable,
    IPointerDownHandler, IPointerUpHandler, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI keywordText;

    [Header("Drag")]
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

    private Transform _originParent;
    private int _originSiblingIndex;
    private Vector2 _originAnchoredPosition;
    private KeywordSlot _originSlot;

    public static KeywordChip_UI Dragged { get; private set; }

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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) BeginDrag(eventData.position);
    }

    public void OnInitializePotentialDrag(PointerEventData eventData) => eventData.useDragThreshold = false;

    public void OnDrag(PointerEventData eventData) => Drag(eventData.position);

    public void OnPointerUp(PointerEventData eventData)
    {
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

    public void PlayAppearAnimation(float duration, Action onCompleteCallback = null)
    {
        draggable = false;

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

    public void PlayFlyToHUDAnimation(Vector3 targetWorldPosition, float duration, Action onCompleteCallback = null)
    {
        _onFlyToHUDComplete = onCompleteCallback;

        float deltaX = targetWorldPosition.x - transform.position.x;
        float targetTiltZ = Mathf.Sign(deltaX) * -tiltAmount;

        iTween.MoveTo(gameObject, iTween.Hash(
            "position", targetWorldPosition,
            "time", duration,
            "easetype", moveEaseType,
            "ignoretimescale", true
        ));

        iTween.RotateTo(gameObject, iTween.Hash(
            "rotation", new Vector3(0f, 0f, targetTiltZ),
            "time", duration,
            "easetype", moveEaseType,
            "ignoretimescale", true
        ));

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

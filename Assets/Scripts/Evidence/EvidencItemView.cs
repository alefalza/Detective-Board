using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class EvidenceItemView : MonoBehaviour
{
    [Header("Referencias Visuales")]
    [SerializeField] private SpriteRenderer backgroundImage;
    [SerializeField] private TextMeshPro titleText;
    [SerializeField] private GameObject strikeThroughOverlay;
    [SerializeField] private SortingGroup sortingGroup;

    [Header("Shadow & Tilt")]
    [SerializeField] private Transform shadowTransform;
    [SerializeField] private Vector3 shadowDragOffset = new Vector3(0.15f, -0.2f, 0f);
    [Space]
    [SerializeField] private float tiltAmount = 4f;
    [SerializeField] private float tiltSensitivity = 2f;

    private const string ValidatedTweenName = "EvidenceValidatedPunch";

    private static int _topOrder;

    private Quaternion _originalRotation;
    private Vector3 _originalShadowLocalPos;
    private Vector3 _lastPosition;
    private Color _pendingValidatedColor;

    private void Start()
    {
        _originalRotation = transform.localRotation;
        if (shadowTransform != null) _originalShadowLocalPos = shadowTransform.localPosition;
    }

    // ── Contenido ────────────────────────────────────────────────────

    public void SetTitle(string title)
    {
        if (titleText != null) titleText.text = title;
    }

    public void SetBackgroundImage(Sprite sprite)
    {
        if (backgroundImage == null) return;
        backgroundImage.gameObject.SetActive(sprite != null);
        backgroundImage.sprite = sprite;
    }

    public void SetBackgroundColor(Color color)
    {
        if (backgroundImage != null) backgroundImage.color = color;
    }

    public void ShowInvalidated()
    {
        if (strikeThroughOverlay != null) strikeThroughOverlay.SetActive(true);
    }

    public void BringToFront()
    {
        if (sortingGroup != null) sortingGroup.sortingOrder = ++_topOrder;
    }

    // Punch de escala y, al terminar, colorea el fondo (pista de una deducción validada)
    public void PlayValidatedFeedback(Vector3 punchAmount, float punchTime, Color validatedColor)
    {
        _pendingValidatedColor = validatedColor;

        iTween.StopByName(gameObject, ValidatedTweenName);
        iTween.PunchScale(gameObject, iTween.Hash(
            "name", ValidatedTweenName,
            "amount", punchAmount,
            "time", punchTime,
            "ignoretimescale", true,
            "oncomplete", nameof(OnValidatedPunchComplete),
            "oncompletetarget", gameObject
        ));
    }

    private void OnValidatedPunchComplete() => SetBackgroundColor(_pendingValidatedColor);

    // ── Feedback del arrastre ────────────────────────────────────────

    public void Lift()
    {
        _lastPosition = transform.position;
        if (shadowTransform != null) shadowTransform.localPosition = _originalShadowLocalPos + shadowDragOffset;
    }

    // Inclinación según la velocidad horizontal
    public void UpdateTilt()
    {
        if (Time.deltaTime > 0f)
        {
            float velocityX = (transform.position.x - _lastPosition.x) / Time.deltaTime;
            float targetTilt = Mathf.Clamp(-velocityX * tiltSensitivity, -tiltAmount, tiltAmount);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, targetTilt), Time.deltaTime * 10f);
        }
        _lastPosition = transform.position;
    }

    public void Release()
    {
        transform.localRotation = _originalRotation;
        if (shadowTransform != null) shadowTransform.localPosition = _originalShadowLocalPos;
    }
}

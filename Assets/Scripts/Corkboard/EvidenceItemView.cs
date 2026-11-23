/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: parte visual de una pista: contenido, sombra, inclinación, contador y animaciones.
 */

using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class EvidenceItemView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer backgroundImage;
    [SerializeField] private TextMeshPro titleText;
    [SerializeField] private GameObject strikeThroughOverlay;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private TextMeshPro groupCounterText;

    [Header("Shadow & Tilt")]
    [SerializeField] private Transform shadowTransform;
    [SerializeField] private Vector3 shadowDragOffset = new Vector3(0.15f, -0.2f, 0f);
    [Space]
    [SerializeField] private float tiltAmount = 4f;
    [SerializeField] private float tiltSensitivity = 2f;

    private const string ValidatedTweenName = "EvidenceValidatedPunch";
    private const string SpawnTweenName = "EvidenceSpawnPunch";
    private const string ShakeTweenName = "EvidenceOverflowShake";

    private static int _topOrder;

    private Quaternion _originalRotation;
    private Vector3 _originalShadowLocalPos;
    private Vector3 _lastPosition;
    private Color _pendingValidatedColor;

    public bool IsSpawnPunching { get; private set; }

    private void Start()
    {
        _originalRotation = transform.localRotation;
        if (shadowTransform != null) _originalShadowLocalPos = shadowTransform.localPosition;
    }

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

    public void ShowGroupCounter(string text, Color color)
    {
        if (groupCounterText == null) return;

        groupCounterText.gameObject.SetActive(true);
        groupCounterText.text = text;
        groupCounterText.color = color;
    }

    public void HideGroupCounter()
    {
        if (groupCounterText != null) groupCounterText.gameObject.SetActive(false);
    }

    public void BringToFront()
    {
        if (sortingGroup != null) sortingGroup.sortingOrder = ++_topOrder;
    }

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

    public void PlaySpawnFeedback(Vector3 punchAmount, float punchTime)
    {
        IsSpawnPunching = true;

        iTween.PunchScale(gameObject, iTween.Hash(
            "name", SpawnTweenName,
            "amount", punchAmount,
            "time", punchTime,
            "ignoretimescale", true,
            "oncomplete", nameof(OnSpawnPunchComplete),
            "oncompletetarget", gameObject
        ));
    }

    private void OnSpawnPunchComplete() => IsSpawnPunching = false;

    public void PlayShake(Vector3 amount, float time)
    {
        iTween.StopByName(gameObject, ShakeTweenName);
        transform.localRotation = _originalRotation;

        iTween.ShakeRotation(gameObject, iTween.Hash(
            "name", ShakeTweenName,
            "amount", amount,
            "time", time,
            "ignoretimescale", true
        ));
    }

    public void Lift()
    {
        _lastPosition = transform.position;
        if (shadowTransform != null) shadowTransform.localPosition = _originalShadowLocalPos + shadowDragOffset;
    }

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

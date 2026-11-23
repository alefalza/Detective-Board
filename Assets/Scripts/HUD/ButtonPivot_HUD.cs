/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: muestra y oculta un botón del HUD rotándolo sobre su pivot.
 */

using System;
using UnityEngine;
using EaseType = iTween.EaseType;

public class ButtonPivot_HUD : MonoBehaviour
{
    [Header("X Rotation")]
    [SerializeField] private float hiddenAngleOffset = -60f;

    [Header("Visibility")]
    [SerializeField] private bool startVisible = false;

    [Header("Animation")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private EaseType showEase = EaseType.easeOutBack;
    [SerializeField] private EaseType hideEase = EaseType.easeInBack;

    [Header("Audio")]
    [SerializeField] private AudioSource onAnimEndAudioSource;

    private const string TweenName = "ButtonPivotRotate";

    private float _shownAngleX;
    private float _hiddenAngleX;

    private bool _wantsVisible;
    private bool _hiddenByView;
    private bool _isShown;
    private bool _initialized;

    private bool ShouldBeVisible => _wantsVisible && !_hiddenByView;

    public bool IsFullyShown { get; private set; }

    public event Action AnimationStateChanged;

    private void Awake()
    {
        _shownAngleX = transform.localEulerAngles.x;
        _hiddenAngleX = _shownAngleX + hiddenAngleOffset;
        _wantsVisible = startVisible;
    }

    private void Start()
    {
        _isShown = ShouldBeVisible;
        IsFullyShown = _isShown;
        _initialized = true;

        SetAngleImmediate(_isShown ? _shownAngleX : _hiddenAngleX);
    }

    private void SetAngleImmediate(float angle)
    {
        Vector3 euler = transform.localEulerAngles;
        euler.x = angle;
        transform.localEulerAngles = euler;
    }

    public void SetVisible(bool visible)
    {
        _wantsVisible = visible;
        Refresh();
    }

    public void SetHiddenByView(bool hidden)
    {
        _hiddenByView = hidden;
        Refresh();
    }

    private void Refresh()
    {
        if (!_initialized) return;

        bool visible = ShouldBeVisible;
        if (visible == _isShown) return;

        _isShown = visible;
        IsFullyShown = false;
        AnimationStateChanged?.Invoke();

        RotateTo(visible ? _shownAngleX : _hiddenAngleX, visible ? showEase : hideEase);
    }

    private void RotateTo(float angleX, EaseType ease)
    {
        iTween.StopByName(gameObject, TweenName);

        Vector3 target = transform.localEulerAngles;
        target.x = angleX;

        iTween.RotateTo(gameObject, iTween.Hash(
            "name", TweenName,
            "rotation", target,
            "islocal", true,
            "time", duration,
            "easetype", ease,
            "ignoretimescale", true,
            "oncomplete", nameof(OnAnimComplete)
        ));
    }

    private void OnAnimComplete()
    {
        IsFullyShown = _isShown;
        AnimationStateChanged?.Invoke();

        if (onAnimEndAudioSource != null) onAnimEndAudioSource.Play();
    }
}

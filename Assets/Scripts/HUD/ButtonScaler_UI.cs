/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: feedback de escala de un botón al presionarlo, seleccionarlo o hacer un punch.
 */

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using EaseType = iTween.EaseType;

public class ButtonScaler_UI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("On Pressed Settings")]
    [SerializeField] private Vector3 pressedScale = new Vector3(0.9f, 0.9f, 0.9f);
    [SerializeField] private float duration = 0.1f;
    [SerializeField] private EaseType pressEase = EaseType.easeOutQuad;

    [Header("Selected Settings")]
    [SerializeField] private Vector3 selectedScale = new Vector3(1.15f, 1.15f, 1.15f);
    [SerializeField] private float selectedDuration = 0.4f;
    [SerializeField] private EaseType selectEase = EaseType.easeOutBack;
    [SerializeField] private EaseType deselectEase = EaseType.easeOutCubic;

    [Header("Events")]
    [SerializeField] private UnityEvent onClickEvent;
    [SerializeField] private UnityEvent onReleaseEvent;

    private const string TweenName = "ButtonScalerTween";

    private Vector3 _baseScale;
    private bool _isSelected;
    private bool _isPressed;

    private Vector3 RestScale => _isSelected ? Vector3.Scale(_baseScale, selectedScale) : _baseScale;

    private void Awake() => _baseScale = transform.localScale;

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;
        ScaleTo(Vector3.Scale(RestScale, pressedScale), duration, pressEase);
        onClickEvent?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ReleasePress();
        onReleaseEvent?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData) => ReleasePress();

    private void ReleasePress()
    {
        if (!_isPressed) return;

        _isPressed = false;
        ScaleTo(RestScale, duration, pressEase);
    }
    
    public void SetSelected(bool selected, bool animate = true)
    {
        if (_isSelected == selected) return;
        _isSelected = selected;

        if (!animate)
        {
            iTween.StopByName(gameObject, TweenName);
            transform.localScale = RestScale;
            return;
        }

        ScaleTo(RestScale, selectedDuration, selected ? selectEase : deselectEase);
    }

    public void Punch(Vector3 amount, float time)
    {
        iTween.StopByName(gameObject, TweenName);
        transform.localScale = RestScale;

        iTween.PunchScale(gameObject, iTween.Hash(
            "name", TweenName,
            "amount", amount,
            "time", time,
            "ignoretimescale", true
        ));
    }

    private void ScaleTo(Vector3 target, float time, EaseType ease)
    {
        iTween.StopByName(gameObject, TweenName);

        iTween.ScaleTo(gameObject, iTween.Hash(
            "name", TweenName,
            "scale", target,
            "time", time,
            "easetype", ease,
            "ignoretimescale", true
        ));
    }

    private void OnDisable()
    {
        iTween.StopByName(gameObject, TweenName);
        _isPressed = false;
        transform.localScale = RestScale;
    }
}

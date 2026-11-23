/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: texto de UI clickeable con subrayado al pasar el mouse.
 */

using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextMeshProUIInteractive : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Underline")]
    [SerializeField] private GameObject underlineObject;

    [Header("OnClick")]
    [SerializeField] private UnityEvent onClickEvent;

    private void OnEnable()
    {
        if (underlineObject != null)
            SetUnderlineActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (underlineObject != null)
            SetUnderlineActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (underlineObject != null)
            SetUnderlineActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClickEvent.Invoke();
    }

    private void SetUnderlineActive(bool active)
    {
        if (underlineObject != null)
            underlineObject.SetActive(active);
    }
}

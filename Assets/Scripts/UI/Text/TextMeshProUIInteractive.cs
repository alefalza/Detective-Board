using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextMeshProUIInteractive : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Underline")]
    [Tooltip("Arrastra aquí el objeto hijo de UI (una Image o Sprite) que actúa como subrayado")]
    [SerializeField] private GameObject underlineObject;

    [Header("OnClick")]
    [Tooltip("Puedes arrastrar funciones directamente desde el Inspector")]
    [SerializeField] private UnityEvent onClickEvent;

    private void Awake()
    {
        if (underlineObject != null)
            underlineObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (underlineObject != null)
            underlineObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (underlineObject != null)
            underlineObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClickEvent.Invoke();
    }
}

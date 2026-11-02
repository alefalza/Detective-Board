using UnityEngine;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class TextMeshPro3DInteractive : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMeshPro;
    
    [Header("Underline")]
    [Tooltip("Arrastra aquí el objeto hijo (un Quad o Sprite) que actúa como subrayado")]
    [SerializeField] private GameObject underlineObject;

    [Header("OnClick")]
    [Tooltip("Puedes arrastrar funciones directamente desde el Inspector o programarlas abajo")]
    [SerializeField] private UnityEvent onClickEvent;

    private void Awake()
    {
        // Asegurarnos de que el subrayado comience oculto
        if (underlineObject != null)
            underlineObject.SetActive(false);
    }

    private void OnMouseEnter()
    {
        if (underlineObject != null)
            underlineObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (underlineObject != null)
            underlineObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        onClickEvent.Invoke();
    }
}

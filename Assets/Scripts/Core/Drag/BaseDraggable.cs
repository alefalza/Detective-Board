/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: lógica base de arrastre (offset, suavizado y escala) con hooks para las subclases.
 */

using UnityEngine;

public abstract class BaseDraggable : MonoBehaviour, IDraggable
{
    [Header("Drag")]
    [SerializeField] private float dragSmoothness = 15f;
    [SerializeField] private float dragScaleFactor = 1.05f;

    private Vector3 _dragOffset;
    private Vector3 _scaleBeforeDrag;

    public bool IsDragging { get; private set; }

    protected virtual bool CanDrag => true;

    public void BeginDrag(Vector3 pointerPosition)
    {
        if (IsDragging || !CanDrag) return;
        IsDragging = true;

        _dragOffset = transform.position - pointerPosition;
        OnDragStarted();

        _scaleBeforeDrag = transform.localScale;
        transform.localScale = _scaleBeforeDrag * dragScaleFactor;
    }

    public void Drag(Vector3 pointerPosition)
    {
        if (!IsDragging) return;

        Vector3 target = pointerPosition + _dragOffset;
        transform.position = dragSmoothness > 0f ?
            Vector3.Lerp(transform.position, target, Time.deltaTime * dragSmoothness) : target;

        OnDragMoved();
    }

    public void EndDrag()
    {
        if (!IsDragging) return;
        IsDragging = false;

        transform.localScale = _scaleBeforeDrag;
        OnDragEnded();
    }

    protected virtual void OnDragStarted() { }
    protected virtual void OnDragMoved() { }
    protected virtual void OnDragEnded() { }
}

using UnityEngine;

public interface IDraggable
{
    bool IsDragging { get; }

    void BeginDrag(Vector3 pointerPosition);
    void Drag(Vector3 pointerPosition);
    void EndDrag();
}

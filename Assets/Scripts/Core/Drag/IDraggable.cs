/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: interfaz común para objetos que se pueden arrastrar.
 */

using UnityEngine;

public interface IDraggable
{
    bool IsDragging { get; }

    void BeginDrag(Vector3 pointerPosition);
    void Drag(Vector3 pointerPosition);
    void EndDrag();
}

/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: loguea qué elementos de UI hay bajo el puntero cada vez que cambian.
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PointerRaycastDebugger : MonoBehaviour
{
    private readonly List<RaycastResult> _results = new List<RaycastResult>();
    private PointerEventData _pointerData;
    private string _lastLog;

    private void Update()
    {
        if (EventSystem.current == null || Mouse.current == null) return;

        if (_pointerData == null) _pointerData = new PointerEventData(EventSystem.current);
        _pointerData.position = Mouse.current.position.ReadValue();

        _results.Clear();
        EventSystem.current.RaycastAll(_pointerData, _results);

        string log = _results.Count == 0
            ? "[PointerDebug] Nada bajo el puntero"
            : "[PointerDebug] " + string.Join(" > ", _results.Select(result => result.gameObject.name));

        if (log == _lastLog) return;

        _lastLog = log;
        Debug.Log(log);
    }
}

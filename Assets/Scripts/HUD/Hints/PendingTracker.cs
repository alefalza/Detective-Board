/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: base para los sistemas que indican si hay algo pendiente.
 */

using System;
using UnityEngine;

public abstract class PendingTracker : MonoBehaviour
{
    public bool HasPending { get; private set; }

    public event Action<bool> PendingChanged;

    protected void SetPending(bool pending)
    {
        if (pending == HasPending) return;

        HasPending = pending;
        PendingChanged?.Invoke(pending);
    }
}

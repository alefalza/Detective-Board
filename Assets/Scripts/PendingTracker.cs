using System;
using UnityEngine;

/// <summary>Algo nuevo que el jugador todavía no vio. Lo consume ButtonHint_HUD.</summary>
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

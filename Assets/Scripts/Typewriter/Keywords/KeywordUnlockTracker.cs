/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: avisa cuando están todas las keywords y las mezcla para el panel del papel.
 */

using System;
using UnityEngine;

public class KeywordUnlockTracker : MonoBehaviour
{
    [SerializeField] private DeductionResolver deductions;

    private bool _notified;

    public event Action Completed;

    public bool IsComplete
    {
        get
        {
            var all = deductions.GetAllKeywords();
            if (all.Count == 0) return false;

            var available = TypewriterManager.Instance.AvailableKeywords;
            foreach (string keyword in all)
            {
                if (!available.Contains(keyword)) return false;
            }
            return true;
        }
    }

    private void Start() => TypewriterManager.Instance.KeywordAdded += OnKeywordAdded;

    private void OnKeywordAdded()
    {
        if (_notified || !IsComplete) return;

        _notified = true;

        TypewriterManager.Instance.ShuffleKeywords();
        Completed?.Invoke();
    }

    private void OnDestroy()
    {
        if (TypewriterManager.Instance != null) TypewriterManager.Instance.KeywordAdded -= OnKeywordAdded;
    }
}

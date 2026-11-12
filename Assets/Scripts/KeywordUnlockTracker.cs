using System;
using UnityEngine;

/// <summary>
/// Avisa cuando están disponibles todas las keywords de las deducciones (incluido el cheat).
/// </summary>
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
        Completed?.Invoke();
    }

    private void OnDestroy()
    {
        if (TypewriterManager.Instance != null) TypewriterManager.Instance.KeywordAdded -= OnKeywordAdded;
    }
}

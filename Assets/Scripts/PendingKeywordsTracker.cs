using System;
using UnityEngine;

public class PendingKeywordsTracker : MonoBehaviour
{
    [SerializeField] private ArticleValidator article;

    public bool HasPending { get; private set; }

    public event Action<bool> PendingChanged;

    private void Start()
    {
        TypewriterManager.Instance.KeywordAdded += Refresh;
        article.PlacementChanged += Refresh;
        Refresh();
    }

    private void Refresh()
    {
        bool pending = TypewriterManager.Instance.AvailableKeywords.Count > article.PlacedCount;
        if (pending == HasPending) return;

        HasPending = pending;
        PendingChanged?.Invoke(pending);
    }

    private void OnDestroy()
    {
        if (TypewriterManager.Instance != null) TypewriterManager.Instance.KeywordAdded -= Refresh;
        if (article != null) article.PlacementChanged -= Refresh;
    }
}

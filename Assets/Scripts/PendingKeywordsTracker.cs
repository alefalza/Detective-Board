public class PendingKeywordsTracker : PendingTracker
{
    private int _seenCount;

    private void Start()
    {
        TypewriterManager.Instance.KeywordAdded += Refresh;
        GameViewManager.Instance.StateChanged += OnViewChanged;
        Refresh();
    }

    private void OnViewChanged(GameView view) => Refresh();

    private void Refresh()
    {
        int available = TypewriterManager.Instance.AvailableKeywords.Count;

        // En la vista Paper, todas las keywords disponibles cuentan como vistas
        if (GameViewManager.Instance.CurrentView == GameView.Paper)
            _seenCount = available;

        SetPending(available > _seenCount);
    }

    private void OnDestroy()
    {
        if (TypewriterManager.Instance != null) TypewriterManager.Instance.KeywordAdded -= Refresh;
        if (GameViewManager.Instance != null) GameViewManager.Instance.StateChanged -= OnViewChanged;
    }
}

using UnityEngine;

public class PendingKeywordsTracker : PendingTracker
{
    [Tooltip("El hint no se muestra hasta que el botón de Typewriter esté habilitado.")]
    [SerializeField] private KeywordUnlockTracker unlockTracker;

    private int _seenCount;

    private void Start()
    {
        TypewriterManager.Instance.KeywordAdded += Refresh;
        GameViewManager.Instance.StateChanged += OnViewChanged;
        unlockTracker.Completed += Refresh;
        Refresh();
    }

    private void OnViewChanged(GameView view) => Refresh();

    private void Refresh()
    {
        int available = TypewriterManager.Instance.AvailableKeywords.Count;

        // En la vista Paper, todas las keywords disponibles cuentan como vistas
        if (GameViewManager.Instance.CurrentView == GameView.Paper)
            _seenCount = available;

        SetPending(unlockTracker.IsComplete && available > _seenCount);
    }

    private void OnDestroy()
    {
        if (TypewriterManager.Instance != null) TypewriterManager.Instance.KeywordAdded -= Refresh;
        if (GameViewManager.Instance != null) GameViewManager.Instance.StateChanged -= OnViewChanged;
        if (unlockTracker != null) unlockTracker.Completed -= Refresh;
    }
}

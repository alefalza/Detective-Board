/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: indica que se archivó la primera pista y todavía no se volvió al tablero.
 */

public class PendingCorkboardTracker : PendingTracker
{
    private bool _done;

    private void Start()
    {
        SceneryProgress.Instance.EvidenceArchived += OnEvidenceArchived;
        GameViewManager.Instance.TransitionStarted += OnTransitionStarted;
    }

    private void OnEvidenceArchived()
    {
        if (!_done) SetPending(true);
    }

    private void OnTransitionStarted(GameView view)
    {
        if (view != GameView.Corkboard || !HasPending) return;

        _done = true;
        SetPending(false);
    }

    private void OnDestroy()
    {
        if (SceneryProgress.Instance != null) SceneryProgress.Instance.EvidenceArchived -= OnEvidenceArchived;
        if (GameViewManager.Instance != null) GameViewManager.Instance.TransitionStarted -= OnTransitionStarted;
    }
}

/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: indica que el teléfono está disponible y todavía no se consultó nunca.
 */

public class PendingPhoneTracker : PendingTracker
{
    private bool _hasConsulted;

    private void Start()
    {
        CorkboardManager.Instance.CanConsultChanged += OnCanConsultChanged;
        GameViewManager.Instance.TransitionStarted += OnTransitionStarted;
    }

    private void OnCanConsultChanged(bool canConsult) => SetPending(canConsult && !_hasConsulted);

    private void OnTransitionStarted(GameView view)
    {
        if (view != GameView.Phone) return;

        _hasConsulted = true;
        SetPending(false);
    }

    private void OnDestroy()
    {
        if (CorkboardManager.Instance != null) CorkboardManager.Instance.CanConsultChanged -= OnCanConsultChanged;
        if (GameViewManager.Instance != null) GameViewManager.Instance.TransitionStarted -= OnTransitionStarted;
    }
}

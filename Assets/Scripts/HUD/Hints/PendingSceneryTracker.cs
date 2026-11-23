/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: indica si se desbloqueó algo en el Scenery que todavía no se vio.
 */

public class PendingSceneryTracker : PendingTracker
{
    private void Start()
    {
        SceneryProgress.Instance.InspectableUnlocked += OnInspectableUnlocked;
        GameViewManager.Instance.StateChanged += OnViewChanged;
    }

    private void OnInspectableUnlocked() => SetPending(true);

    private void OnViewChanged(GameView view)
    {
        if (view == GameView.Scenery) SetPending(false);
    }

    private void OnDestroy()
    {
        if (SceneryProgress.Instance != null) SceneryProgress.Instance.InspectableUnlocked -= OnInspectableUnlocked;
        if (GameViewManager.Instance != null) GameViewManager.Instance.StateChanged -= OnViewChanged;
    }
}

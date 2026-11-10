using UnityEngine;

/// <summary>
/// Solo deja interactuar con el Scenery con la cámara ya asentada en esa vista.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class SceneryInteraction : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    private void Awake() => _canvasGroup = GetComponent<CanvasGroup>();

    private void Start()
    {
        GameViewManager view = GameViewManager.Instance;
        view.TransitionStarted += OnTransitionStarted;
        view.StateChanged += OnStateChanged;

        SetInteractive(view.CurrentView == GameView.Scenery);
    }

    private void OnTransitionStarted(GameView view) => SetInteractive(false);

    private void OnStateChanged(GameView view) => SetInteractive(view == GameView.Scenery);

    private void SetInteractive(bool interactive)
    {
        _canvasGroup.interactable = interactive;
        _canvasGroup.blocksRaycasts = interactive;
    }

    private void OnDestroy()
    {
        if (GameViewManager.Instance != null)
        {
            GameViewManager.Instance.TransitionStarted -= OnTransitionStarted;
            GameViewManager.Instance.StateChanged -= OnStateChanged;
        }
    }
}

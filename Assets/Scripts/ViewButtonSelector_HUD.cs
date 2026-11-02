using System;
using UnityEngine;

[RequireComponent(typeof(ButtonScaler_UI))]
public class ViewButtonSelector_HUD : MonoBehaviour
{
    [SerializeField] private GameView[] selectedViews;

    private ButtonScaler_UI _scaler;

    private void Awake() => _scaler = GetComponent<ButtonScaler_UI>();

    private void Start()
    {
        GameViewManager view = GameViewManager.Instance;
        view.StateChanged += OnStateChanged;

        _scaler.SetSelected(IsSelected(view.CurrentView), animate: false);
    }

    private void OnStateChanged(GameView view) => _scaler.SetSelected(IsSelected(view));

    private bool IsSelected(GameView view) => Array.IndexOf(selectedViews, view) >= 0;

    private void OnDestroy()
    {
        if (GameViewManager.Instance != null)
            GameViewManager.Instance.StateChanged -= OnStateChanged;
    }
}

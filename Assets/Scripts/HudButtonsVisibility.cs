using System;
using UnityEngine;

public class HudButtonsVisibility : MonoBehaviour
{
    [SerializeField] private ButtonPivot_HUD[] pivots;
    [SerializeField] private GameView[] hiddenInViews = { GameView.Paper };

    private void Start()
    {
        GameViewManager view = GameViewManager.Instance;
        view.StateChanged += OnStateChanged;

        Apply(view.CurrentView);
    }

    private void OnStateChanged(GameView view) => Apply(view);

    private void Apply(GameView view)
    {
        bool hidden = Array.IndexOf(hiddenInViews, view) >= 0;

        foreach (ButtonPivot_HUD pivot in pivots)
            pivot.SetHiddenByView(hidden);
    }

    private void OnDestroy()
    {
        if (GameViewManager.Instance != null)
            GameViewManager.Instance.StateChanged -= OnStateChanged;
    }
}

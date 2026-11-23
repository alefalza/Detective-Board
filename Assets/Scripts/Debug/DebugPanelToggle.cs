/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: abre y cierra el panel de cheats con una tecla, y lo oculta en la vista Paper.
 */

using UnityEngine;
using UnityEngine.InputSystem;

public class DebugPanelToggle : MonoBehaviour
{
    [SerializeField] private GameObject debugPanel;
    [SerializeField] private Key toggleKey = Key.C;
    [SerializeField] private bool startVisible = false;
    [SerializeField] private bool allowInReleaseBuilds = false;

    private void Start()
    {
        debugPanel.SetActive(startVisible && IsAllowed());
        GameViewManager.Instance.TransitionStarted += OnTransitionStarted;
    }

    private void OnTransitionStarted(GameView view)
    {
        if (view == GameView.Paper) debugPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (GameViewManager.Instance != null)
            GameViewManager.Instance.TransitionStarted -= OnTransitionStarted;
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || !keyboard[toggleKey].wasPressedThisFrame || !IsAllowed()) return;

        debugPanel.SetActive(!debugPanel.activeSelf);
    }

    private bool IsAllowed() => allowInReleaseBuilds || Debug.isDebugBuild;
}

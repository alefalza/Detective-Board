// Abre y cierra el panel de cheats/debug con una tecla. Va en un objeto siempre activo, no en el panel.
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugPanelToggle : MonoBehaviour
{
    [SerializeField] private GameObject debugPanel;
    [SerializeField] private Key toggleKey = Key.C;
    [SerializeField] private bool startVisible = false;
    [Tooltip("Si está desactivado, el panel solo se puede abrir en el editor y en development builds.")]
    [SerializeField] private bool allowInReleaseBuilds = false;

    private void Start() => debugPanel.SetActive(startVisible && IsAllowed());

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || !keyboard[toggleKey].wasPressedThisFrame || !IsAllowed()) return;

        debugPanel.SetActive(!debugPanel.activeSelf);
    }

    private bool IsAllowed() => allowInReleaseBuilds || Debug.isDebugBuild;
}

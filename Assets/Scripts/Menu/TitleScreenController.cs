/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: menú principal: jugar, configuración, créditos y salir.
 */

using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "Gameplay";

    [Header("Paneles de la libreta")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    private void Start() => ShowPanel(mainPanel);

    public void OnPlayClicked() => SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
    public void OnSettingsClicked() => ShowPanel(settingsPanel);
    public void OnCreditsClicked() => ShowPanel(creditsPanel);
    public void OnBackClicked() => ShowPanel(mainPanel);

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ShowPanel(GameObject panel)
    {
        mainPanel.SetActive(panel == mainPanel);
        settingsPanel.SetActive(panel == settingsPanel);
        creditsPanel.SetActive(panel == creditsPanel);
    }
}

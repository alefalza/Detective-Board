using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "Gameplay";

    public void OnPlayClicked() => SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

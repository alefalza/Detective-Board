using UnityEngine;
using UnityEngine.InputSystem;

public class PhoneViewManager : MonoBehaviour
{
    [SerializeField] private KeywordTextPlayer keywordTextPlayer;
    [SerializeField] private DeductionPhoneBridge phoneBridge;
    [SerializeField] private string content;

    [Header("Visual References")]
    [SerializeField] private Collider2D notebookSpriteCollider;
    [SerializeField] private GameObject openedPhone;
    [SerializeField] private GameObject closedPhone;
    [SerializeField] private VignetteController vignetteController;
    [SerializeField] private float vignetteDuration = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource handwritingAudioSource;

    private Camera _cam;
    private bool _inPhone;

    private void Awake() => _cam = Camera.main;

    private void Start()
    {
        if (GameViewManager.Instance != null)
        {
            GameViewManager.Instance.TransitionStarted += OnTransitionStarted;
            GameViewManager.Instance.StateChanged += OnStateChanged;
        }

        SetPhoneSprite(false);
    }

    private void OnTransitionStarted(GameView view)
    {
        if (view == GameView.Phone)
        {
            _inPhone = true;
            vignetteController.TriggerVignette(true, vignetteDuration);
            return;
        }

        if (_inPhone && view == GameView.Corkboard)
            vignetteController.TriggerVignette(false, vignetteDuration);

        _inPhone = false;
    }

    private void OnStateChanged(GameView view)
    {
        if (view != GameView.Phone) return;

        string text = content;
        if (phoneBridge != null && phoneBridge.TryTakeText(out string deductionText))
            text = deductionText;

        GameViewManager.Instance.BeginSequence();
        // TODO: aca deberia esperar por la interaccion del usuario: "Consultar fuente"
        SetPhoneSprite(true);
        handwritingAudioSource.Play();
        keywordTextPlayer.Play(
            text,
            onFinished: () =>
            {
                SetPhoneSprite(false);
                GameViewManager.Instance.EndSequence();
                GameViewManager.Instance.SwitchToCorkboard();
            },
            skipRequested: IsNotebookSpriteClicked,
            onTypingFinished: handwritingAudioSource.Stop);
    }

    private void SetPhoneSprite(bool open)
    {
        openedPhone.SetActive(open);
        closedPhone.SetActive(!open);
    }

    private bool IsNotebookSpriteClicked()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || !mouse.leftButton.wasPressedThisFrame) return false;

        Vector2 worldPoint = _cam.ScreenToWorldPoint(mouse.position.ReadValue());
        return notebookSpriteCollider.OverlapPoint(worldPoint);
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

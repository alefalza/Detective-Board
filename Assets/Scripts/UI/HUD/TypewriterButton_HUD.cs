using UnityEngine;

[RequireComponent(typeof(ButtonScaler_UI))]
public class TypewriterButton_HUD : MonoBehaviour
{
    [Header("Bloqueo")]
    [SerializeField] private KeywordUnlockTracker unlockTracker;
    [Tooltip("Overlay gris que se apaga al desbloquear todas las keywords.")]
    [SerializeField] private GameObject lockedOverlay;

    [Header("Punch Settings")]
    [SerializeField] private Vector3 punchAmount = new Vector3(0.25f, 0.25f, 0f);
    [SerializeField] private float punchDuration = 0.35f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip receiveSound;

    private ButtonScaler_UI _scaler;
    private CanvasGroup _canvasGroup;
    private bool _pendingUnlock;

    private void Awake()
    {
        _scaler = GetComponent<ButtonScaler_UI>();

        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        unlockTracker.Completed += OnUnlockCompleted;
        GameViewManager.Instance.StateChanged += OnViewChanged;
        SetLocked(!unlockTracker.IsComplete);
    }

    // Se desbloquea al terminar de llegar al tablero; si ya está ahí, al instante
    private void OnUnlockCompleted()
    {
        GameViewManager view = GameViewManager.Instance;
        bool inCorkboard = view.CurrentView == GameView.Corkboard && !view.IsTransitioning;

        if (inCorkboard) Unlock();
        else _pendingUnlock = true;
    }

    private void OnViewChanged(GameView view)
    {
        if (view == GameView.Corkboard && _pendingUnlock) Unlock();
    }

    private void Unlock()
    {
        _pendingUnlock = false;
        SetLocked(false);
        //_scaler.Punch(punchAmount, punchDuration);
    }

    // Bloqueado: se ve gris y no recibe clicks (el overlay solo no alcanza: sus eventos suben al botón)
    private void SetLocked(bool locked)
    {
        lockedOverlay.SetActive(locked);
        _canvasGroup.blocksRaycasts = !locked;
    }

    public void OnKeywordReceived()
    {
        if (audioSource != null && receiveSound != null)
            audioSource.PlayOneShot(receiveSound);

        _scaler.Punch(punchAmount, punchDuration);
    }

    private void OnDestroy()
    {
        if (unlockTracker != null) unlockTracker.Completed -= OnUnlockCompleted;
        if (GameViewManager.Instance != null) GameViewManager.Instance.StateChanged -= OnViewChanged;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhoneViewManager : MonoBehaviour
{
    [SerializeField] private PhoneSequenceSettings settings;
    [SerializeField] private KeywordTextPlayer keywordTextPlayer;
    [SerializeField] private DeductionPhoneBridge phoneBridge;
    [SerializeField] private string content;

    [Header("Visual References")]
    [SerializeField] private Collider2D notebookSpriteCollider;
    [SerializeField] private GameObject openedPhone;
    [SerializeField] private GameObject closedPhone;
    [Tooltip("Botón que saltea la llamada y la escritura. Su OnClick debe llamar a SkipSequence.")]
    [SerializeField] private GameObject skipButton;
    [SerializeField] private VignetteController vignetteController;
    [SerializeField] private float vignetteDuration = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource phoneLineAudioSource;
    [SerializeField] private AudioSource handwritingAudioSource;

    private Camera _cam;
    private bool _inPhone;
    private bool _skipRequested;

    private void Awake() => _cam = Camera.main;

    private void Start()
    {
        if (GameViewManager.Instance != null)
        {
            GameViewManager.Instance.TransitionStarted += OnTransitionStarted;
            GameViewManager.Instance.StateChanged += OnStateChanged;
        }

        SetPhoneSprite(false);
        skipButton.SetActive(false);
    }

    // Asignar en el OnClick del botón de saltear
    public void SkipSequence()
    {
        _skipRequested = true;
        skipButton.SetActive(false);
        phoneLineAudioSource.Stop();
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
        if (view == GameView.Phone) StartCoroutine(PhoneSequence());
    }

    private IEnumerator PhoneSequence()
    {
        GameViewManager view = GameViewManager.Instance;
        view.BeginSequence();

        _skipRequested = false;
        skipButton.SetActive(true);

        // Descuelga: discado y tono
        SetPhoneSprite(true);
        PlayOneShot(settings.PickUpClip);
        yield return WaitOrSkip(settings.PickUpDuration);
        yield return PlayLineClip(settings.DialClip, settings.DialDuration);
        yield return PlayLineClip(settings.ToneClip, settings.ToneDuration);

        // Libreta
        if (!_skipRequested)
        {
            handwritingAudioSource.Play();
            StartLineClip(settings.MumblingClip);
        }

        keywordTextPlayer.Play(
            GetText(),
            skipRequested: IsTypingSkipRequested,
            onTypingFinished: OnTypingFinished);
        yield return new WaitUntil(() => !keywordTextPlayer.IsPlaying);

        // Cuelga y vuelve al tablero
        SetPhoneSprite(false);
        PlayOneShot(settings.HangUpClip);
        yield return new WaitForSeconds(settings.HangUpDelay);

        view.EndSequence();
        view.SwitchToCorkboard();
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (clip != null) phoneLineAudioSource.PlayOneShot(clip);
    }

    private IEnumerator PlayLineClip(AudioClip clip, float duration)
    {
        if (_skipRequested) yield break;

        StartLineClip(clip);

        yield return WaitOrSkip(duration);

        phoneLineAudioSource.Stop();
    }

    private IEnumerator WaitOrSkip(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration && !_skipRequested)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private bool IsTypingSkipRequested() => _skipRequested || IsNotebookSpriteClicked();

    // En loop para que el clip cubra todo el tiempo que deba sonar
    private void StartLineClip(AudioClip clip)
    {
        phoneLineAudioSource.clip = clip;
        phoneLineAudioSource.loop = true;
        phoneLineAudioSource.Play();
    }

    private void OnTypingFinished()
    {
        skipButton.SetActive(false);
        handwritingAudioSource.Stop();
        phoneLineAudioSource.Stop();
    }

    private string GetText()
    {
        if (phoneBridge != null && phoneBridge.TryTakeText(out string deductionText))
            return deductionText;

        return content;
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

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectionWindow : Singleton<InspectionWindow>
{
    [SerializeField] private SceneryViewerSettings settings;

    [Header("Referencias de UI")]
    [Tooltip("Raíz a pantalla completa: mientras está abierta bloquea lo que hay debajo.")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform window;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image image;
    [Tooltip("Texto del botón de cierre: \"Archivar\" o \"Cerrar\".")]
    [SerializeField] private TextMeshProUGUI closeButtonLabel;
    [SerializeField] private AudioSource archiveAudioSource;

    private const string ScaleTweenName = "InspectionWindowScale";
    private const string FadeTweenName = "InspectionWindowFade";

    private bool _isOpen;
    private InspectableData _current;

    public bool IsOpen => _isOpen;

    public event System.Action Opened;

    private void Start()
    {
        GameViewManager.Instance.TransitionStarted += OnTransitionStarted;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        window.localScale = Vector3.zero;
    }

    public void Show(InspectableData data)
    {
        _current = data;
        bool canArchive = SceneryProgress.Instance.CanArchive(data);
        closeButtonLabel.text = canArchive ? settings.ArchiveLabel : settings.CloseLabel;

        titleText.text = data.Title;
        descriptionText.text = data.Description;

        image.sprite = data.Image;
        image.gameObject.SetActive(data.Image != null);

        SetOpen(true);
    }

    // Asignar en el OnClick del botón: archiva la pista si todavía no se archivó, y cierra
    public void ArchiveAndClose()
    {
        if (_current != null && SceneryProgress.Instance.CanArchive(_current))
        {
            SceneryProgress.Instance.Archive(_current);
            if (settings.ArchiveClip != null) archiveAudioSource.PlayOneShot(settings.ArchiveClip);
        }

        Close();
    }

    // Asignar en el OnClick del fondo: cierra sin archivar
    public void Close() => SetOpen(false);

    private void OnTransitionStarted(GameView view) => Close();

    private void SetOpen(bool open)
    {
        if (open == _isOpen) return;
        _isOpen = open;

        canvasGroup.interactable = open;
        canvasGroup.blocksRaycasts = open;

        if (open) Opened?.Invoke();

        iTween.StopByName(window.gameObject, ScaleTweenName);
        iTween.ScaleTo(window.gameObject, iTween.Hash(
            "name", ScaleTweenName,
            "scale", open ? Vector3.one : Vector3.zero,
            "time", settings.WindowDuration,
            "easetype", open ? settings.WindowOpenEase : settings.WindowCloseEase,
            "ignoretimescale", true
        ));

        iTween.StopByName(canvasGroup.gameObject, FadeTweenName);
        iTween.ValueTo(canvasGroup.gameObject, iTween.Hash(
            "name", FadeTweenName,
            "from", canvasGroup.alpha,
            "to", open ? 1f : 0f,
            "time", settings.WindowDuration,
            "ignoretimescale", true,
            "onupdate", nameof(OnFadeUpdate),
            "onupdatetarget", gameObject
        ));
    }

    private void OnFadeUpdate(float alpha) => canvasGroup.alpha = alpha;

    private void OnDestroy()
    {
        if (GameViewManager.Instance != null)
            GameViewManager.Instance.TransitionStarted -= OnTransitionStarted;
    }
}

/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: objeto clickeable de la escena, con cursor, ventana de detalles, brillos y aparición con fade.
 */

using UnityEngine;
using UnityEngine.EventSystems;

public class SceneryInspectable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private InspectableData data;
    [SerializeField] private bool startsUnlocked = true;
    [SerializeField] private PointSparkle[] sparkles;

    private const string RevealTweenName = "InspectableReveal";

    private CanvasGroup _canvasGroup;
    private float _revealDuration;
    private bool _isPendingReveal;
    private bool _isRevealing;

    public InspectableData Data => data;
    public bool StartsUnlocked => startsUnlocked;

    public bool IsNewlyUnlocked { get; private set; }

    public bool IsCollected => data.HasEvidence ? !SceneryProgress.Instance.CanArchive(data) : _wasOpened;

    private bool _wasOpened;

    public bool IsSparkling
    {
        get
        {
            foreach (PointSparkle sparkle in sparkles)
            {
                if (sparkle.IsPlaying) return true;
            }
            return false;
        }
    }

    public void PlaySparkles(SparkleSettings settings)
    {
        IsNewlyUnlocked = false;
        foreach (PointSparkle sparkle in sparkles) sparkle.Play(settings);
    }

    public void StopSparkles()
    {
        foreach (PointSparkle sparkle in sparkles) sparkle.Stop();
    }

    public void Unlock(float revealDuration)
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _revealDuration = revealDuration;
        _isPendingReveal = true;
        IsNewlyUnlocked = true;

        gameObject.SetActive(true);
    }

    public bool TryReveal()
    {
        if (!_isPendingReveal || !gameObject.activeInHierarchy) return false;

        GameViewManager view = GameViewManager.Instance;
        if (view.CurrentView != GameView.Scenery || view.IsTransitioning) return false;

        _isPendingReveal = false;
        _isRevealing = true;
        _canvasGroup.blocksRaycasts = true;

        iTween.ValueTo(gameObject, iTween.Hash(
            "name", RevealTweenName,
            "from", 0f,
            "to", 1f,
            "time", _revealDuration,
            "easetype", iTween.EaseType.easeInOutSine,
            "ignoretimescale", true,
            "onupdate", nameof(OnRevealUpdate),
            "oncomplete", nameof(FinishReveal)
        ));

        return true;
    }

    private void OnRevealUpdate(float alpha) => _canvasGroup.alpha = alpha;

    private void FinishReveal()
    {
        _isRevealing = false;
        _canvasGroup.alpha = 1f;
    }

    private void OnEnable() => TryReveal();

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorManager.Instance.Request(this, GameCursorType.Mark);
    }

    public void OnPointerExit(PointerEventData eventData) => ReleaseCursor();

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        _wasOpened = true;
        InspectionWindow.Instance.Show(data);
    }

    private void OnDisable()
    {
        ReleaseCursor();

        if (!_isRevealing) return;

        iTween.StopByName(gameObject, RevealTweenName);
        FinishReveal();
    }

    private void ReleaseCursor()
    {
        if (CursorManager.Instance != null)
            CursorManager.Instance.Release(this);
    }
}

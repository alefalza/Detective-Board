using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Objeto de una slide: requiere un Graphic con Raycast Target (por ejemplo, una Image transparente).
/// </summary>
public class SceneryInspectable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private InspectableData data;
    [Tooltip("Si está desactivado, permanece oculto hasta que una deducción lo desbloquee.")]
    [SerializeField] private bool startsUnlocked = true;
    [SerializeField] private PointSparkle[] sparkles;

    private const string RevealTweenName = "InspectableReveal";

    private CanvasGroup _canvasGroup;
    private float _revealDuration;
    private bool _isPendingReveal;
    private bool _isRevealing;

    public InspectableData Data => data;
    public bool StartsUnlocked => startsUnlocked;

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
        foreach (PointSparkle sparkle in sparkles) sparkle.Play(settings);
    }

    public void StopSparkles()
    {
        foreach (PointSparkle sparkle in sparkles) sparkle.Stop();
    }

    // ── Desbloqueo ───────────────────────────────────────────────────

    // Queda activo pero invisible hasta que el jugador pueda ver su slide
    public void Unlock(float revealDuration)
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _revealDuration = revealDuration;
        _isPendingReveal = true;

        gameObject.SetActive(true);
    }

    // Se intenta al llegar a la Scenery View y al activarse la slide
    public void TryReveal()
    {
        if (!_isPendingReveal || !gameObject.activeInHierarchy) return;

        GameViewManager view = GameViewManager.Instance;
        if (view.CurrentView != GameView.Scenery || view.IsTransitioning) return;

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
    }

    private void OnRevealUpdate(float alpha) => _canvasGroup.alpha = alpha;

    private void FinishReveal()
    {
        _isRevealing = false;
        _canvasGroup.alpha = 1f;
    }

    private void OnEnable() => TryReveal();

    // ── Puntero ──────────────────────────────────────────────────────

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorManager.Instance.Request(this, GameCursorType.Mark);
    }

    public void OnPointerExit(PointerEventData eventData) => ReleaseCursor();

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        bool isNewEvidence = SceneryProgress.Instance.CollectEvidence(data);
        InspectionWindow.Instance.Show(data, isNewEvidence);
    }

    // Al desactivarse la slide no llega OnPointerExit ni termina el fade
    private void OnDisable()
    {
        ReleaseCursor();

        if (!_isRevealing) return;

        iTween.StopByName(gameObject, RevealTweenName);
        FinishReveal();
    }

    private void ReleaseCursor()
    {
        if (CursorManager.Instance != null) CursorManager.Instance.Release(this);
    }
}

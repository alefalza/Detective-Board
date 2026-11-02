using System;
using System.Collections;
using UnityEngine;
using EaseType = iTween.EaseType;

[RequireComponent(typeof(ButtonScaler_UI))]
public class ButtonHint_HUD : MonoBehaviour
{
    [SerializeField] private PendingKeywordsTracker tracker;
    [SerializeField] private GameView[] activeViews = { GameView.Corkboard };

    [Header("Heartbeat Settings")]
    [Min(0.5f)] [SerializeField] private float heartbeatInterval = 4f;
    [SerializeField] private Vector3 beatAmount = new Vector3(0.2f, 0.2f, 0f);
    [SerializeField] private float beatTime = 0.25f;
    [SerializeField] private float beatGap = 0.2f;

    [Header("Arrow Settings")]
    [SerializeField] private RectTransform arrow;
    [SerializeField] private float jumpHeight = 25f;
    [SerializeField] private float jumpTime = 0.4f;
    [SerializeField] private EaseType jumpEase = EaseType.easeInOutQuad;

    private const string ArrowTweenName = "HintArrow";

    private ButtonScaler_UI _scaler;
    private Vector3 _arrowRestPosition;
    private GameView _settledView;
    private bool _isTransitioning;
    private Coroutine _heartbeatRoutine;
    private bool _isShowing;

    private void Awake()
    {
        _scaler = GetComponent<ButtonScaler_UI>();

        if (arrow != null)
        {
            _arrowRestPosition = arrow.localPosition;
            arrow.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (_isShowing) Show();
    }

    private void Start()
    {
        GameViewManager view = GameViewManager.Instance;
        _settledView = view.CurrentView;
        view.TransitionStarted += OnTransitionStarted;
        view.StateChanged += OnStateChanged;
        
        if (tracker != null) tracker.PendingChanged += OnPendingChanged;

        Refresh();
    }

    private void OnTransitionStarted(GameView newView)
    {
        _isTransitioning = true;
        Refresh();
    }

    private void OnStateChanged(GameView view)
    {
        _isTransitioning = false;
        _settledView = view;
        Refresh();
    }

    private void OnPendingChanged(bool pending) => Refresh();

    private void Refresh()
    {
        bool shouldShow = tracker != null &&
                          tracker.HasPending &&
                          !_isTransitioning &&
                          Array.IndexOf(activeViews, _settledView) >= 0;

        if (shouldShow == _isShowing) return;

        _isShowing = shouldShow;
        if (shouldShow) Show();
        else Hide();
    }

    private void Show()
    {
        if (_heartbeatRoutine == null) _heartbeatRoutine = StartCoroutine(HeartbeatLoop());

        if (arrow == null) return;

        arrow.gameObject.SetActive(true);
        arrow.localPosition = _arrowRestPosition;

        iTween.StopByName(arrow.gameObject, ArrowTweenName);
        iTween.MoveTo(arrow.gameObject, iTween.Hash(
            "name", ArrowTweenName,
            "y", _arrowRestPosition.y + jumpHeight,
            "islocal", true,
            "time", jumpTime,
            "easetype", jumpEase,
            "looptype", iTween.LoopType.pingPong,
            "ignoretimescale", true
        ));
    }

    private void Hide()
    {
        if (_heartbeatRoutine != null)
        {
            StopCoroutine(_heartbeatRoutine);
            _heartbeatRoutine = null;
        }

        if (arrow == null) return;

        iTween.StopByName(arrow.gameObject, ArrowTweenName);
        arrow.localPosition = _arrowRestPosition;
        arrow.gameObject.SetActive(false);
    }

    private IEnumerator HeartbeatLoop()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(heartbeatInterval);

            _scaler.Punch(beatAmount, beatTime);
            yield return new WaitForSecondsRealtime(beatGap);
            _scaler.Punch(beatAmount * 0.7f, beatTime);
        }
    }

    private void OnDisable() => Hide();

    private void OnDestroy()
    {
        if (GameViewManager.Instance != null)
        {
            GameViewManager.Instance.TransitionStarted -= OnTransitionStarted;
            GameViewManager.Instance.StateChanged -= OnStateChanged;
        }

        if (tracker != null) tracker.PendingChanged -= OnPendingChanged;
    }    
}

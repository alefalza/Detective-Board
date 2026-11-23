/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: maneja las vistas Typewriter y Paper, y las keywords disponibles.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class TypewriterManager : Singleton<TypewriterManager>
{
    [SerializeField] private TypewriterPanels_UI panelsUI;
    [SerializeField] private TypewriterPaper_UI paperUI;

    [Header("Interactive Zone")]
    [SerializeField] private RectTransform paperHotspot;
    [SerializeField] private RectTransform paperCloseTarget;
    [SerializeField] private Camera canvasCamera;

    [Header("Bypass")]
    [SerializeField] private bool bypassTypewriterView = true;

    [SerializeField] private bool allowClosePaper = false;

    private List<string> _availableKeywords = new List<string>();
    private GameCursorType? _hoverCursor;
    private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();
    private PointerEventData _pointerData;
    private GameViewManager _gameViewManager;
    private GameView _lastView;
    private GameView _settledView;
    private GameView? _bypassTarget;

    public List<string> AvailableKeywords => _availableKeywords;

    public event System.Action KeywordAdded;

    public void AddKeyword(string keyword)
    {
        if (!_availableKeywords.Contains(keyword))
        {
            _availableKeywords.Add(keyword);
            KeywordAdded?.Invoke();
        }
    }

    public void ShuffleKeywords()
    {
        if (_availableKeywords.Count < 2) return;

        var original = new List<string>(_availableKeywords);
        do
        {
            for (int i = _availableKeywords.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (_availableKeywords[i], _availableKeywords[j]) = (_availableKeywords[j], _availableKeywords[i]);
            }
        }
        while (IsSameOrder(original, _availableKeywords));
    }

    private static bool IsSameOrder(List<string> a, List<string> b)
    {
        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }

    public void AddKeywords(IEnumerable<string> keywords)
    {
        foreach (string keyword in keywords) AddKeyword(keyword);

        if (_gameViewManager.CurrentView == GameView.Paper)
            panelsUI.RefreshUIKeywords();
    }

    private void Start()
    {
        _gameViewManager = GameViewManager.Instance;

        if (_gameViewManager != null)
        {
            _gameViewManager.TransitionStarted += OnTransitionStarted;
            _gameViewManager.StateChanged += OnStateChanged;
        }

        _lastView = _gameViewManager.CurrentView;
        _settledView = _gameViewManager.CurrentView;
        SetCloseTargetActive(false);
    }

    private void OnStateChanged(GameView view)
    {
        GameView? target = view == GameView.Typewriter ? _bypassTarget : null;

        _bypassTarget = null;
        _settledView = view;

        if (target == GameView.Paper) _gameViewManager.SwitchToPaper();
        else if (target == GameView.Corkboard) _gameViewManager.SwitchToCorkboard();
    }

    private GameView? GetBypassTarget(GameView newView)
    {
        if (!bypassTypewriterView || newView != GameView.Typewriter) return null;

        if (_settledView == GameView.Corkboard) return GameView.Paper;
        if (_settledView == GameView.Paper) return GameView.Corkboard;
        return null;
    }

    private void SetCloseTargetActive(bool active)
    {
        if (paperCloseTarget != null) paperCloseTarget.gameObject.SetActive(active);
    }

    private void OnTransitionStarted(GameView newView)
    {
        _bypassTarget = GetBypassTarget(newView);
        bool open = newView == GameView.Paper;

        if (newView == GameView.Paper ||
            _lastView == GameView.Paper && newView == GameView.Typewriter)
        {
            _lastView = newView;

            open = newView == GameView.Paper;

            if (open)
            {
                panelsUI.RefreshUIKeywords();
            }

            panelsUI.Slide(open);
            paperUI.ScaleCenter(open);
            paperUI.FadeCenter(open);
        }

        SetCloseTargetActive(open && allowClosePaper);
    }

    private void Update()
    {
        GameView activeView = _gameViewManager != null ? _gameViewManager.CurrentView : GameView.Corkboard;
        bool inPaper = activeView == GameView.Paper;

        if (_gameViewManager == null
            || (activeView != GameView.Typewriter && !inPaper)
            || _gameViewManager.IsTransitioning
            || (inPaper && !allowClosePaper))
        {
            SetHover(null);
            return;
        }

        GameView current = activeView;

        bool hovering = inPaper ? IsPointerOverCloseTarget() : IsPointerOverPaperHotspot();

        if (!hovering) SetHover(null);
        else SetHover(GameCursorType.Magnifier);

        if (!hovering || !WasClickedThisFrame() || KeywordChip_UI.Dragged != null) return;

        SetHover(null);

        if (current == GameView.Typewriter)
            _gameViewManager.SwitchToPaper();
        else
            _gameViewManager.SwitchToTypewriter();
    }

    private bool IsPointerOverPaperHotspot()
    {
        return paperHotspot != null
               && RectTransformUtility.RectangleContainsScreenPoint(paperHotspot, GetPointerPosition(), canvasCamera);
    }

    private bool IsPointerOverCloseTarget()
    {
        if (paperCloseTarget == null || EventSystem.current == null) return false;

        if (_pointerData == null) _pointerData = new PointerEventData(EventSystem.current);
        _pointerData.position = GetPointerPosition();

        _raycastResults.Clear();
        EventSystem.current.RaycastAll(_pointerData, _raycastResults);

        return _raycastResults.Count > 0
               && _raycastResults[0].gameObject.transform.IsChildOf(paperCloseTarget);
    }

    private void SetHover(GameCursorType? cursor)
    {
        if (cursor == _hoverCursor) return;
        _hoverCursor = cursor;

        if (cursor.HasValue)
            CursorManager.Instance.Request(this, cursor.Value);
        else
            CursorManager.Instance.Release(this);
    }

    private void OnDisable()
    {
        SetHover(null);
    }

    private void OnDestroy()
    {
        if (_gameViewManager != null)
        {
            _gameViewManager.TransitionStarted -= OnTransitionStarted;
            _gameViewManager.StateChanged -= OnStateChanged;
        }
    }

    private static Vector2 GetPointerPosition()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
        return Input.mousePosition;
#endif
    }

    private static bool WasClickedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }
}

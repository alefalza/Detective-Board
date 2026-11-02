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

    [Header("Zona interactiva (papel)")]
    [Tooltip("Click en esta zona abre el Paper (desde la Typewriter).")]
    [SerializeField] private RectTransform paperHotspot;
    [Tooltip("Imagen de UI (con Raycast Target) cuyo click cierra el Paper.")]
    [SerializeField] private RectTransform paperCloseTarget;
    [Tooltip("Cámara del Canvas. Dejar vacío si el Canvas es Screen Space - Overlay.")]
    [SerializeField] private Camera canvasCamera;

    [Header("Bypass")]
    [Tooltip("Si está activo, la vista Typewriter es solo de paso: Corkboard → Typewriter → Paper al entrar, y Paper → Typewriter → Corkboard al salir.")]
    [SerializeField] private bool bypassTypewriterView = true;

    private List<string> _availableKeywords = new List<string>();
    private GameCursorType? _hoverCursor;   // cursor que este manager tiene pedido (null = ninguno)
    private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();
    private PointerEventData _pointerData;
    private GameViewManager _gameViewManager;
    private GameView _lastView;
    private GameView _settledView;      // última vista en la que terminó una transición
    private GameView? _bypassTarget;  // vista a la que se sigue solo al llegar a la Typewriter

    public List<string> AvailableKeywords => _availableKeywords;

    // Se dispara cuando se suma una keyword que no estaba disponible
    public event System.Action KeywordAdded;

    public void AddKeyword(string keyword)
    {
        if (!_availableKeywords.Contains(keyword))
        {
            _availableKeywords.Add(keyword);
            KeywordAdded?.Invoke();
        }
    }

    public void AddKeywords(IEnumerable<string> keywords)
    {
        foreach (string keyword in keywords) AddKeyword(keyword);

        // Si el panel ya está abierto, se refresca; si no, se arma al abrir la vista Paper
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
    }

    private void OnStateChanged(GameView view)
    {
        GameView? target = view == GameView.Typewriter ? _bypassTarget : null;

        _bypassTarget = null;
        _settledView = view;

        // Ya en la Typewriter, sigue de largo hacia el otro extremo
        if (target == GameView.Paper) _gameViewManager.SwitchToPaper();
        else if (target == GameView.Corkboard) _gameViewManager.SwitchToCorkboard();
    }

    // Corkboard → Typewriter sigue al Paper; Paper → Typewriter sigue al Corkboard
    private GameView? GetBypassTarget(GameView newView)
    {
        if (!bypassTypewriterView || newView != GameView.Typewriter) return null;

        if (_settledView == GameView.Corkboard) return GameView.Paper;
        if (_settledView == GameView.Paper) return GameView.Corkboard;
        return null;
    }

    private void OnTransitionStarted(GameView newView)
    {
        _bypassTarget = GetBypassTarget(newView);

        if (newView == GameView.Paper ||
            _lastView == GameView.Paper && newView == GameView.Typewriter)
        {
            _lastView = newView;

            bool open = newView == GameView.Paper;

            if (open)
            {
                panelsUI.RefreshUIKeywords();
            }

            panelsUI.Slide(open);
            paperUI.ScaleCenter(open);
            paperUI.FadeCenter(open);
        }
    }

    private void Update()
    {
        if (_gameViewManager == null
            || _gameViewManager.CurrentView == GameView.Corkboard
            || _gameViewManager.IsTransitioning)
        {
            SetHover(null);
            return;
        }

        GameView current = _gameViewManager.CurrentView;
        bool inPaper = current == GameView.Paper;

        // En Paper se cierra clickeando la otra imagen; en el resto, el papel abre
        bool hovering = inPaper ? IsPointerOverCloseTarget() : IsPointerOverPaperHotspot();

        if (!hovering) SetHover(null);
        else SetHover(GameCursorType.Magnifier);

        // Un click que agarra una keyword (EventSystem corre antes y ya la marcó como Dragged) no cuenta como click
        if (!hovering || !WasClickedThisFrame() || KeywordChipUI.Dragged != null) return;

        SetHover(null);

        if (current == GameView.Typewriter)
            _gameViewManager.SwitchToPaper();
        else
            _gameViewManager.SwitchToTypewriter(); // desde Paper cierra (con bypass sigue al Corkboard)
    }

    private bool IsPointerOverPaperHotspot()
    {
        return paperHotspot != null
               && RectTransformUtility.RectangleContainsScreenPoint(paperHotspot, GetPointerPosition(), canvasCamera);
    }

    // Raycast de UI: cuenta solo si lo que está más arriba bajo el puntero es la imagen (o un hijo suyo),
    // así el papel y las keywords que estén encima la tapan
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

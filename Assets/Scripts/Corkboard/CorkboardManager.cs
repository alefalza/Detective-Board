/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: maneja el input del tablero: arrastre de pistas, conexión de hilos,
 * cursores y consulta de la fuente.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CorkboardManager : Singleton<CorkboardManager>
{
    [SerializeField] private Camera mainCamera;

    [Header("Modules")]
    [SerializeField] private ThreadController threads;
    [SerializeField] private DeductionResolver deductions;
    [SerializeField] private DeductionPhoneBridge phoneBridge;

    [Header("Input System Asset")]
    [SerializeField] private InputActionAsset inputActions;

    private InputAction clickAction;
    private InputAction rightClickAction;
    private InputAction cursorPositionAction;

    private EvidenceItem draggedItem;
    private EvidenceItem selectedPinItemA;
    private EvidenceItem lastConnectedItem;

    private readonly object _hoverCursorOwner = new object();
    private GameCursorType? _hoverCursor;
    private EvidenceItem _hoveredPinItem;

    public bool CanConsult { get; private set; }

    public event System.Action<bool> CanConsultChanged;

    protected override void Awake()
    {
        base.Awake();

        var boardMap = inputActions.FindActionMap("Board");
        clickAction = boardMap.FindAction("Click");
        rightClickAction = boardMap.FindAction("RightClick");
        cursorPositionAction = boardMap.FindAction("CursorPosition");
    }

    private void OnEnable()
    {
        inputActions.Enable();
        clickAction.started += OnClickStarted;
        clickAction.canceled += OnClickCanceled;
        rightClickAction.performed += OnRightClickPerformed;
        threads.ConnectionsChanged += RefreshCanConsult;
    }

    private void Start() => GameViewManager.Instance.TransitionStarted += OnTransitionStarted;

    private void OnTransitionStarted(GameView view) => CancelConnection();

    private void OnDestroy()
    {
        if (GameViewManager.Instance != null)
            GameViewManager.Instance.TransitionStarted -= OnTransitionStarted;
    }

    private void OnDisable()
    {
        threads.ConnectionsChanged -= RefreshCanConsult;
        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.Release(this);
            CursorManager.Instance.Release(_hoverCursorOwner);
        }
        _hoverCursor = null;
        SetHoveredPin(null);
        clickAction.started -= OnClickStarted;
        clickAction.canceled -= OnClickCanceled;
        rightClickAction.performed -= OnRightClickPerformed;
        inputActions.Disable();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //SceneManager.LoadScene("GameplayScene");
        }

        Vector3 mouseWorldPos = GetMouseWorldPosition();

        if (draggedItem != null)
            draggedItem.Drag(mouseWorldPos);

        if (selectedPinItemA != null)
            threads.ShowActiveThread(selectedPinItemA.GetPinPosition(), mouseWorldPos);

        bool canHoverThread = !IsInputBlocked() && selectedPinItemA == null && draggedItem == null;
        threads.UpdateHoverMarker(mouseWorldPos, canHoverThread);

        UpdateItemHoverCursor(mouseWorldPos, !IsInputBlocked() && draggedItem == null);
    }

    private void UpdateItemHoverCursor(Vector3 mouseWorldPos, bool canHover)
    {
        EvidenceItem pinItem = null;
        GameCursorType? wanted = canHover ? GetHoverCursor(mouseWorldPos, out pinItem) : null;

        SetHoveredPin(pinItem);

        if (wanted == _hoverCursor) return;

        _hoverCursor = wanted;
        if (CursorManager.Instance == null) return;

        if (wanted.HasValue) CursorManager.Instance.Request(_hoverCursorOwner, wanted.Value);
        else CursorManager.Instance.Release(_hoverCursorOwner);
    }

    private void SetHoveredPin(EvidenceItem item)
    {
        if (item == _hoveredPinItem) return;

        if (_hoveredPinItem != null) _hoveredPinItem.SetPinHovered(false);
        _hoveredPinItem = item;
        if (_hoveredPinItem != null) _hoveredPinItem.SetPinHovered(true);
    }

    private GameCursorType? GetHoverCursor(Vector3 mouseWorldPos, out EvidenceItem hoveredPin)
    {
        hoveredPin = null;

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
        if (hit.collider == null) return null;

        EvidenceItem item = hit.collider.GetComponent<EvidenceItem>();
        if (item == null) return null;

        if (!item.IsClickingPin(mouseWorldPos)) return GameCursorType.Hover;

        if (!CanUsePin(item)) return null;

        hoveredPin = item;
        return GameCursorType.Pin;
    }

    private bool CanUsePin(EvidenceItem item) => !threads.IsLocked(item) && !item.IsInvalidated;

    private void OnClickStarted(InputAction.CallbackContext context)
    {
        if (IsInputBlocked()) return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
        if (hit.collider == null) return;

        EvidenceItem item = hit.collider.GetComponent<EvidenceItem>();
        if (item == null) return;

        if (item.IsClickingPin(mouseWorldPos))
        {
            HandlePinClick(item);
        }
        else
        {
            draggedItem = item;
            draggedItem.BeginDrag(mouseWorldPos);
            CursorManager.Instance.Request(this, GameCursorType.Grab);
        }
    }

    private void OnClickCanceled(InputAction.CallbackContext context)
    {
        if (draggedItem == null) return;

        draggedItem.EndDrag();
        draggedItem = null;
        CursorManager.Instance.Release(this);
    }

    private void OnRightClickPerformed(InputAction.CallbackContext context)
    {
        if (IsInputBlocked()) return;

        if (selectedPinItemA != null)
        {
            CancelConnection();
            return;
        }

        threads.TryRemoveAt(GetMouseWorldPosition());
    }

    private void HandlePinClick(EvidenceItem item)
    {
        if (!CanUsePin(item)) return;

        if (selectedPinItemA == null)
        {
            selectedPinItemA = item;
            return;
        }

        if (selectedPinItemA != item)
            TryConnect(selectedPinItemA, item);

        CancelConnection();
    }

    private void TryConnect(EvidenceItem a, EvidenceItem b)
    {
        if (threads.HasConnection(a, b)) return;

        threads.CreateConnection(a, b);
        lastConnectedItem = b;
    }

    public void ConsultSource()
    {
        if (IsInputBlocked()) return;

        CancelConnection();

        List<List<EvidenceItem>> groups = threads.GetGroupsOfSize(deductions.GroupSize);
        if (groups.Count == 0) return;

        List<EvidenceItem> group = PickGroup(groups);
        phoneBridge.Present(deductions.Evaluate(group));
    }

    private void RefreshCanConsult()
    {
        bool canConsult = threads.GetGroupsOfSize(deductions.GroupSize).Count > 0;
        if (canConsult == CanConsult) return;

        CanConsult = canConsult;
        CanConsultChanged?.Invoke(canConsult);
    }

    private List<EvidenceItem> PickGroup(List<List<EvidenceItem>> groups)
    {
        foreach (List<EvidenceItem> group in groups)
        {
            if (lastConnectedItem != null && group.Contains(lastConnectedItem)) return group;
        }
        return groups[0];
    }

    public void OnItemMoved(EvidenceItem item) => threads.OnItemMoved(item);

    private void CancelConnection()
    {
        selectedPinItemA = null;
        threads.HideActiveThread();
    }

    private bool IsInputBlocked()
    {
        bool messageOpen = GameMessageManager.Instance != null && GameMessageManager.Instance.IsOpen;

        GameViewManager view = GameViewManager.Instance;
        bool viewBlocked = view != null && (view.IsTransitioning || view.CurrentView != GameView.Corkboard);

        return messageOpen || viewBlocked;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 screenPos = cursorPositionAction.ReadValue<Vector2>();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;
        return worldPos;
    }
}

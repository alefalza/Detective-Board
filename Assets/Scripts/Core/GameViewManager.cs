/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: controla la vista inicial, las vistas del juego y sus transiciones de cámara,
 * y bloquea los cambios durante las secuencias.
 */

using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using System;

public enum GameView { Scenery, Corkboard, Phone, Typewriter, Paper }

public class GameViewManager : Singleton<GameViewManager>
{
    [SerializeField] private CinemachineBrain brain;

    [Header("Initial View")]
    [SerializeField] private GameView startView = GameView.Scenery;

    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineCamera sceneryCam;
    [SerializeField] private CinemachineCamera corkboardCam;
    [SerializeField] private CinemachineCamera phoneCam;
    [SerializeField] private CinemachineCamera typewriterCam;
    [SerializeField] private CinemachineCamera paperCam;

    private const int BlendStartTimeoutFrames = 3;
    private const int ActivePriority = 10;
    private const int InactivePriority = 5;

    public bool IsTransitioning { get; private set; }

    public bool IsSequenceRunning { get; private set; }

    private bool IsLocked => IsTransitioning || IsSequenceRunning;
    public GameView CurrentView { get; private set; } = GameView.Corkboard;

    public event Action<GameView> TransitionStarted;
    public event Action<GameView> StateChanged;

    protected override void Awake()
    {
        base.Awake();

        CurrentView = startView;
        SetPriorities(startView);
    }

    public void BeginSequence() => IsSequenceRunning = true;

    public void EndSequence() => IsSequenceRunning = false;

    public void ToggleView()
    {
        if (CurrentView == GameView.Corkboard) SwitchToTypewriter();
        else if (CurrentView != GameView.Corkboard && CurrentView != GameView.Paper) SwitchToCorkboard();
    }

    public void SwitchToScenery()
    {
        if (IsLocked || CurrentView != GameView.Corkboard) return;

        StartTransition(GameView.Scenery);
    }

    public void SwitchToPhone()
    {
        if (IsLocked || CurrentView != GameView.Corkboard) return;

        StartTransition(GameView.Phone);
    }

    public void SwitchToTypewriter()
    {
        if (IsLocked || (CurrentView != GameView.Corkboard && CurrentView != GameView.Paper)) return;

        if (EnvironmentLightingManager.Instance != null)
            EnvironmentLightingManager.Instance.SetLightingForTypewriter();
        StartTransition(GameView.Typewriter);
    }

    public void SwitchToPaper()
    {
        if (IsLocked || CurrentView == GameView.Paper) return;

        if (EnvironmentLightingManager.Instance != null)
            EnvironmentLightingManager.Instance.SetLightingForPaper();
        StartTransition(GameView.Paper);
    }

    public void SwitchToCorkboard()
    {
        if (IsLocked || CurrentView == GameView.Corkboard || CurrentView == GameView.Paper) return;

        if (EnvironmentLightingManager.Instance != null)
            EnvironmentLightingManager.Instance.SetLightingForCorkboard();
        StartTransition(GameView.Corkboard);
    }

    private void StartTransition(GameView view)
    {
        CurrentView = view;
        SetPriorities(view);
        StartCoroutine(TransitionRoutine());
    }

    private void SetPriorities(GameView view)
    {
        sceneryCam.Priority = view == GameView.Scenery ? ActivePriority : InactivePriority;
        corkboardCam.Priority = view == GameView.Corkboard ? ActivePriority : InactivePriority;
        phoneCam.Priority = view == GameView.Phone ? ActivePriority : InactivePriority;
        typewriterCam.Priority = view == GameView.Typewriter ? ActivePriority : InactivePriority;
        paperCam.Priority = view == GameView.Paper ? ActivePriority : InactivePriority;
    }

    private IEnumerator TransitionRoutine()
    {
        IsTransitioning = true;
        TransitionStarted?.Invoke(CurrentView);

        int waitedFrames = 0;
        do
        {
            yield return null;
            waitedFrames++;
        }
        while (brain != null && !brain.IsBlending && waitedFrames < BlendStartTimeoutFrames);

        while (brain != null && brain.IsBlending)
            yield return null;

        IsTransitioning = false;
        StateChanged?.Invoke(CurrentView);
    }
}

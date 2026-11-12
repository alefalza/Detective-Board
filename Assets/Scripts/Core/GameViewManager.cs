using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using System;

public enum GameView { Scenery, Corkboard, Phone, Typewriter, Paper }

public class GameViewManager : Singleton<GameViewManager>
{
    [SerializeField] private CinemachineBrain brain;

    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineCamera sceneryCam;
    [SerializeField] private CinemachineCamera corkboardCam;
    [SerializeField] private CinemachineCamera phoneCam;
    [SerializeField] private CinemachineCamera typewriterCam;
    [SerializeField] private CinemachineCamera paperCam;

    private const int BlendStartTimeoutFrames = 3;

    public bool IsTransitioning { get; private set; }

    // Una secuencia (ej. la del teléfono) bloquea todo cambio de vista hasta que llame a EndSequence
    public bool IsSequenceRunning { get; private set; }

    private bool IsLocked => IsTransitioning || IsSequenceRunning;
    public GameView CurrentView { get; private set; } = GameView.Corkboard;

    public event Action<GameView> TransitionStarted;
    public event Action<GameView> StateChanged;

    public void BeginSequence() => IsSequenceRunning = true;

    // Llamar antes del Switch que cierra la secuencia, si no ese Switch también queda bloqueado
    public void EndSequence() => IsSequenceRunning = false;

    public void ToggleView()
    {
        if (CurrentView == GameView.Corkboard) SwitchToTypewriter();
        else if (CurrentView != GameView.Corkboard && CurrentView != GameView.Paper) SwitchToCorkboard();
    }

    public void SwitchToScenery()
    {
        if (IsLocked || CurrentView != GameView.Corkboard) return;

        CurrentView = GameView.Scenery;

        sceneryCam.Priority = 10;
        corkboardCam.Priority = 5;
        phoneCam.Priority = 5;
        typewriterCam.Priority = 5;
        paperCam.Priority = 5;

        //EnvironmentLightingManager.Instance?.SetLightingForPhone();
        StartCoroutine(TransitionRoutine());
    }

    public void SwitchToPhone()
    {
        if (IsLocked || CurrentView != GameView.Corkboard) return;

        CurrentView = GameView.Phone;

        sceneryCam.Priority = 5;
        corkboardCam.Priority = 5;
        phoneCam.Priority = 10;
        typewriterCam.Priority = 5;
        paperCam.Priority = 5;

        //EnvironmentLightingManager.Instance?.SetLightingForPhone();
        StartCoroutine(TransitionRoutine());
    }

    public void SwitchToTypewriter()
    {
        // Solo se llega a la Typewriter desde el tablero o cerrando el papel
        if (IsLocked || (CurrentView != GameView.Corkboard && CurrentView != GameView.Paper)) return;

        CurrentView = GameView.Typewriter;

        sceneryCam.Priority = 5;
        corkboardCam.Priority = 5;
        phoneCam.Priority = 5;
        typewriterCam.Priority = 10;
        paperCam.Priority = 5;

        EnvironmentLightingManager.Instance?.SetLightingForTypewriter();
        StartCoroutine(TransitionRoutine());
    }

    public void SwitchToPaper()
    {
        if (IsLocked || CurrentView == GameView.Paper) return;

        CurrentView = GameView.Paper;

        sceneryCam.Priority = 5;
        corkboardCam.Priority = 5;
        phoneCam.Priority = 5;
        typewriterCam.Priority = 5;
        paperCam.Priority = 10;

        EnvironmentLightingManager.Instance?.SetLightingForPaper();
        StartCoroutine(TransitionRoutine());
    }

    public void SwitchToCorkboard()
    {
        // Desde Paper solo se vuelve a Typewriter (lo cierra el papel); no se salta directo al tablero
        if (IsLocked || CurrentView == GameView.Corkboard || CurrentView == GameView.Paper) return;

        CurrentView = GameView.Corkboard;

        sceneryCam.Priority = 5;
        typewriterCam.Priority = 5;
        phoneCam.Priority = 5;
        corkboardCam.Priority = 10;
        paperCam.Priority = 5;

        EnvironmentLightingManager.Instance?.SetLightingForCorkboard();
        StartCoroutine(TransitionRoutine());
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

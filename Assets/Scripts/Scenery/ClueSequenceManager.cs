/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: hace brillar cada tanto, en secuencia, los inspectables que tienen pistas por archivar.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueSequenceManager : MonoBehaviour
{
    [SerializeField] private SparkleSettings settings;

    [Header("Inspectables")]
    [SerializeField] private SceneryInspectable[] inspectables;

    private readonly List<SceneryInspectable> _sequence = new List<SceneryInspectable>();
    private Coroutine _loop;

    private void Start()
    {
        GameViewManager view = GameViewManager.Instance;
        view.TransitionStarted += OnTransitionStarted;
        view.StateChanged += OnStateChanged;
        InspectionWindow.Instance.Opened += StopAllSparkles;

        if (view.CurrentView == GameView.Scenery && !view.IsTransitioning) StartLoop();
    }

    private void OnStateChanged(GameView view)
    {
        if (view == GameView.Scenery) StartLoop();
    }

    private void OnTransitionStarted(GameView view) => StopLoop();

    private void StartLoop()
    {
        StopLoop();
        _loop = StartCoroutine(SparkleLoop());
    }

    private void StopLoop()
    {
        if (_loop != null)
        {
            StopCoroutine(_loop);
            _loop = null;
        }

        StopAllSparkles();
    }

    private IEnumerator SparkleLoop()
    {
        yield return new WaitWhile(() => GameViewManager.Instance.IsSequenceRunning);
        yield return WaitWithWindowClosed(settings.FirstSequenceDelay);

        while (true)
        {
            yield return PlaySequence();
            yield return WaitWithWindowClosed(settings.RandomInterval);
        }
    }

    private IEnumerator PlaySequence()
    {
        BuildSequence();

        foreach (SceneryInspectable inspectable in _sequence)
        {
            yield return new WaitUntil(() => !InspectionWindow.Instance.IsOpen);

            if (inspectable.isActiveAndEnabled && !inspectable.IsCollected)
                inspectable.PlaySparkles(settings);

            if (settings.SequenceOffset > 0f) yield return new WaitForSeconds(settings.SequenceOffset);
        }

        yield return new WaitUntil(IsSequenceFinished);
    }

    private void BuildSequence()
    {
        _sequence.Clear();

        foreach (SceneryInspectable inspectable in inspectables)
        {
            if (inspectable != null && inspectable.isActiveAndEnabled && !inspectable.IsCollected)
                _sequence.Add(inspectable);
        }

        Shuffle(_sequence);
        _sequence.Sort((a, b) => b.IsNewlyUnlocked.CompareTo(a.IsNewlyUnlocked));
    }

    private bool IsSequenceFinished()
    {
        foreach (SceneryInspectable inspectable in _sequence)
        {
            if (inspectable != null && inspectable.IsSparkling) return false;
        }
        return true;
    }

    private static IEnumerator WaitWithWindowClosed(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (!InspectionWindow.Instance.IsOpen) elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void StopAllSparkles()
    {
        foreach (SceneryInspectable inspectable in inspectables)
        {
            if (inspectable != null) inspectable.StopSparkles();
        }
    }

    private static void Shuffle(List<SceneryInspectable> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private void OnDestroy()
    {
        if (GameViewManager.Instance != null)
        {
            GameViewManager.Instance.TransitionStarted -= OnTransitionStarted;
            GameViewManager.Instance.StateChanged -= OnStateChanged;
        }

        if (InspectionWindow.Instance != null) InspectionWindow.Instance.Opened -= StopAllSparkles;
    }
}

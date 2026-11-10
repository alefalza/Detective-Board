using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueSequenceManager : MonoBehaviour
{
    [SerializeField] private SparkleSettings settings;

    [Header("Lista de Pistas")]
    [Tooltip("Inspectables con brillos. Cada uno debe tener sus PointSparkle como hijos.")]
    [SerializeField] private SceneryInspectable[] inspectables;

    private readonly List<SceneryInspectable> _candidates = new List<SceneryInspectable>();
    private SceneryInspectable _lastInspectable;

    private void Start()
    {
        InspectionWindow.Instance.Opened += OnWindowOpened;
        StartCoroutine(SparkleLoop());
    }

    private IEnumerator SparkleLoop()
    {
        while (true)
        {
            yield return WaitInterval(settings.RandomInterval);

            if (!CanSparkle()) continue;

            SceneryInspectable inspectable = PickInspectable();
            if (inspectable == null) continue;

            _lastInspectable = inspectable;
            inspectable.PlaySparkles(settings);

            // El intervalo recién empieza a contar cuando termina el destello
            yield return new WaitWhile(() => inspectable.IsSparkling);
        }
    }

    // Solo cuenta con la cámara asentada en Scenery; fuera de esa vista queda en pausa
    private static IEnumerator WaitInterval(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (IsInScenery()) elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void OnWindowOpened()
    {
        if (_lastInspectable != null) _lastInspectable.StopSparkles();
    }

    private static bool IsInScenery()
    {
        GameViewManager view = GameViewManager.Instance;
        return view.CurrentView == GameView.Scenery && !view.IsTransitioning;
    }

    private static bool CanSparkle() => IsInScenery() && !InspectionWindow.Instance.IsOpen;

    // Al azar entre los inspectables de la slide visible, evitando repetir si hay otra opción
    private SceneryInspectable PickInspectable()
    {
        _candidates.Clear();

        foreach (SceneryInspectable inspectable in inspectables)
        {
            if (inspectable != null && inspectable.isActiveAndEnabled) _candidates.Add(inspectable);
        }

        if (_candidates.Count > 1) _candidates.Remove(_lastInspectable);

        return _candidates.Count > 0 ? _candidates[Random.Range(0, _candidates.Count)] : null;
    }

    private void OnDestroy()
    {
        if (InspectionWindow.Instance != null) InspectionWindow.Instance.Opened -= OnWindowOpened;
    }
}

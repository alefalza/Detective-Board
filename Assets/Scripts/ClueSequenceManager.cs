using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueSequenceManager : MonoBehaviour
{
    [SerializeField] private SparkleSettings settings;

    [Header("Lista de Pistas")]
    [Tooltip("Inspectables con brillos, incluidos los que arrancan bloqueados.")]
    [SerializeField] private SceneryInspectable[] inspectables;

    private readonly List<SceneryInspectable> _candidates = new List<SceneryInspectable>();
    private readonly HashSet<SceneryInspectable> _shownThisCycle = new HashSet<SceneryInspectable>();
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
            _shownThisCycle.Add(inspectable);
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

    // Cada ciclo muestra todos los inspectables visibles una vez antes de repetir.
    // Dentro del ciclo: primero los recién desbloqueados, después los que no se archivaron y por último el resto
    private SceneryInspectable PickInspectable()
    {
        CollectUnshown();

        if (_candidates.Count == 0)
        {
            // Ciclo completo: empieza otro, sin repetir el último si hay otra opción
            _shownThisCycle.Clear();
            CollectUnshown();
            if (_candidates.Count > 1) _candidates.Remove(_lastInspectable);
        }

        if (_candidates.Count == 0) return null;

        List<SceneryInspectable> tier = FilterTier(i => i.IsNewlyUnlocked);
        if (tier.Count == 0) tier = FilterTier(i => !i.IsCollected);
        if (tier.Count == 0) tier = _candidates;

        return tier[Random.Range(0, tier.Count)];
    }

    private void CollectUnshown()
    {
        _candidates.Clear();

        foreach (SceneryInspectable inspectable in inspectables)
        {
            if (inspectable != null && inspectable.isActiveAndEnabled && !_shownThisCycle.Contains(inspectable))
                _candidates.Add(inspectable);
        }
    }

    private List<SceneryInspectable> FilterTier(System.Predicate<SceneryInspectable> match)
    {
        return _candidates.FindAll(match);
    }

    private void OnDestroy()
    {
        if (InspectionWindow.Instance != null) InspectionWindow.Instance.Opened -= OnWindowOpened;
    }
}

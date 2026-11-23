using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instancia en el tablero las pistas archivadas al volver al corcho, cada una en el punto de aparición
/// más alejado de las pistas que ya están, para que no queden amontonadas.
/// </summary>
public class EvidenceSpawner : MonoBehaviour
{
    [SerializeField] private DeductionSettings settings;
    [SerializeField] private EvidenceItem evidenceItemPrefab;
    [SerializeField] private Transform itemsContainer;
    [Tooltip("Puntos del tablero donde pueden aparecer pistas nuevas. Conviene repartirlos por zonas libres.")]
    [SerializeField] private Transform[] spawnPoints;

    private readonly HashSet<EvidenceData> _queuedOnce = new HashSet<EvidenceData>();
    private readonly List<EvidenceData> _pending = new List<EvidenceData>();
    private readonly List<Vector3> _occupied = new List<Vector3>();
    private readonly List<Transform> _shuffledPoints = new List<Transform>();

    private void Start() => GameViewManager.Instance.StateChanged += OnViewChanged;

    // La pista se instancia al volver al tablero. false si ya se había entregado.
    public bool TryQueue(EvidenceData data)
    {
        if (!_queuedOnce.Add(data)) return false;

        _pending.Add(data);
        return true;
    }

    public bool WasQueued(EvidenceData data) => _queuedOnce.Contains(data);

    private void OnViewChanged(GameView view)
    {
        if (view != GameView.Corkboard || _pending.Count == 0) return;

        CollectOccupiedPositions();

        foreach (EvidenceData data in _pending)
        {
            Vector3 position = PickFreestPoint();
            _occupied.Add(position); // las siguientes del mismo lote la evitan
            Spawn(data, position);
        }

        _pending.Clear();
    }

    private void CollectOccupiedPositions()
    {
        _occupied.Clear();
        foreach (EvidenceItem item in FindObjectsByType<EvidenceItem>(FindObjectsSortMode.None))
            _occupied.Add(item.transform.position);
    }

    // El punto cuya pista más cercana está más lejos. Se mezclan antes para que los empates se resuelvan al azar
    private Vector3 PickFreestPoint()
    {
        _shuffledPoints.Clear();
        foreach (Transform point in spawnPoints)
        {
            if (point != null) _shuffledPoints.Add(point);
        }

        if (_shuffledPoints.Count == 0) return itemsContainer.position;

        Shuffle(_shuffledPoints);

        Vector3 best = _shuffledPoints[0].position;
        float bestDistance = float.MinValue;

        foreach (Transform point in _shuffledPoints)
        {
            float distance = DistanceToNearestOccupied(point.position);
            if (distance <= bestDistance) continue;

            bestDistance = distance;
            best = point.position;
        }

        return best;
    }

    private float DistanceToNearestOccupied(Vector3 position)
    {
        float nearest = float.MaxValue;
        foreach (Vector3 occupied in _occupied)
            nearest = Mathf.Min(nearest, Vector2.Distance(position, occupied));

        return nearest;
    }

    private void Spawn(EvidenceData data, Vector3 position)
    {
        EvidenceItem item = Instantiate(evidenceItemPrefab, itemsContainer);
        item.transform.SetPositionAndRotation(position, Quaternion.identity);
        item.SetupFromData(data);
        item.BringToFront();
        item.ResolveOverlap(); // respaldo, por si el tablero ya está lleno

        item.PlaySpawnFeedback(settings.SpawnPunchAmount, settings.SpawnPunchTime);
    }

    private static void Shuffle(List<Transform> list)
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
            GameViewManager.Instance.StateChanged -= OnViewChanged;
    }
}

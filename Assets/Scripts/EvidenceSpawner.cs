using System.Collections.Generic;
using UnityEngine;

public class EvidenceSpawner : MonoBehaviour
{
    [SerializeField] private DeductionSettings settings;
    [SerializeField] private EvidenceItem evidenceItemPrefab;
    [SerializeField] private Transform itemsContainer;
    [Tooltip("Dónde aparece la pista si no queda ninguna DropZone libre.")]
    [SerializeField] private Transform fallbackSpawnPoint;

    private readonly HashSet<EvidenceData> _queuedOnce = new HashSet<EvidenceData>();
    private readonly List<EvidenceData> _pending = new List<EvidenceData>();

    private void Start() => GameViewManager.Instance.StateChanged += OnViewChanged;

    // La pista se instancia al volver al tablero. false si ya se había entregado.
    public bool TryQueue(EvidenceData data)
    {
        if (!_queuedOnce.Add(data)) return false;

        _pending.Add(data);
        return true;
    }

    private void OnViewChanged(GameView view)
    {
        if (view != GameView.Corkboard) return;

        foreach (EvidenceData data in _pending) Spawn(data);
        _pending.Clear();
    }

    private void Spawn(EvidenceData data)
    {
        DropZone freeZone = FindFreeDropZone();
        Vector3 position = freeZone != null ? freeZone.transform.position : fallbackSpawnPoint.position;

        EvidenceItem item = Instantiate(evidenceItemPrefab, itemsContainer);
        item.transform.SetPositionAndRotation(position, Quaternion.identity);
        item.SetupFromData(data);
        item.BringToFront();

        if (freeZone != null) item.SnapToZone(freeZone);
        else item.ResolveOverlap();

        item.PlaySpawnFeedback(settings.SpawnPunchAmount, settings.SpawnPunchTime);
    }

    private static DropZone FindFreeDropZone()
    {
        foreach (DropZone zone in FindObjectsByType<DropZone>(FindObjectsSortMode.InstanceID))
        {
            if (!zone.IsOccupied) return zone;
        }
        return null;
    }

    private void OnDestroy()
    {
        if (GameViewManager.Instance != null)
            GameViewManager.Instance.StateChanged -= OnViewChanged;
    }
}

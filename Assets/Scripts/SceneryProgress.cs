using System;
using UnityEngine;

/// <summary>
/// Estado de progreso del Scenery: qué inspectables están desbloqueados y qué pistas ya entregaron.
/// </summary>
public class SceneryProgress : Singleton<SceneryProgress>
{
    [SerializeField] private SceneryViewerSettings settings;
    [SerializeField] private EvidenceSpawner evidenceSpawner;

    private SceneryInspectable[] _inspectables;

    public event Action InspectableUnlocked;

    protected override void Awake()
    {
        base.Awake();

        _inspectables = FindObjectsByType<SceneryInspectable>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (SceneryInspectable inspectable in _inspectables)
            inspectable.gameObject.SetActive(inspectable.StartsUnlocked);
    }

    private void Start() => GameViewManager.Instance.StateChanged += OnViewChanged;

    private void OnViewChanged(GameView view)
    {
        if (view != GameView.Scenery) return;

        foreach (SceneryInspectable inspectable in _inspectables)
            inspectable.TryReveal();
    }

    public void Unlock(InspectableData data)
    {
        bool found = false;
        bool unlockedAny = false;

        foreach (SceneryInspectable inspectable in _inspectables)
        {
            if (inspectable.Data != data) continue;

            found = true;
            if (inspectable.gameObject.activeSelf) continue;

            inspectable.Unlock(settings.RevealDuration);
            unlockedAny = true;
        }

        if (!found) Debug.LogWarning($"[SceneryProgress] Ningún inspectable de la escena usa '{data.name}'.", data);
        if (unlockedAny) InspectableUnlocked?.Invoke();
    }

    // true solo la primera vez que el inspectable entrega su pista
    public bool CollectEvidence(InspectableData data)
    {
        return data.Evidence != null && evidenceSpawner.TryQueue(data.Evidence);
    }

    private void OnDestroy()
    {
        if (GameViewManager.Instance != null)
            GameViewManager.Instance.StateChanged -= OnViewChanged;
    }
}

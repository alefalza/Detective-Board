/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: desbloquea inspectables por deducción, los revela al llegar a la vista y archiva sus pistas.
 */

using System;
using UnityEngine;

public class SceneryProgress : Singleton<SceneryProgress>
{
    [SerializeField] private SceneryViewerSettings settings;
    [SerializeField] private EvidenceSpawner evidenceSpawner;
    [SerializeField] private AudioSource revealAudioSource;

    private SceneryInspectable[] _inspectables;

    public event Action InspectableUnlocked;
    public event Action EvidenceArchived;

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

        bool revealedAny = false;
        foreach (SceneryInspectable inspectable in _inspectables)
        {
            if (inspectable.TryReveal()) revealedAny = true;
        }

        if (revealedAny && settings.RevealClip != null)
            revealAudioSource.PlayOneShot(settings.RevealClip);
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

    public bool CanArchive(InspectableData data)
    {
        foreach (EvidenceData evidence in data.Evidences)
        {
            if (!evidenceSpawner.WasQueued(evidence)) return true;
        }
        return false;
    }

    public void Archive(InspectableData data)
    {
        if (!CanArchive(data)) return;

        foreach (EvidenceData evidence in data.Evidences)
            evidenceSpawner.TryQueue(evidence);

        EvidenceArchived?.Invoke();
    }

    private void OnDestroy()
    {
        if (GameViewManager.Instance != null)
            GameViewManager.Instance.StateChanged -= OnViewChanged;
    }
}

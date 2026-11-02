using System.Collections.Generic;
using UnityEngine;

public class DeductionResolver : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private DeductionSettings settings;

    [Header("Base de Datos de Deducciones")]
    [SerializeField] private List<DeductionRule> deductionRules = new List<DeductionRule>();

    [Header("Módulos")]
    [SerializeField] private ThreadController threads;

    [Header("Prefabs")]
    [SerializeField] private EvidenceItem evidenceItemPrefab;
    [SerializeField] private Transform itemsContainer;

    private readonly HashSet<EvidenceData> unlockedEvidence = new HashSet<EvidenceData>();
    private readonly HashSet<DeductionRule> resolvedRules = new HashSet<DeductionRule>();

    public int GroupSize => settings.GroupSize;

    // ── Evaluación (no modifica estado) ──────────────────────────────

    public DeductionResult Evaluate(List<EvidenceItem> group)
    {
        var datas = new List<EvidenceData>(group.Count);
        foreach (EvidenceItem item in group) datas.Add(item.Data);

        DeductionRule bestRule = null;
        int bestMatches = 0;

        foreach (DeductionRule rule in deductionRules)
        {
            if (rule == null) continue;

            int matches = rule.CountMatches(datas);
            if (matches <= bestMatches) continue;

            bestRule = rule;
            bestMatches = matches;
        }

        if (bestRule != null && bestRule.IsComplete(bestMatches))
            return BuildCorrect(bestRule, group);

        if (bestRule != null && bestMatches >= 2)
        {
            if (bestRule.TryGetHint(datas, out string hint))
                return new DeductionResult(DeductionOutcome.Close, StripTags(hint), bestRule, group);

            Debug.LogWarning($"[DeductionResolver] '{bestRule.name}' no tiene PartialHint para este par. Se trata como 'lejos'.");
        }

        return new DeductionResult(DeductionOutcome.Far, settings.FarText);
    }

    // Todas las keywords de las frases de resultado (sin repetir)
    public List<string> GetAllKeywords()
    {
        var all = new List<string>();
        foreach (DeductionRule rule in deductionRules)
        {
            if (rule == null) continue;

            foreach (string keyword in KeywordTagParser.Extract(rule.ResultDescription ?? string.Empty, out _))
            {
                if (!all.Contains(keyword)) all.Add(keyword);
            }
        }
        return all;
    }

    private DeductionResult BuildCorrect(DeductionRule rule, List<EvidenceItem> group)
    {
        bool isNewSolve = !resolvedRules.Contains(rule);

        // Si ya estaba resuelta se muestra la frase sin keywords (no se vuelven a emitir)
        string text = isNewSolve ? rule.ResultDescription : StripTags(rule.ResultDescription);

        return new DeductionResult(DeductionOutcome.Correct, text, rule, group, isNewSolve);
    }

    private static string StripTags(string tagged)
    {
        KeywordTagParser.Extract(tagged ?? string.Empty, out string clean);
        return clean;
    }

    // ── Efectos (se aplican al volver al tablero) ────────────────────

    public void Apply(DeductionResult result)
    {
        if (!result.IsNewSolve) return;

        DeductionRule rule = result.Rule;
        resolvedRules.Add(rule);
        threads.LockConnectionsBetween(result.Participants);

        foreach (EvidenceItem item in result.Participants)
            item.MarkValidated(settings.ValidatedPunchAmount, settings.ValidatedPunchTime, settings.ValidatedColor);

        if (rule.InvalidatedEvidence != null)
        {
            foreach (EvidenceItem item in FindObjectsByType<EvidenceItem>(FindObjectsSortMode.None))
            {
                if (item.Data == rule.InvalidatedEvidence) item.InvalidateEvidence();
            }
        }

        if (rule.NewUnlockedEvidence != null)
            SpawnEvidenceBetween(rule.NewUnlockedEvidence, result.Participants);
    }

    private void SpawnEvidenceBetween(EvidenceData newData, IReadOnlyList<EvidenceItem> participants)
    {
        if (evidenceItemPrefab == null)
        {
            Debug.LogWarning("DeductionResolver: falta asignar evidenceItemPrefab.");
            return;
        }

        if (!unlockedEvidence.Add(newData)) return;

        DropZone freeZone = FindFreeDropZone();
        Vector3 spawnPosition = freeZone != null
            ? freeZone.transform.position
            : GetCenter(participants);

        EvidenceItem newItem = Instantiate(evidenceItemPrefab, itemsContainer);
        newItem.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
        newItem.SetupFromData(newData);
        newItem.BringToFront();

        if (freeZone != null) newItem.SnapToZone(freeZone);
    }

    private Vector3 GetCenter(IReadOnlyList<EvidenceItem> items)
    {
        Vector3 sum = Vector3.zero;
        foreach (EvidenceItem item in items) sum += item.transform.position;
        return sum / items.Count;
    }

    private DropZone FindFreeDropZone()
    {
        foreach (DropZone zone in FindObjectsByType<DropZone>(FindObjectsSortMode.InstanceID))
        {
            if (!zone.IsOccupied) return zone;
        }
        return null;
    }
}

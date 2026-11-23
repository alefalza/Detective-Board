/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: regla de deducción con su trío, frases de “estás cerca”, texto con keywords, invalidación y desbloqueos.
 */

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PartialHint
{
    [SerializeField] private EvidenceData evidenceA;
    [SerializeField] private EvidenceData evidenceB;
    [TextArea(2, 4)] [SerializeField] private string text;

    public string Text => text;

    public bool Covers(List<EvidenceData> group)
    {
        return evidenceA != null && evidenceB != null
            && group.Contains(evidenceA) && group.Contains(evidenceB);
    }
}

[CreateAssetMenu(fileName = "NewDeductionRule", menuName = "Detective/Deduction Rule")]
public class DeductionRule : ScriptableObject
{
    [Header("Condition")]
    [SerializeField] private List<EvidenceData> requiredEvidences = new List<EvidenceData>();

    [Header("Partial Hints")]
    [SerializeField] private List<PartialHint> partialHints = new List<PartialHint>();

    [Header("Result")]
    [TextArea(2, 4)] [SerializeField] private string resultDescription;
    [SerializeField] private EvidenceData invalidatedEvidence;
    [SerializeField] private List<InspectableData> newUnlockedInspectables = new List<InspectableData>();

    public string ResultDescription => resultDescription;
    public EvidenceData InvalidatedEvidence => invalidatedEvidence;
    public IReadOnlyList<InspectableData> NewUnlockedInspectables => newUnlockedInspectables;

    public int CountMatches(List<EvidenceData> group)
    {
        int count = 0;
        foreach (EvidenceData required in requiredEvidences)
        {
            if (group.Contains(required)) count++;
        }
        return count;
    }

    public bool IsComplete(int matches) => matches == requiredEvidences.Count;

    public bool TryGetHint(List<EvidenceData> group, out string text)
    {
        foreach (PartialHint hint in partialHints)
        {
            if (!hint.Covers(group)) continue;

            text = hint.Text;
            return true;
        }

        text = null;
        return false;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Frase de "estás cerca" para un par de evidencias de la regla.</summary>
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
    [Header("Condición (las 3 evidencias del trío)")]
    [SerializeField] private List<EvidenceData> requiredEvidences = new List<EvidenceData>();

    [Header("Estás cerca (una frase por cada par)")]
    [SerializeField] private List<PartialHint> partialHints = new List<PartialHint>();

    [Header("Resultado (con <keyword>…</keyword> para las palabras clave)")]
    [TextArea(2, 4)] [SerializeField] private string resultDescription;
    [SerializeField] private EvidenceData invalidatedEvidence;
    [Tooltip("Inspectables del Scenery que se habilitan al resolver la regla.")]
    [SerializeField] private List<InspectableData> newUnlockedInspectables = new List<InspectableData>();

    public string ResultDescription => resultDescription;
    public EvidenceData InvalidatedEvidence => invalidatedEvidence;
    public IReadOnlyList<InspectableData> NewUnlockedInspectables => newUnlockedInspectables;

    // Cuántas de las evidencias requeridas están presentes en el grupo
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

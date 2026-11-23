/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: título, descripción, imagen y pistas que entrega un inspectable.
 */

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInspectable", menuName = "Detective/Inspectable")]
public class InspectableData : ScriptableObject
{
    [SerializeField] private string title;
    [TextArea(3, 8)] [SerializeField] private string description;
    [SerializeField] private Sprite image;

    [Header("Evidences")]
    [SerializeField] private EvidenceData evidence;
    [SerializeField] private List<EvidenceData> additionalEvidences = new List<EvidenceData>();

    private readonly List<EvidenceData> _evidences = new List<EvidenceData>();

    public string Title => title;
    public string Description => description;
    public Sprite Image => image;

    public IReadOnlyList<EvidenceData> Evidences
    {
        get
        {
            _evidences.Clear();
            AddEvidence(evidence);
            foreach (EvidenceData extra in additionalEvidences) AddEvidence(extra);
            return _evidences;
        }
    }

    public bool HasEvidence => Evidences.Count > 0;

    private void AddEvidence(EvidenceData data)
    {
        if (data != null && !_evidences.Contains(data)) _evidences.Add(data);
    }
}

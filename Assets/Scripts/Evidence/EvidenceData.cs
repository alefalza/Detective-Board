using UnityEngine;

public enum EvidenceType
{
    OfficialReport,  // Acta policial o diario oficial (puede ser falso/desmentido)
    Testimony,       // Audio o nota de testigo
    PhysicalProof,   // Objeto físico (reloj, boleto)
    Deduction        // Ficha que surge al unir dos pistas
}

public abstract class EvidenceData : ScriptableObject
{
    [Header("Identificación")]
    public string id; // Ej: "EVI_ACTA_HORA", "EVI_RELOJ_ROTO"
    public string title;
    public EvidenceType type;
    public Color backgroundColor = Color.white;

    public abstract void ApplyTo(EvidenceItemView itemView);
}

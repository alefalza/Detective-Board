using UnityEngine;

[CreateAssetMenu(fileName = "NewNote", menuName = "Detective/Evidence/Note")]
public class NoteEvidenceData : EvidenceData
{
    [Header("Visuales")]
    public Sprite backgroundImage;

    public override void ApplyTo(EvidenceItemView item)
    {
        item.SetTitle(title);
        item.SetBackgroundColor(backgroundColor);
    }
}

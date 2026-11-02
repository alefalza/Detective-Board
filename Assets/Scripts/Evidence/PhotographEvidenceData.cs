using UnityEngine;

[CreateAssetMenu(fileName = "NewPhotograph", menuName = "Detective/Evidence/Photograph")]
public class PhotographEvidenceData : EvidenceData
{
    [Header("Visuales")]
    public Sprite backgroundImage;

    public override void ApplyTo(EvidenceItemView item)
    {
        item.SetTitle(title);
        item.SetBackgroundColor(Color.white);
    }
}

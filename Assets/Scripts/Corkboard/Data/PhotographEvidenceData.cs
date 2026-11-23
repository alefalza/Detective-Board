/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: datos de una pista de tipo fotografía.
 */

using UnityEngine;

[CreateAssetMenu(fileName = "NewPhotograph", menuName = "Detective/Evidence/Photograph")]
public class PhotographEvidenceData : EvidenceData
{
    [Header("Visuals")]
    public Sprite backgroundImage;

    public override void ApplyTo(EvidenceItemView item)
    {
        item.SetTitle(title);
        item.SetBackgroundColor(Color.white);
    }
}

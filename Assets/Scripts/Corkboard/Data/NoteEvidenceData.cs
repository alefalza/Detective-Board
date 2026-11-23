/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: datos de una pista de tipo nota.
 */

using UnityEngine;

[CreateAssetMenu(fileName = "NewNote", menuName = "Detective/Evidence/Note")]
public class NoteEvidenceData : EvidenceData
{
    [Header("Visuals")]
    public Sprite backgroundImage;

    public override void ApplyTo(EvidenceItemView item)
    {
        item.SetTitle(title);
        item.SetBackgroundColor(backgroundColor);
    }
}

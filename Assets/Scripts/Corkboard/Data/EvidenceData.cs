/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: base abstracta de los datos de una pista.
 */

using UnityEngine;

public abstract class EvidenceData : ScriptableObject
{
    public string id;
    public string title;
    public Color backgroundColor = Color.white;

    public abstract void ApplyTo(EvidenceItemView itemView);
}

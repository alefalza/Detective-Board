/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: nombre, epílogo y voz en off de un tono.
 */

using UnityEngine;

[CreateAssetMenu(fileName = "NewTone", menuName = "Detective/Article Tone")]
public class ToneData : ScriptableObject
{
    [SerializeField] private string displayName;

    [Header("Epílogo")]
    [TextArea(4, 10)] [SerializeField] private string epilogueText;
    [SerializeField] private AudioClip epilogueVoice;

    public string DisplayName => displayName;
    public string EpilogueText => epilogueText;
    public AudioClip EpilogueVoice => epilogueVoice;
}

using UnityEngine;

[CreateAssetMenu(fileName = "NewTone", menuName = "Detective/Article Tone")]
public class ToneData : ScriptableObject
{
    [SerializeField] private string displayName;

    [Header("Epílogo")]
    [TextArea(4, 10)] [SerializeField] private string epilogueText;
    [Tooltip("Opcional. Voz en off que acompaña al epílogo.")]
    [SerializeField] private AudioClip epilogueVoice;

    public string DisplayName => displayName;
    public string EpilogueText => epilogueText;
    public AudioClip EpilogueVoice => epilogueVoice;
}

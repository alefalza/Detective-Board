using UnityEngine;

[CreateAssetMenu(fileName = "PhoneSequenceSettings", menuName = "Detective/Phone Sequence Settings")]
public class PhoneSequenceSettings : ScriptableObject
{
    [Header("Descolgar y colgar")]
    [SerializeField] private AudioClip pickUpClip;
    [SerializeField] private AudioClip hangUpClip;

    [Header("Discado")]
    [SerializeField] private AudioClip dialClip;
    [Min(0f)] [SerializeField] private float dialDuration = 2.5f;

    [Header("Tono")]
    [SerializeField] private AudioClip toneClip;
    [Min(0f)] [SerializeField] private float toneDuration = 2f;

    [Header("Escritura en la libreta")]
    [Tooltip("Suena en loop mientras se escribe el texto.")]
    [SerializeField] private AudioClip mumblingClip;

    [Header("Cierre")]
    [Tooltip("Segundos entre que se cuelga el teléfono y la transición al tablero.")]
    [Min(0f)] [SerializeField] private float hangUpDelay = 0.5f;

    public AudioClip PickUpClip => pickUpClip;
    public float PickUpDuration => pickUpClip != null ? pickUpClip.length : 0f;
    public AudioClip HangUpClip => hangUpClip;
    public AudioClip DialClip => dialClip;
    public float DialDuration => dialDuration;
    public AudioClip ToneClip => toneClip;
    public float ToneDuration => toneDuration;
    public AudioClip MumblingClip => mumblingClip;
    public float HangUpDelay => hangUpDelay;
}

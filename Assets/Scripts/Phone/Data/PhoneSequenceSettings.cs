/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: clips y tiempos de la secuencia del teléfono.
 */

using UnityEngine;

[CreateAssetMenu(fileName = "PhoneSequenceSettings", menuName = "Detective/Phone Sequence Settings")]
public class PhoneSequenceSettings : ScriptableObject
{
    [Header("Pickup/Hangup")]
    [SerializeField] private AudioClip pickUpClip;
    [SerializeField] private AudioClip hangUpClip;

    [Header("Dial")]
    [SerializeField] private AudioClip dialClip;
    [Min(0f)] [SerializeField] private float dialDuration = 2.5f;

    [Header("Tone")]
    [SerializeField] private AudioClip toneClip;
    [Min(0f)] [SerializeField] private float toneDuration = 2f;

    [Header("Answer")]
    [Tooltip("Suena apenas termina el tono.")]
    [SerializeField] private AudioClip answerClip;
    [Min(0f)] [SerializeField] private float answerDelay = 1f;

    [Header("Handwriting")]
    [Tooltip("Suena en loop mientras se escribe el texto.")]
    [SerializeField] private AudioClip mumblingClip;

    [Header("Close")]
    [Min(0f)] [SerializeField] private float hangUpDelay = 0.5f;

    public AudioClip PickUpClip => pickUpClip;
    public float PickUpDuration => pickUpClip != null ? pickUpClip.length : 0f;
    public AudioClip HangUpClip => hangUpClip;
    public AudioClip DialClip => dialClip;
    public float DialDuration => dialDuration;
    public AudioClip ToneClip => toneClip;
    public float ToneDuration => toneDuration;
    public AudioClip AnswerClip => answerClip;
    public float AnswerDuration => answerClip != null ? answerClip.length : 0f;
    public float AnswerDelay => answerDelay;
    public AudioClip MumblingClip => mumblingClip;
    public float HangUpDelay => hangUpDelay;
}

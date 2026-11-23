/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: tiempos y animaciones del cierre del artículo, del epílogo y de la vuelta al menú.
 */

using UnityEngine;

[CreateAssetMenu(fileName = "ArticleFinaleSettings", menuName = "Detective/Article Finale Settings")]
public class ArticleFinaleSettings : ScriptableObject
{
    [Header("Clean Copy")]
    [Min(0f)] [SerializeField] private float cleanCopyDelay = 0.8f;

    [Header("Lift & Slam")]
    [Min(1f)] [SerializeField] private float liftScale = 1.12f;
    [Min(0.01f)] [SerializeField] private float liftDuration = 0.8f;
    [SerializeField] private iTween.EaseType liftEase = iTween.EaseType.easeOutSine;
    [Min(0.01f)] [SerializeField] private float slamDuration = 0.12f;
    [SerializeField] private iTween.EaseType slamEase = iTween.EaseType.easeInQuart;

    [Header("Impact")]
    [SerializeField] private Vector3 impactPunchAmount = new Vector3(0.04f, 0.04f, 0f);
    [Min(0.01f)] [SerializeField] private float impactPunchTime = 0.3f;
    [SerializeField] private AudioClip impactClip;

    [Header("Tone Selector")]
    [Min(0.01f)] [SerializeField] private float selectorFadeDuration = 0.4f;

    [Header("Print")]
    [Min(0f)] [SerializeField] private float printDelay = 1.5f;

    [Header("Epilogue")]
    [Min(0.01f)] [SerializeField] private float endingFadeDuration = 2f;
    [Min(0f)] [SerializeField] private float epilogueTextDelay = 1f;
    [Min(0f)] [SerializeField] private float epilogueHoldDuration = 4f;
    [Min(0.01f)] [SerializeField] private float epilogueTextFadeDuration = 2f;
    [Min(0f)] [SerializeField] private float returnToMenuDelay = 1.5f;

    public float CleanCopyDelay => cleanCopyDelay;
    public float LiftScale => liftScale;
    public float LiftDuration => liftDuration;
    public iTween.EaseType LiftEase => liftEase;
    public float SlamDuration => slamDuration;
    public iTween.EaseType SlamEase => slamEase;
    public Vector3 ImpactPunchAmount => impactPunchAmount;
    public float ImpactPunchTime => impactPunchTime;
    public AudioClip ImpactClip => impactClip;
    public float SelectorFadeDuration => selectorFadeDuration;
    public float PrintDelay => printDelay;
    public float EndingFadeDuration => endingFadeDuration;
    public float EpilogueTextDelay => epilogueTextDelay;
    public float EpilogueHoldDuration => epilogueHoldDuration;
    public float EpilogueTextFadeDuration => epilogueTextFadeDuration;
    public float ReturnToMenuDelay => returnToMenuDelay;
}

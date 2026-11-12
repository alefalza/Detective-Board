using UnityEngine;

[CreateAssetMenu(fileName = "ArticleFinaleSettings", menuName = "Detective/Article Finale Settings")]
public class ArticleFinaleSettings : ScriptableObject
{
    [Header("Pasar en limpio")]
    [Tooltip("Segundos entre que se resuelve el artículo y se pasa en limpio.")]
    [Min(0f)] [SerializeField] private float cleanCopyDelay = 0.8f;

    [Header("Levantar y dejar caer")]
    [Tooltip("Escala máxima al levantarse.")]
    [Min(1f)] [SerializeField] private float liftScale = 1.12f;
    [Min(0.01f)] [SerializeField] private float liftDuration = 0.8f;
    [SerializeField] private iTween.EaseType liftEase = iTween.EaseType.easeOutSine;
    [Min(0.01f)] [SerializeField] private float slamDuration = 0.12f;
    [SerializeField] private iTween.EaseType slamEase = iTween.EaseType.easeInQuart;

    [Header("Impacto sobre la hoja")]
    [SerializeField] private Vector3 impactPunchAmount = new Vector3(0.04f, 0.04f, 0f);
    [Min(0.01f)] [SerializeField] private float impactPunchTime = 0.3f;
    [SerializeField] private AudioClip impactClip;

    [Header("Selector de tono")]
    [Min(0.01f)] [SerializeField] private float selectorFadeDuration = 0.4f;

    [Header("Imprenta")]
    [Tooltip("Segundos entre que se confirma el tono y empieza el fundido a negro.")]
    [Min(0f)] [SerializeField] private float printDelay = 1.5f;

    [Header("Epílogo")]
    [Min(0.01f)] [SerializeField] private float endingFadeDuration = 2f;
    [Tooltip("Segundos con la pantalla en negro antes de que empiece el texto.")]
    [Min(0f)] [SerializeField] private float epilogueTextDelay = 1f;
    [Tooltip("Segundos entre que termina de escribirse el epílogo y empieza a desvanecerse.")]
    [Min(0f)] [SerializeField] private float epilogueHoldDuration = 4f;
    [Min(0.01f)] [SerializeField] private float epilogueTextFadeDuration = 2f;

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
}

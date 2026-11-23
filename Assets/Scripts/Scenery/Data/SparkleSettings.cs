/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: tiempos de las secuencias y forma de los brillos.
 */

using UnityEngine;

[CreateAssetMenu(fileName = "SparkleSettings", menuName = "Detective/Sparkle Settings")]
public class SparkleSettings : ScriptableObject
{
    [Header("Sequence")]
    [Min(0f)] [SerializeField] private float firstSequenceDelay = 1.5f;
    [Min(0f)] [SerializeField] private float sequenceOffset = 0.25f;
    [SerializeField] private Vector2 intervalRange = new Vector2(8f, 20f);

    [Header("Burst")]
    [SerializeField] private Vector2 startDelayRange = new Vector2(0f, 0.3f);
    [SerializeField] private Vector2Int twinkleCountRange = new Vector2Int(1, 3);
    [SerializeField] private Vector2 twinkleGapRange = new Vector2(0.05f, 0.2f);

    [Header("Sparkle")]
    [SerializeField] private Vector2 durationRange = new Vector2(0.35f, 0.6f);
    [SerializeField] private Vector2 sizeRange = new Vector2(0.6f, 1.2f);
    [SerializeField] private float spin = 45f;
    [Min(0f)] [SerializeField] private float positionJitter = 12f;

    public float FirstSequenceDelay => firstSequenceDelay;
    public float SequenceOffset => sequenceOffset;
    public float RandomInterval =>Random.Range(intervalRange.x, intervalRange.y);
    public float RandomStartDelay => Random.Range(startDelayRange.x, startDelayRange.y);
    public int RandomTwinkleCount => Random.Range(twinkleCountRange.x, twinkleCountRange.y + 1);
    public float RandomTwinkleGap => Random.Range(twinkleGapRange.x, twinkleGapRange.y);
    public float RandomDuration => Random.Range(durationRange.x, durationRange.y);
    public float RandomSize => Random.Range(sizeRange.x, sizeRange.y);
    public float Spin => spin;
    public float PositionJitter => positionJitter;
}

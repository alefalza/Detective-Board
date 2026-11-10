using UnityEngine;

[CreateAssetMenu(fileName = "SparkleSettings", menuName = "Detective/Sparkle Settings")]
public class SparkleSettings : ScriptableObject
{
    [Header("Frecuencia")]
    [Tooltip("Segundos de silencio entre el final de un destello y el siguiente (mínimo y máximo). Es el valor que más define cuánto se regala la pista.")]
    [SerializeField] private Vector2 intervalRange = new Vector2(8f, 20f);

    [Header("Ráfaga")]
    [Tooltip("Demora de cada brillo al arrancar, para que los de un mismo inspectable no salgan todos a la vez.")]
    [SerializeField] private Vector2 startDelayRange = new Vector2(0f, 0.3f);
    [Tooltip("Cantidad de brillitos por destello (mínimo y máximo, ambos incluidos).")]
    [SerializeField] private Vector2Int twinkleCountRange = new Vector2Int(1, 3);
    [SerializeField] private Vector2 twinkleGapRange = new Vector2(0.05f, 0.2f);

    [Header("Cada brillito")]
    [SerializeField] private Vector2 durationRange = new Vector2(0.35f, 0.6f);
    [SerializeField] private Vector2 sizeRange = new Vector2(0.6f, 1.2f);
    [Tooltip("Grados que gira durante el brillito.")]
    [SerializeField] private float spin = 45f;
    [Tooltip("Desplazamiento máximo desde su posición original, en unidades del Canvas.")]
    [Min(0f)] [SerializeField] private float positionJitter = 12f;

    public float RandomInterval => Random.Range(intervalRange.x, intervalRange.y);
    public float RandomStartDelay => Random.Range(startDelayRange.x, startDelayRange.y);
    public int RandomTwinkleCount => Random.Range(twinkleCountRange.x, twinkleCountRange.y + 1);
    public float RandomTwinkleGap => Random.Range(twinkleGapRange.x, twinkleGapRange.y);
    public float RandomDuration => Random.Range(durationRange.x, durationRange.y);
    public float RandomSize => Random.Range(sizeRange.x, sizeRange.y);
    public float Spin => spin;
    public float PositionJitter => positionJitter;
}

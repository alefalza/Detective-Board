using UnityEngine;

[CreateAssetMenu(fileName = "DeductionSettings", menuName = "Detective/Deduction Settings")]
public class DeductionSettings : ScriptableObject
{
    [Header("Consulta")]
    [Min(2)] [SerializeField] private int groupSize = 3;

    [Header("Textos")]
    [TextArea(2, 4)] [SerializeField] private string farText = "Todavía estás lejos.";

    [Header("Feedback de validación (pistas del trío)")]
    [SerializeField] private Vector3 validatedPunchAmount = new Vector3(0.2f, 0.2f, 0f);
    [Min(0.05f)] [SerializeField] private float validatedPunchTime = 0.5f;
    [Tooltip("Color del fondo de las pistas una vez terminado el punch.")]
    [SerializeField] private Color validatedColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    [Header("Feedback de aparición (pistas nuevas)")]
    [SerializeField] private Vector3 spawnPunchAmount = new Vector3(0.3f, 0.3f, 0f);
    [Min(0.05f)] [SerializeField] private float spawnPunchTime = 0.5f;

    public int GroupSize => groupSize;
    public string FarText => farText;
    public Vector3 ValidatedPunchAmount => validatedPunchAmount;
    public float ValidatedPunchTime => validatedPunchTime;
    public Color ValidatedColor => validatedColor;
    public Vector3 SpawnPunchAmount => spawnPunchAmount;
    public float SpawnPunchTime => spawnPunchTime;
}

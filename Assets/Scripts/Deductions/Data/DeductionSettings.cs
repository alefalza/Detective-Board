/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: tamaño del grupo, textos y feedback de validación, aparición y contador de las pistas.
 */

using UnityEngine;

[CreateAssetMenu(fileName = "DeductionSettings", menuName = "Detective/Deduction Settings")]
public class DeductionSettings : ScriptableObject
{
    [Header("Check")]
    [Min(2)] [SerializeField] private int groupSize = 3;

    [Header("Texts")]
    [TextArea(2, 4)] [SerializeField] private string farText = "Todavía estás lejos.";

    [Header("Validation Feedback")]
    [SerializeField] private Vector3 validatedPunchAmount = new Vector3(0.2f, 0.2f, 0f);
    [Min(0.05f)] [SerializeField] private float validatedPunchTime = 0.5f;
    [SerializeField] private Color validatedColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    [Header("Appear Feedback")]
    [SerializeField] private Vector3 spawnPunchAmount = new Vector3(0.3f, 0.3f, 0f);
    [Min(0.05f)] [SerializeField] private float spawnPunchTime = 0.5f;

    [Header("Group Counter")]
    [SerializeField] private Color counterColor = new Color(0.15f, 0.15f, 0.15f, 1f);
    [SerializeField] private Color counterOverflowColor = new Color(0.75f, 0.1f, 0.1f, 1f);
    [SerializeField] private Vector3 overflowShakeAmount = new Vector3(0f, 0f, 6f);
    [Min(0.05f)] [SerializeField] private float overflowShakeTime = 0.4f;

    public int GroupSize => groupSize;
    public Color CounterColor => counterColor;
    public Color CounterOverflowColor => counterOverflowColor;
    public Vector3 OverflowShakeAmount => overflowShakeAmount;
    public float OverflowShakeTime => overflowShakeTime;
    public string FarText => farText;
    public Vector3 ValidatedPunchAmount => validatedPunchAmount;
    public float ValidatedPunchTime => validatedPunchTime;
    public Color ValidatedColor => validatedColor;
    public Vector3 SpawnPunchAmount => spawnPunchAmount;
    public float SpawnPunchTime => spawnPunchTime;
}

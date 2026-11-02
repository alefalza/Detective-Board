using UnityEngine;

public class WobblyFloat2D : MonoBehaviour
{
    [Header("Flotación (Posición)")]
    [Tooltip("Distancia vertical que sube y baja el sprite.")]
    [SerializeField] private float floatAmplitude = 0.25f;
    [Tooltip("Velocidad del movimiento vertical.")]
    [SerializeField] private float floatFrequency = 2f;

    [Header("Tambaleo (Rotación Z)")]
    [Tooltip("Ángulo máximo de inclinación en grados.")]
    [SerializeField] private float wobbleAngle = 5f;
    [Tooltip("Velocidad del tambaleo/rotación.")]
    [SerializeField] private float wobbleFrequency = 1.5f;

    [Header("Desfase")]
    [Tooltip("Permite desfasar el inicio para que varios objetos no se muevan idénticos.")]
    [SerializeField] private bool randomOffset = true;

    private Vector3 startPosition;
    private float timeOffset;

    private void Start()
    {
        // Guardamos la posición inicial local para que el objeto pueda moverse o ser padre de otro
        startPosition = transform.localPosition;

        if (randomOffset)
        {
            timeOffset = Random.Range(0f, 100f);
        }
    }

    private void Update()
    {
        float time = Time.time + timeOffset;

        // 1. Calculamos la flotación en Y usando Mathf.Sin
        float newY = startPosition.y + Mathf.Sin(time * floatFrequency) * floatAmplitude;
        transform.localPosition = new Vector3(startPosition.x, newY, startPosition.z);

        if (wobbleAngle == 0 || wobbleFrequency == 0) return;

        float newZRotation = Mathf.Sin(time * wobbleFrequency) * wobbleAngle;
        transform.localRotation = Quaternion.Euler(0f, 0f, newZRotation);
    }
}

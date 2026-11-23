/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: balanceo de una luz 2D como un péndulo, con parpadeo opcional.
 */

using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PendulumLight2D : MonoBehaviour
{
    [SerializeField] private Light2D light2D;

    [Header("Settings")]
    [SerializeField] private float maxAngle = 8f;
    [SerializeField] private float speed = 1.2f;
    [SerializeField] private float randomPhaseOffset = 0f;

    [Header("Flicker")]
    [SerializeField] private bool enableFlicker = false;
    [SerializeField] private float baseIntensity = 0.8f;
    [SerializeField] private float flickerAmount = 0.05f;

    private Quaternion _initialRotation;

    private void Awake()
    {
        _initialRotation = transform.localRotation;
        
        if (randomPhaseOffset == 0f)
        {
            randomPhaseOffset = Random.Range(0f, 100f);
        }
    }

    private void Update()
    {
        float angle = Mathf.Sin((Time.time * speed) + randomPhaseOffset) * maxAngle;
        
        transform.localRotation = _initialRotation * Quaternion.Euler(0f, 0f, angle);

        if (enableFlicker && light2D != null)
        {
            float noise = Mathf.PerlinNoise(Time.time * 5f, 0f);
            light2D.intensity = baseIntensity + (noise - 0.5f) * flickerAmount;
        }
    }
}

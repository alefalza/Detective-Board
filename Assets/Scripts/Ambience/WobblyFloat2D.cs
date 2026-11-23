/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: flotación y tambaleo sutil de un objeto.
 */

using UnityEngine;

public class WobblyFloat2D : MonoBehaviour
{
    [Header("Float")]
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatFrequency = 2f;

    [Header("Wobble")]
    [SerializeField] private float wobbleAngle = 5f;
    [SerializeField] private float wobbleFrequency = 1.5f;

    [Header("Offset")]
    [SerializeField] private bool randomOffset = true;

    private Vector3 startPosition;
    private float timeOffset;

    private void Start()
    {
        startPosition = transform.localPosition;

        if (randomOffset)
        {
            timeOffset = Random.Range(0f, 100f);
        }
    }

    private void Update()
    {
        float time = Time.time + timeOffset;

        float newY = startPosition.y + Mathf.Sin(time * floatFrequency) * floatAmplitude;
        transform.localPosition = new Vector3(startPosition.x, newY, startPosition.z);

        if (wobbleAngle == 0 || wobbleFrequency == 0) return;

        float newZRotation = Mathf.Sin(time * wobbleFrequency) * wobbleAngle;
        transform.localRotation = Quaternion.Euler(0f, 0f, newZRotation);
    }
}

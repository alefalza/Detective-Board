/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: brillito individual que marca dónde hay una pista.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PointSparkle : MonoBehaviour
{
    private Image _image;
    private RectTransform _rect;
    private Vector2 _basePosition;
    private Vector3 _baseScale;

    public bool IsPlaying { get; private set; }

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.raycastTarget = false;
        _rect = (RectTransform)transform;
        _basePosition = _rect.anchoredPosition;
        _baseScale = _rect.localScale;

        Hide();
    }

    public void Play(SparkleSettings settings)
    {
        StopAllCoroutines();
        IsPlaying = true;
        StartCoroutine(BurstRoutine(settings));
    }

    public void Stop()
    {
        StopAllCoroutines();
        IsPlaying = false;

        if (_image != null) Hide();
    }

    private IEnumerator BurstRoutine(SparkleSettings settings)
    {
        yield return new WaitForSeconds(settings.RandomStartDelay);

        int count = settings.RandomTwinkleCount;

        for (int i = 0; i < count; i++)
        {
            yield return TwinkleRoutine(settings);
            yield return new WaitForSeconds(settings.RandomTwinkleGap);
        }

        IsPlaying = false;
    }

    private IEnumerator TwinkleRoutine(SparkleSettings settings)
    {
        float duration = settings.RandomDuration;
        float size = settings.RandomSize;
        float startAngle = Random.Range(0f, 360f);

        _rect.anchoredPosition = _basePosition + Random.insideUnitCircle * settings.PositionJitter;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float intensity = Mathf.Sin(progress * Mathf.PI);

            _rect.localScale = _baseScale * (size * intensity);
            _rect.localRotation = Quaternion.Euler(0f, 0f, startAngle + settings.Spin * progress);
            SetAlpha(intensity);

            yield return null;
        }

        Hide();
    }

    private void OnDisable()
    {
        IsPlaying = false;
        if (_image != null) Hide();
    }

    private void Hide()
    {
        SetAlpha(0f);
        _rect.anchoredPosition = _basePosition;
        _rect.localScale = _baseScale;
    }

    private void SetAlpha(float alpha)
    {
        Color color = _image.color;
        color.a = alpha;
        _image.color = color;
    }
}

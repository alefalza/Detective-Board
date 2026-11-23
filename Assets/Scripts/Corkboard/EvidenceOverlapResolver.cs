/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: separa una pista de otra si queda superpuesta al soltarla.
 */

using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EvidenceOverlapResolver : MonoBehaviour
{
    [Header("Overlap")]
    [SerializeField] private float overlapGap = 0.2f;
    [SerializeField] private float separationDuration = 0.15f;

    private static readonly int[] DirectionPriority = { 0, -1, 1, -2, 2, -3, 3, 4 };

    private Collider2D _collider;
    private Coroutine _slideRoutine;

    public event Action Moved;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    public void CancelSlide()
    {
        if (_slideRoutine == null) return;

        StopCoroutine(_slideRoutine);
        _slideRoutine = null;
    }

    public void Resolve()
    {
        Physics2D.SyncTransforms();

        Collider2D other = FindOverlappingItem(Vector2.zero, 0f, null);
        if (other == null) return;

        Bounds mine = _collider.bounds;
        Bounds theirs = other.bounds;
        Vector2 delta = mine.center - theirs.center;
        Vector2 clearance = (Vector2)(mine.extents + theirs.extents) + Vector2.one * overlapGap;

        int baseStep = Mathf.RoundToInt(Mathf.Atan2(delta.y, delta.x) / (Mathf.PI / 4f));

        foreach (int turn in DirectionPriority)
        {
            Vector2 direction = DirectionFromStep(baseStep + turn);
            Vector2 offset = direction * DistanceToClear(delta, direction, clearance);

            if (FindOverlappingItem(offset, overlapGap, other) != null) continue;

            _slideRoutine = StartCoroutine(SlideTo(transform.position + (Vector3)offset));
            return;
        }
    }

    private static float DistanceToClear(Vector2 delta, Vector2 direction, Vector2 clearance)
    {
        float distance = float.MaxValue;

        if (Mathf.Abs(direction.x) > 0.01f)
            distance = Mathf.Min(distance, (Mathf.Sign(direction.x) * clearance.x - delta.x) / direction.x);

        if (Mathf.Abs(direction.y) > 0.01f)
            distance = Mathf.Min(distance, (Mathf.Sign(direction.y) * clearance.y - delta.y) / direction.y);

        return Mathf.Max(distance, 0f);
    }

    private static Vector2 DirectionFromStep(int step)
    {
        float angle = step * Mathf.PI / 4f;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    private Collider2D FindOverlappingItem(Vector2 offset, float padding, Collider2D ignore)
    {
        Bounds bounds = _collider.bounds;
        Vector2 size = (Vector2)bounds.size + Vector2.one * padding * 2f;

        foreach (Collider2D hit in Physics2D.OverlapBoxAll((Vector2)bounds.center + offset, size, 0f))
        {
            if (hit != _collider && hit != ignore && hit.GetComponent<EvidenceItem>() != null) return hit;
        }
        return null;
    }

    private IEnumerator SlideTo(Vector3 target)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < separationDuration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, Mathf.SmoothStep(0f, 1f, elapsed / separationDuration));
            Moved?.Invoke();
            yield return null;
        }

        transform.position = target;
        Moved?.Invoke();
        _slideRoutine = null;
    }
}

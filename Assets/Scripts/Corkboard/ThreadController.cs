using System.Collections.Generic;
using UnityEngine;

public class ThreadController : MonoBehaviour
{
    [Header("Referencias de Escena")]
    [SerializeField] private Transform threadsContainer;
    [SerializeField] private LineRenderer activeThreadLine;
    [SerializeField] private AudioSource onThreadCutAudioSource;

    [Header("Ajustes de Hilos")]
    [SerializeField] private float clickTolerance = 0.15f;
    [SerializeField] private Color threadColor = Color.red;
    [Range(0f, 1f)] [SerializeField] private float alpha = 0.6f;

    [Header("Gravedad y Físicas del Hilo")]
    [SerializeField] private int segments = 16;
    [SerializeField] private float sagFactor = 0.12f; // caída máxima respecto a la distancia entre pines

    public event System.Action ConnectionsChanged; // se creó o se borró un hilo

    private class Connection
    {
        public LineRenderer line;
        public EvidenceItem from;
        public EvidenceItem to;
        public bool isLocked; // hilos de una deducción validada: no se pueden borrar
    }

    private readonly List<Connection> connections = new List<Connection>();

    public bool HasConnection(EvidenceItem a, EvidenceItem b)
    {
        foreach (var conn in connections)
        {
            if ((conn.from == a && conn.to == b) || (conn.from == b && conn.to == a))
                return true;
        }
        return false;
    }

    public void CreateConnection(EvidenceItem a, EvidenceItem b)
    {
        GameObject lineObj = new GameObject($"Thread_{a.name}_{b.name}");
        if (threadsContainer != null) lineObj.transform.SetParent(threadsContainer);

        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.widthMultiplier = 0.08f;
        SetThreadPositions(line, a.GetPinPosition(), b.GetPinPosition());
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.sortingLayerName = "Threads";

        Color color = threadColor;
        color.a = alpha;
        line.startColor = color;
        line.endColor = color;

        connections.Add(new Connection { line = line, from = a, to = b });
        ConnectionsChanged?.Invoke();
    }

    // Evidencias alcanzables desde 'start' por hilos, pasando solo por items que cumplan 'include'
    public List<EvidenceItem> GetConnectedGroup(EvidenceItem start, System.Func<EvidenceItem, bool> include)
    {
        var group = new List<EvidenceItem> { start };

        for (int i = 0; i < group.Count; i++)
        {
            foreach (var conn in connections)
            {
                EvidenceItem other = null;
                if (conn.from == group[i]) other = conn.to;
                else if (conn.to == group[i]) other = conn.from;

                if (other != null && include(other) && !group.Contains(other))
                    group.Add(other);
            }
        }
        return group;
    }

    // Grupos de evidencias conectadas con exactamente 'size' integrantes, sin los ya validados
    public List<List<EvidenceItem>> GetGroupsOfSize(int size)
    {
        var groups = new List<List<EvidenceItem>>();
        var visited = new HashSet<EvidenceItem>();

        foreach (var conn in connections)
        {
            if (visited.Contains(conn.from)) continue;

            List<EvidenceItem> group = GetConnectedGroup(conn.from, _ => true);
            visited.UnionWith(group);

            if (group.Count == size && !HasLockedConnection(group)) groups.Add(group);
        }
        return groups;
    }

    // true si la evidencia forma parte de un trío ya validado
    public bool IsLocked(EvidenceItem item)
    {
        foreach (var conn in connections)
        {
            if (conn.isLocked && (conn.from == item || conn.to == item)) return true;
        }
        return false;
    }

    private bool HasLockedConnection(List<EvidenceItem> group)
    {
        foreach (var conn in connections)
        {
            if (conn.isLocked && group.Contains(conn.from)) return true;
        }
        return false;
    }

    // Bloquea los hilos que unen a los items dados (deducción validada)
    public void LockConnectionsBetween(IReadOnlyList<EvidenceItem> items)
    {
        foreach (var conn in connections)
        {
            if (ContainsItem(items, conn.from) && ContainsItem(items, conn.to))
                conn.isLocked = true;
        }
        ConnectionsChanged?.Invoke(); // el trío validado deja de contar como consultable
    }

    private static bool ContainsItem(IReadOnlyList<EvidenceItem> items, EvidenceItem item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == item) return true;
        }
        return false;
    }

    public void OnItemMoved(EvidenceItem item)
    {
        foreach (var conn in connections)
        {
            if (conn.from == item || conn.to == item)
                SetThreadPositions(conn.line, conn.from.GetPinPosition(), conn.to.GetPinPosition());
        }
    }

    public void TryRemoveAt(Vector3 worldPos)
    {
        if (!FindRemovableConnectionAt(worldPos, out Connection conn, out _)) return;

        if (onThreadCutAudioSource != null) onThreadCutAudioSource.Play();
        Destroy(conn.line.gameObject);
        connections.Remove(conn);
        ConnectionsChanged?.Invoke();
    }

    public void UpdateHoverMarker(Vector3 worldPos, bool allowed)
    {
        bool hovering = allowed && FindRemovableConnectionAt(worldPos, out _, out _);

        if (hovering)
            CursorManager.Instance.Request(this, GameCursorType.Cut);
        else
            CursorManager.Instance.Release(this);
    }

    public void ShowActiveThread(Vector3 from, Vector3 to)
    {
        if (activeThreadLine == null) return;

        activeThreadLine.enabled = true;
        SetThreadPositions(activeThreadLine, from, to);
    }

    public void HideActiveThread()
    {
        if (activeThreadLine != null) activeThreadLine.enabled = false;
    }

    private bool FindRemovableConnectionAt(Vector3 worldPos, out Connection found, out Vector2 closestPoint)
    {
        for (int i = connections.Count - 1; i >= 0; i--)
        {
            Connection conn = connections[i];
            if (conn.isLocked) continue;

            closestPoint = ClosestPointOnLine(worldPos, conn.line);
            if (Vector2.Distance(worldPos, closestPoint) <= clickTolerance)
            {
                found = conn;
                return true;
            }
        }

        found = null;
        closestPoint = default;
        return false;
    }

    private Vector2 ClosestPointOnSegment(Vector2 point, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        if (ab.sqrMagnitude <= 0f) return a;

        float t = Mathf.Clamp01(Vector2.Dot(point - a, ab) / ab.sqrMagnitude);
        return a + ab * t;
    }

    private Vector2 ClosestPointOnLine(Vector2 point, LineRenderer line)
    {
        Vector2 best = line.GetPosition(0);
        float bestDistance = float.MaxValue;

        for (int i = 0; i < line.positionCount - 1; i++)
        {
            Vector2 candidate = ClosestPointOnSegment(point, line.GetPosition(i), line.GetPosition(i + 1));
            float distance = Vector2.Distance(point, candidate);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = candidate;
            }
        }
        return best;
    }

    private void SetThreadPositions(LineRenderer line, Vector3 a, Vector3 b)
    {
        float sag = Vector3.Distance(a, b) * sagFactor;

        line.positionCount = segments + 1;
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 point = Vector3.Lerp(a, b, t);
            point.y -= sag * 4f * t * (1f - t); // parábola: 0 en los extremos, máxima en el medio
            line.SetPosition(i, point);
        }
    }
}

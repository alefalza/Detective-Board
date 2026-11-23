/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: muestra en las pistas conectadas cuántas tiene su grupo, y las sacude si sobran.
 */

using System.Collections.Generic;
using UnityEngine;

public class GroupCounterPresenter : MonoBehaviour
{
    [SerializeField] private ThreadController threads;
    [SerializeField] private DeductionSettings settings;

    private readonly List<EvidenceItem> _joinOrder = new List<EvidenceItem>();
    private readonly Dictionary<EvidenceItem, List<EvidenceItem>> _neighbors = new Dictionary<EvidenceItem, List<EvidenceItem>>();
    private readonly HashSet<EvidenceItem> _lockedItems = new HashSet<EvidenceItem>();
    private readonly HashSet<EvidenceItem> _visited = new HashSet<EvidenceItem>();

    private int _connectionCount;
    private int _previousConnectionCount;
    private EvidenceItem _lastLinked;

    private void OnEnable() => threads.ConnectionsChanged += Refresh;
    private void OnDisable() => threads.ConnectionsChanged -= Refresh;

    private void Start() => Refresh();

    private void Refresh()
    {
        BuildGraph();

        bool connectionAdded = _connectionCount > _previousConnectionCount;
        _previousConnectionCount = _connectionCount;

        foreach (EvidenceItem item in FindObjectsByType<EvidenceItem>(FindObjectsSortMode.None))
            item.HideGroupCounter();

        _visited.Clear();
        foreach (EvidenceItem start in _joinOrder)
        {
            if (_visited.Contains(start)) continue;

            List<EvidenceItem> group = CollectGroup(start);
            if (_lockedItems.Contains(start)) continue;

            int total = settings.GroupSize;
            bool overflow = group.Count > total;
            Color color = overflow ? settings.CounterOverflowColor : settings.CounterColor;

            string counter = $"{group.Count}/{total}";
            foreach (EvidenceItem item in group)
                item.ShowGroupCounter(counter, color);

            if (overflow && connectionAdded && group.Contains(_lastLinked)) Shake(group);
        }
    }

    private void Shake(List<EvidenceItem> group)
    {
        foreach (EvidenceItem item in group)
            item.PlayShake(settings.OverflowShakeAmount, settings.OverflowShakeTime);
    }

    private void BuildGraph()
    {
        _joinOrder.Clear();
        _neighbors.Clear();
        _lockedItems.Clear();
        _connectionCount = 0;

        foreach (var (from, to, isLocked) in threads.Connections)
        {
            Link(from, to);
            Link(to, from);
            _connectionCount++;
            _lastLinked = to;

            if (!isLocked) continue;
            _lockedItems.Add(from);
            _lockedItems.Add(to);
        }
    }

    private void Link(EvidenceItem item, EvidenceItem other)
    {
        if (!_neighbors.TryGetValue(item, out List<EvidenceItem> list))
        {
            list = new List<EvidenceItem>();
            _neighbors.Add(item, list);
            _joinOrder.Add(item);
        }

        if (!list.Contains(other)) list.Add(other);
    }

    private List<EvidenceItem> CollectGroup(EvidenceItem start)
    {
        var members = new HashSet<EvidenceItem> { start };
        var pending = new Queue<EvidenceItem>();
        pending.Enqueue(start);

        while (pending.Count > 0)
        {
            foreach (EvidenceItem other in _neighbors[pending.Dequeue()])
            {
                if (members.Add(other)) pending.Enqueue(other);
            }
        }

        _visited.UnionWith(members);
        return _joinOrder.FindAll(members.Contains);
    }
}

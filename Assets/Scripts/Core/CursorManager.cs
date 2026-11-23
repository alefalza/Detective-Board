/*
 * Nombre: Alejandro Falzarano
 * Mail: falzarano.alejandro@gmail.com
 * 
 * Descripción: muestra el cursor de mayor prioridad entre los que piden los distintos sistemas.
 */

using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameCursorType
{
    Default,
    Cut,
    Hover,
    Grab,
    Magnifier,
    Mark,
    Pin
}

public class CursorManager : Singleton<CursorManager>
{
    [Serializable]
    public struct CursorData
    {
        public GameCursorType type;
        public Texture2D texture;
        public Vector2 hotspot;
        public int priority;
    }

    private struct CursorRequest
    {
        public GameCursorType type;
        public int priority;
        public int order;
    }

    [SerializeField] private CursorData[] cursorList;

    private readonly Dictionary<object, CursorRequest> _requests = new Dictionary<object, CursorRequest>();
    private Dictionary<GameCursorType, CursorData> _cursorDict;
    private GameCursorType _currentCursorType = GameCursorType.Default;
    private int _requestCounter;

    private readonly object _legacyOwner = new object();

    protected override void Awake()
    {
        base.Awake();
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        _cursorDict = new Dictionary<GameCursorType, CursorData>();
        foreach (var data in cursorList)
        {
            if (!_cursorDict.ContainsKey(data.type))
            {
                _cursorDict.Add(data.type, data);
            }
        }
    }

    public void Request(object owner, GameCursorType type)
    {
        if (_requests.TryGetValue(owner, out CursorRequest existing) && existing.type == type) return;

        _requests[owner] = new CursorRequest
        {
            type = type,
            priority = GetPriority(type),
            order = ++_requestCounter
        };
        Refresh();
    }

    public void Release(object owner)
    {
        if (_requests.Remove(owner)) Refresh();
    }

    private int GetPriority(GameCursorType type)
    {
        return _cursorDict.TryGetValue(type, out CursorData data) ? data.priority : 0;
    }

    private void Refresh()
    {
        GameCursorType best = GameCursorType.Default;
        int bestPriority = int.MinValue;
        int bestOrder = int.MinValue;

        foreach (CursorRequest request in _requests.Values)
        {
            bool wins = request.priority > bestPriority
                        || (request.priority == bestPriority && request.order > bestOrder);
            if (!wins) continue;

            best = request.type;
            bestPriority = request.priority;
            bestOrder = request.order;
        }

        Apply(best);
    }

    private void Apply(GameCursorType type)
    {
        if (_currentCursorType == type) return;
        _currentCursorType = type;

        if (_cursorDict.TryGetValue(type, out CursorData data))
        {
            Cursor.SetCursor(data.texture, data.hotspot, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimStateRelay : MonoBehaviour
{
    [Serializable]
    public class KeyedEvents
    {
        public string key;                // e.g., "Entrance", "Exit"
        public UnityEvent onEnter;
        public UnityEvent onUpdate;
        public UnityEvent onExit;         // <-- You'll mostly use this
    }

    [Header("Map state keys to events")]
    public List<KeyedEvents> mappings = new List<KeyedEvents>();

    Dictionary<string, KeyedEvents> _lookup;

    void Awake()
    {
        _lookup = new Dictionary<string, KeyedEvents>(StringComparer.Ordinal);
        foreach (var m in mappings)
        {
            if (!string.IsNullOrEmpty(m.key) && !_lookup.ContainsKey(m.key))
                _lookup.Add(m.key, m);
        }
    }

    public void InvokeEnter(string key)
    {
        if (key != null && _lookup != null && _lookup.TryGetValue(key, out var m))
            m.onEnter?.Invoke();
    }

    public void InvokeUpdate(string key)
    {
        if (key != null && _lookup != null && _lookup.TryGetValue(key, out var m))
            m.onUpdate?.Invoke();
    }

    public void InvokeExit(string key)
    {
        if (key != null && _lookup != null && _lookup.TryGetValue(key, out var m))
            m.onExit?.Invoke();
    }
}

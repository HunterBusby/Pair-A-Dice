// IntData.cs
using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "IntData")]
public class IntData : ScriptableObject
{
    [Header("Persistence")]
    [Tooltip("Unique key in save file. If empty, uses asset name.")]
    public string saveKey = "";

    [Header("Runtime Value")]
    public int value;

    [Header("Reset Defaults")]
    public int defaultValue = 0;

    [Header("Events")]
    public UnityEvent onValueChanged;
    public event Action onValueChangedCSharp;

    public void SetValue(int num) { value = num; OnChanged(); }
    public void SetValue(IntData obj) { if (obj) { value = obj.value; OnChanged(); } }

    public void Add(int amount) { value += amount; OnChanged(); }
    public void Add(IntData other) { if (!other) return; value += other.value; OnChanged(); }

    public void Subtract(int amount) { value -= amount; OnChanged(); }
    public void Subtract(IntData other) { if (!other) return; value -= other.value; }

    public void CompareValue(IntData obj)
    {
        if (!obj) return;
        if (value < obj.value) { value = obj.value; OnChanged(); }
    }

    public void ResetToDefault() { value = defaultValue; OnChanged(); }

    private void OnChanged()
    {
        onValueChanged?.Invoke();
        onValueChangedCSharp?.Invoke();
        // No direct disk IO here — SaveManager handles it when you call Save()
    }
}

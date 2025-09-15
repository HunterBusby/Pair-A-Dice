using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "IntData")]
public class IntData : ScriptableObject
{
    public int value;

    public void SetValue(int num)            { value = num; OnChanged(); }
    public void SetValue(IntData obj)        { if (obj) { value = obj.value; OnChanged(); } }

    // --- Add (int / IntData) ---
    public void Add(int amount)              { value += amount; OnChanged(); }
    public void Add(IntData other)           { if (!other) return; value += other.value; OnChanged(); }

    // --- Subtract (int / IntData) ---
    public void Subtract(int amount)         { value -= amount; OnChanged(); }
    public void Subtract(IntData other)      { if (!other) return; value -= other.value; OnChanged(); }

    // Keep your compare/reset/event bits as before:
    public void CompareValue(IntData obj)
    {
        if (!obj) return;
        if (value < obj.value) { value = obj.value; OnChanged(); }
    }

    [Header("Reset Defaults")]
    public int defaultValue = 0;
    public void ResetToDefault()             { value = defaultValue; OnChanged(); }

    public UnityEvent onValueChanged;
    public event Action onValueChangedCSharp;
    private void OnChanged()
    {
        onValueChanged?.Invoke();
        onValueChangedCSharp?.Invoke();
    }
}

using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "IntData")]
public class IntData : ScriptableObject
{
    // Existing API (unchanged)
    public int value;

    public void SetValue(int num)            { value = num; OnChanged(); }
    public void SetValue(IntData obj)        { if (obj) { value = obj.value; OnChanged(); } }
    public void UpdateValue(int num)         { value += num; OnChanged(); }

    // Existing CompareValue kept as-is
    public void CompareValue(IntData obj)
    {
        if (!obj) return;
        if (value < obj.value)
        {
            value = obj.value;
            OnChanged();
        }
    }

    // 🔹 New: per-asset default for resets (safe additive field)
    [Header("Reset Defaults")]
    public int defaultValue = 0;

    // 🔹 New: convenience reset method (optional)
    public void ResetToDefault()
    {
        value = defaultValue;
        OnChanged();
    }

    // 🔹 Optional: event hooks (don’t break anything if unused)
    public UnityEvent onValueChanged;   // can wire in Inspector if you want

    // If you prefer C# event instead (code-only):
    public event Action onValueChangedCSharp;

    private void OnChanged()
    {
        onValueChanged?.Invoke();
        onValueChangedCSharp?.Invoke();
    }
}

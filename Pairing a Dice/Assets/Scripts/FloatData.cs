// FloatData.cs
using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "FloatData")]
public class FloatData : ScriptableObject
{
    [Header("Persistence")]
    [Tooltip("Unique key in save file. If empty, uses asset name.")]
    public string saveKey = "";

    [Header("Runtime Value")]
    public float value;

    [Header("Reset Defaults")]
    public float defaultValue = 0f;

    [Header("Events")]
    public UnityEvent onValueChanged;
    public event Action onValueChangedCSharp;

    public void SetValue(float num) { value = num; OnChanged(); }
    public void UpdateValue(float num) { value += num; OnChanged(); }
    public void ResetToDefault() { value = defaultValue; OnChanged(); }

    private void OnChanged()
    {
        onValueChanged?.Invoke();
        onValueChangedCSharp?.Invoke();
        // SaveManager writes to disk when you call Save()
    }
}

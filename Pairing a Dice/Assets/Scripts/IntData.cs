using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "IntData")]
public class IntData : ScriptableObject
{
    [Header("Persistence")]
    [Tooltip("Leave empty to use asset name as the key.")]
    public string saveKey = "";
    public bool autoSave = true;   // saves on every change
    public bool autoLoadOnEnable = true;

    [Header("Runtime Value")]
    public int value;

    [Header("Reset Defaults")]
    public int defaultValue = 0;

    [Header("Events")]
    public UnityEvent onValueChanged;
    public event Action onValueChangedCSharp;

    private string Key => string.IsNullOrEmpty(saveKey) ? name : saveKey;

    private void OnEnable()
    {
        if (autoLoadOnEnable) LoadNow();
    }

    // ---- Public API you already use ----
    public void SetValue(int num)            { value = num; OnChanged(); }
    public void SetValue(IntData other)      { if (other) { value = other.value; OnChanged(); } }
    public void Add(int amount)              { value += amount; OnChanged(); }
    public void Add(IntData other)           { if (other) { value += other.value; OnChanged(); } }
    public void Subtract(int amount)         { value -= amount; OnChanged(); }
    public void Subtract(IntData other)      { if (other) { value -= other.value; OnChanged(); } }

    public void CompareValue(IntData other)
    {
        if (!other) return;
        if (value < other.value) { value = other.value; OnChanged(); }
    }

    public void ResetToDefault() { value = defaultValue; OnChanged(); }

    // ---- Persistence (simple & robust) ----
    public void SaveNow()
    {
        PlayerPrefs.SetInt(Key, value);
        PlayerPrefs.Save();
    }

    public void LoadNow()
    {
        value = PlayerPrefs.HasKey(Key) ? PlayerPrefs.GetInt(Key, defaultValue) : defaultValue;
        // Fire events so UI/logic refresh
        onValueChanged?.Invoke();
        onValueChangedCSharp?.Invoke();
    }

    public void DeleteSaved()
    {
        if (PlayerPrefs.HasKey(Key)) PlayerPrefs.DeleteKey(Key);
    }

    private void OnChanged()
    {
        onValueChanged?.Invoke();
        onValueChangedCSharp?.Invoke();
        if (autoSave) SaveNow();
    }
}

using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "FloatData")]
public class FloatData : ScriptableObject
{
    [Header("Persistence")]
    [Tooltip("Leave empty to use asset name as the key.")]
    public string saveKey = "";
    public bool autoSave = true;
    public bool autoLoadOnEnable = true;

    [Header("Runtime Value")]
    public float value;

    [Header("Reset Defaults")]
    public float defaultValue = 0f;

    [Header("Events")]
    public UnityEvent onValueChanged;
    public event Action onValueChangedCSharp;

    private string Key => string.IsNullOrEmpty(saveKey) ? name : saveKey;

    private void OnEnable()
    {
        if (autoLoadOnEnable) LoadNow();
    }

    public void SetValue(float num)   { value = num; OnChanged(); }
    public void UpdateValue(float num){ value += num; OnChanged(); }
    public void ResetToDefault()      { value = defaultValue; OnChanged(); }

    // Persistence
    public void SaveNow()
    {
        PlayerPrefs.SetFloat(Key, value);
        PlayerPrefs.Save();
    }

    public void LoadNow()
    {
        value = PlayerPrefs.HasKey(Key) ? PlayerPrefs.GetFloat(Key, defaultValue) : defaultValue;
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

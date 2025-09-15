using UnityEngine;

[CreateAssetMenu(menuName = "Data/FloatData")]
public class FloatData : ScriptableObject
{
    [Header("Runtime Value")]
    public float value;

    [Header("Reset Defaults")]
    public float defaultValue = 0f;   // 👈 shows up in inspector

    public void SetValue(float num)
    {
        value = num;
    }

    public void UpdateValue(float num)
    {
        value += num;
    }

    // 🔹 Reset to default
    public void ResetToDefault()
    {
        value = defaultValue;
    }
}

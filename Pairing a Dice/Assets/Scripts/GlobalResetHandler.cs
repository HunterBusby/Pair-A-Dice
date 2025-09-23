using UnityEngine;

public class GlobalResetHandler : MonoBehaviour
{
    public IntData[] intDataToReset;
    public FloatData[] floatDataToReset;

    void Awake()
    {
        ResetIntValues();
        ResetFloatValues(); // ✅ add this
    }

    void OnApplicationQuit()
    {
        ResetIntValues();
        ResetFloatValues(); // ✅ add this
    }

    public void ResetIntValues()
    {
        if (intDataToReset == null) return;
        foreach (var data in intDataToReset)
        {
            if (data == null) continue;
            data.ResetToDefault();
            Debug.Log($"🔄 Reset {data.name} to {data.defaultValue}.");
        }
    }

    public void ResetFloatValues()
    {
        if (floatDataToReset == null) return;
        foreach (var data in floatDataToReset)
        {
            if (data == null) continue;
            data.ResetToDefault();
            Debug.Log($"🔄 Reset {data.name} to {data.defaultValue}.");
        }
    }
}

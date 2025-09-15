using UnityEngine;

public class GlobalResetHandler : MonoBehaviour
{
    public IntData[] intDataToReset;

    public FloatData[] floatDataToReset;

    void Awake()
    {
        ResetIntValues();
    }

    void OnApplicationQuit()
    {
        ResetIntValues();
    }

    public void ResetIntValues()
    {
        foreach (IntData data in intDataToReset)
        {
            if (data != null)
            {
                data.ResetToDefault(); // 👈 use per-asset default
                Debug.Log($"🔄 Reset {data.name} to {data.defaultValue}.");
            }
        }
    }

    public void ResetAllValues()
    {
        foreach (FloatData data in floatDataToReset)
        {
            if (data != null)
            {
                data.ResetToDefault(); // 👈 use per-asset default
                Debug.Log($"🔄 Reset {data.name} to {data.defaultValue}.");
            }
        }
    }
}

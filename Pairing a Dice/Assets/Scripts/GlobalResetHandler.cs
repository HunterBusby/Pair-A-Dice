using UnityEngine;

public class GlobalResetHandler : MonoBehaviour
{
    public IntData[] intDataToReset; // ✅ Assign ScriptableObjects that need resetting

    void Awake()
    {
        ResetAllValues();
    }

    void OnApplicationQuit()
    {
        ResetAllValues();
    }

    public void ResetAllValues()
    {
        foreach (IntData data in intDataToReset)
        {
            if (data != null)
            {
                data.SetValue(0); // ✅ Reset all assigned IntData assets
                Debug.Log($"🔄 Reset {data.name} to 0.");
            }
        }
    }
}

using UnityEngine;
using UnityEngine.Events;

public class IntDataThresholdChecker : MonoBehaviour
{
    public IntData trackedValue; // ✅ Assign in Unity (tracks progress)
    public int threshold = 5; // ✅ Changeable in Unity Editor (threshold value)
    public UnityEvent onThresholdReached; // ✅ Event triggered when threshold is met

    void Update()
    {
        if (trackedValue == null)
        {
            Debug.LogError("❌ No IntData assigned to IntDataThresholdChecker!");
            return;
        }

        if (trackedValue.value >= threshold)
        {
            Debug.Log($"🎉 Threshold reached! ({trackedValue.value}/{threshold})");
            onThresholdReached.Invoke(); // ✅ Trigger the event
            trackedValue.SetValue(0); // ✅ Reset value after triggering event
        }
    }
}

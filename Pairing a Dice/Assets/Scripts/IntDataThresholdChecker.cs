using UnityEngine;
using UnityEngine.Events;

public class IntDataThresholdChecker : MonoBehaviour
{
    [Header("Config")]
    public IntData trackedValue;     // Assign in Inspector
    public float threshold = 5;

    [Tooltip("If enabled, sets IntData to 0 after the event fires.")]
    public bool resetOnThreshold = true;

    [Tooltip("If NOT resetting, only fire once, then re-arm after the value drops below the threshold again.")]
    public bool rearmWhenBelowThreshold = true;

    [Header("Events")]
    public UnityEvent onThresholdReached;

    // internal state to prevent spamming every frame
    private bool armed = true;

    void Update()
    {
        if (trackedValue == null)
        {
            Debug.LogError("❌ No IntData assigned to IntDataThresholdChecker!");
            return;
        }

        // NOTE: using `.value` (lowercase) to match your IntData shape
        if (armed && trackedValue.value >= threshold)
        {
            Debug.Log($"🎉 Threshold reached! ({trackedValue.value}/{threshold})");
            onThresholdReached.Invoke();

            if (resetOnThreshold)
            {
                trackedValue.SetValue(0);
                armed = true; // immediately ready for next cycle
            }
            else
            {
                // Don’t reset the value; disarm until we drop below threshold (if desired)
                armed = !rearmWhenBelowThreshold;
            }
        }

        // If we’re disarmed and watching for the value to drop below threshold, re-arm when it does
        if (!armed && rearmWhenBelowThreshold && trackedValue.value < threshold)
        {
            armed = true;
        }
    }

    public void SetThreshold(float value)
    {
        threshold = Mathf.Max(0.1f, value); // prevent zero/negative
    }

    public void AdjustThreshold(float delta)
    {
        threshold = Mathf.Max(0.1f, threshold + delta);
    }
}

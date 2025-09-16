using UnityEngine;
using UnityEngine.Events;

public class IntDataThresholdChecker : MonoBehaviour
{
    [Header("Config")]
    public IntData trackedValue;               // Assign in Inspector
    public float threshold = 5f;               // Default float threshold (kept for backward compatibility)

    [Header("Optional Threshold Source (IntData)")]
    public bool useThresholdData = false;      // OFF by default so old scenes behave the same
    public IntData thresholdData;              // If set and useThresholdData == true, this drives the threshold

    [Tooltip("If enabled, sets IntData to 0 after the event fires.")]
    public bool resetOnThreshold = true;

    [Tooltip("If NOT resetting, only fire once, then re-arm after the value drops below the threshold again.")]
    public bool rearmWhenBelowThreshold = true;

    [Header("Events")]
    public UnityEvent onThresholdReached;

    // internal state to prevent spamming every frame
    private bool armed = true;

    // --- Helpers ---
    private int CurrentThresholdInt
    {
        get
        {
            // If using IntData and it exists, read from it; otherwise use the float field.
            if (useThresholdData && thresholdData != null)
                return thresholdData.value;

            // Fallback to float threshold (round to int for comparison with IntData.value)
            return Mathf.RoundToInt(threshold);
        }
    }

    void Update()
    {
        if (trackedValue == null)
        {
            Debug.LogError("❌ No IntData assigned to IntDataThresholdChecker!");
            return;
        }

        int currentThreshold = CurrentThresholdInt;

        if (armed && trackedValue.value >= currentThreshold)
        {
            Debug.Log($"🎉 Threshold reached! ({trackedValue.value}/{currentThreshold})");
            onThresholdReached?.Invoke();

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
        if (!armed && rearmWhenBelowThreshold && trackedValue.value < currentThreshold)
        {
            armed = true;
        }
    }

    // --- Float path setters (kept for backward compatibility) ---
    public void SetThreshold(float value)
    {
        threshold = Mathf.Max(0.1f, value); // prevent zero/negative
    }

    public void AdjustThreshold(float delta)
    {
        threshold = Mathf.Max(0.1f, threshold + delta);
    }

    // --- IntData path helpers (UnityEvent-friendly) ---

    /// <summary>Enable/disable using IntData as the threshold source.</summary>
    public void UseThresholdData(bool useData)
    {
        useThresholdData = useData;
    }

    /// <summary>Assign the IntData asset to use as the threshold source (also enables use flag).</summary>
    public void SetThresholdData(IntData data)
    {
        thresholdData = data;
        useThresholdData = (data != null);
    }

    /// <summary>Copy the current IntData value into the float threshold (does NOT enable useThresholdData).</summary>
    public void CopyThresholdFromDataOnce()
    {
        if (thresholdData != null)
        {
            threshold = Mathf.Max(0.1f, thresholdData.value);
        }
    }

    /// <summary>Set the threshold IntData's value (if present). Useful for UI buttons/sliders.</summary>
    public void SetThresholdDataValue(IntData data)
    {
        if (data != null)
        {
            thresholdData = data;
            useThresholdData = true;
        }
    }
}

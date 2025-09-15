using UnityEngine;
using UnityEngine.Events;

public class IntDataChecker : MonoBehaviour
{
    [Header("Target Data (ScriptableObject)")]
    public IntData currentValue;   // Drag your IntData asset here

    [Header("Comparison Target")]
    public int targetValue;

    [Header("Invoke Mode")]
    [Tooltip("If true, only the first matching branch fires (Equal > Greater > Less). If false, all matching events can fire (e.g., Equal and GreaterOrEqual).")]
    public bool fireOnlyOneEvent = true;

    [Header("Events")]
    public UnityEvent onEqual;
    public UnityEvent onGreater;
    public UnityEvent onLess;
    public UnityEvent onGreaterOrEqual;
    public UnityEvent onLessOrEqual;

    /// <summary>
    /// Call this from a Button/UnityEvent/Timeline/AnimationEvent/etc.
    /// Reads currentValue.value and compares to targetValue.
    /// </summary>
    public void Check()
    {
        if (currentValue == null)
        {
            Debug.LogWarning($"{nameof(IntDataChecker)} on '{name}' has no IntData assigned.", this);
            return;
        }

        int v = currentValue.value;

        if (fireOnlyOneEvent)
        {
            if (v == targetValue) { onEqual?.Invoke(); return; }
            if (v >  targetValue) { onGreater?.Invoke(); onGreaterOrEqual?.Invoke(); return; }
            // v < targetValue
            onLess?.Invoke(); onLessOrEqual?.Invoke();
            return;
        }

        // Fire all matching events (non-exclusive)
        if (v == targetValue) onEqual?.Invoke();
        if (v >  targetValue) onGreater?.Invoke();
        if (v <  targetValue) onLess?.Invoke();
        if (v >= targetValue) onGreaterOrEqual?.Invoke();
        if (v <= targetValue) onLessOrEqual?.Invoke();
    }

    // ---------- Modify helpers (all UnityEvent-friendly, no Update) ----------

    /// <summary>Set the numeric target directly.</summary>
    public void SetTargetValue(int newTarget) => targetValue = newTarget;

    /// <summary>Set the target from another IntData asset.</summary>
    public void SetTargetValue(IntData source)
    {
        if (source == null) { Debug.LogWarning("SetTargetFromSO: source is null.", this); return; }
        targetValue = source.value;
    }

    /// <summary>Increase the target by a raw int, then Check(). Pass negative to decrease.</summary>
    public void UpdateTargetValue(int increaseBy)
    {
        targetValue += increaseBy;
    }

    /// <summary>Increase the target by another IntData's value, then Check().</summary>
    public void UpdateTargetValue(IntData increaseBySO)
    {
        if (increaseBySO == null) { Debug.LogWarning("IncreaseTargetValue(IntData): increaseBySO is null.", this); return; }
        targetValue += increaseBySO.value;
    }
}

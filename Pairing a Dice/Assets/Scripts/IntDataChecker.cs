using UnityEngine;
using UnityEngine.Events;

public class IntDataChecker : MonoBehaviour
{
    [Header("Target Data (ScriptableObjects)")]
    public IntData currentValue;          // ✅ existing int path (leave as-is on old objects)
    public FloatData currentFloatValue;   // ✅ optional: assign to use float path

    [Header("Comparison Target")]
    public int targetValue = 0;           // ✅ existing int target (used when no FloatData)
    public float targetValueFloat = 0f;   // ✅ optional float target (used when FloatData assigned)

    [Header("Invoke Mode")]
    [Tooltip("If true, only the first matching branch fires (Equal > Greater > Less). If false, all matching events can fire (e.g., Equal and GreaterOrEqual).")]
    public bool fireOnlyOneEvent = true;

    [Header("Float Equality")]
    [Tooltip("Tolerance for float equality checks (ignored for int mode).")]
    public float epsilon = 0.0001f;

    [Header("Events")]
    public UnityEvent onEqual;
    public UnityEvent onGreater;
    public UnityEvent onLess;
    public UnityEvent onGreaterOrEqual;
    public UnityEvent onLessOrEqual;

    /// <summary>
    /// Call this from a Button/UnityEvent/etc. Compares either:
    ///  - FloatData vs targetValueFloat (if currentFloatValue is assigned), OR
    ///  - IntData   vs targetValue      (fallback)
    /// </summary>
    public void Check()
    {
        if (currentFloatValue != null)
        {
            // Float mode
            float v = currentFloatValue.value;
            float t = targetValueFloat;

            if (fireOnlyOneEvent)
            {
                if (Mathf.Abs(v - t) <= epsilon) { onEqual?.Invoke(); return; }
                if (v > t) { onGreater?.Invoke(); onGreaterOrEqual?.Invoke(); return; }
                // v < t
                onLess?.Invoke(); onLessOrEqual?.Invoke(); return;
            }

            // Non-exclusive
            if (Mathf.Abs(v - t) <= epsilon) onEqual?.Invoke();
            if (v > t) onGreater?.Invoke();
            if (v < t) onLess?.Invoke();
            if (v >= t - epsilon) onGreaterOrEqual?.Invoke();
            if (v <= t + epsilon) onLessOrEqual?.Invoke();
            return;
        }

        // Int mode (backward compatible)
        if (currentValue == null)
        {
            Debug.LogWarning($"{nameof(IntDataChecker)} on '{name}' has no IntData or FloatData assigned.", this);
            return;
        }

        int vi = currentValue.value;
        int ti = targetValue;

        if (fireOnlyOneEvent)
        {
            if (vi == ti) { onEqual?.Invoke(); return; }
            if (vi >  ti) { onGreater?.Invoke(); onGreaterOrEqual?.Invoke(); return; }
            // vi < ti
            onLess?.Invoke(); onLessOrEqual?.Invoke(); return;
        }

        // Non-exclusive
        if (vi == ti) onEqual?.Invoke();
        if (vi >  ti) onGreater?.Invoke();
        if (vi <  ti) onLess?.Invoke();
        if (vi >= ti) onGreaterOrEqual?.Invoke();
        if (vi <= ti) onLessOrEqual?.Invoke();
    }

    // ---------- Modify helpers (UnityEvent-friendly) ----------

    // Int targets (existing)
    public void SetTargetValue(int newTarget) => targetValue = newTarget;
    public void SetTargetValue(IntData source)
    {
        if (source == null) { Debug.LogWarning("SetTargetValue(IntData): source is null.", this); return; }
        targetValue = source.value;
    }
    public void UpdateTargetValue(int delta) => targetValue += delta;
    public void UpdateTargetValue(IntData deltaSO)
    {
        if (deltaSO == null) { Debug.LogWarning("UpdateTargetValue(IntData): deltaSO is null.", this); return; }
        targetValue += deltaSO.value;
    }

    // Float targets (new)
    public void SetTargetValueFloat(float newTarget) => targetValueFloat = newTarget;
    public void SetTargetValue(FloatData source)
    {
        if (source == null) { Debug.LogWarning("SetTargetValue(FloatData): source is null.", this); return; }
        targetValueFloat = source.value;
    }
    public void UpdateTargetValueFloat(float delta) => targetValueFloat += delta;
    public void UpdateTargetValue(FloatData deltaSO)
    {
        if (deltaSO == null) { Debug.LogWarning("UpdateTargetValue(FloatData): deltaSO is null.", this); return; }
        targetValueFloat += deltaSO.value;
    }
}

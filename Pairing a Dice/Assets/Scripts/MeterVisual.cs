using UnityEngine;

public class MeterVisual : MonoBehaviour
{
    [Header("ScriptableObjects")]
    public IntData meterData;        // ✅ Current meter progress
    public IntData maxDoublesNeeded; // ✅ Reference SO for the max doubles

    [Header("Visuals")]
    public Transform meterObject;    // ✅ The rotating object (e.g., floating dice)

    [Header("Rotation Settings")]
    public float maxRotationSpeed = 200f; // ✅ Spin speed at full meter
    public float minRotationSpeed = 20f;  // ✅ Spin speed when empty
    
    private Vector3 baseRotationAxis = Vector3.up; // ✅ Main rotation axis (Y-axis)
    private Vector3 randomOffset;                  // ✅ Adds variation
    private float offsetChangeRate = 0.5f;         // ✅ How fast the offset shifts

    void Start()
    {
        GenerateNewOffset(); // ✅ Pick a starting variation
    }

    void Update()
    {
        if (meterObject == null || meterData == null || maxDoublesNeeded == null) return;

        // ✅ Use SO values (avoid divide-by-zero if maxDoublesNeeded is 0)
        float normalized = (maxDoublesNeeded.value > 0) 
            ? (float)meterData.value / maxDoublesNeeded.value 
            : 0f;

        float rotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, normalized);

        // ✅ Rotate with subtle random variation
        meterObject.Rotate((baseRotationAxis + randomOffset) * rotationSpeed * Time.deltaTime);

        // ✅ Smoothly shift the offset over time
        randomOffset = Vector3.Lerp(randomOffset, GenerateNewOffset(), offsetChangeRate * Time.deltaTime);
    }

    private Vector3 GenerateNewOffset()
    {
        return new Vector3(
            Random.Range(-20f, 20f),
            0,
            Random.Range(-20f, 20f)
        );
    }
}

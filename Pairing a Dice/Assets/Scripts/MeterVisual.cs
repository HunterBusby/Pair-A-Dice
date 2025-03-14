using UnityEngine;

public class MeterVisual : MonoBehaviour
{
    public IntData meterData; // ✅ Link the ScriptableObject in the Inspector
    public Transform meterObject; // ✅ Assign the rotating object (e.g., floating dice)
    public float maxRotationSpeed = 200f; // ✅ Maximum spin speed at full meter
    public float minRotationSpeed = 20f;  // ✅ Slowest spin speed when empty
    public int maxDoublesNeeded = 3; // ✅ Should match `maxDoublesNeeded` in `IntData`
    
    private Vector3 baseRotationAxis = Vector3.up; // ✅ Main rotation axis (Y-axis)
    private Vector3 randomOffset; // ✅ Adds variation to other axes
    private float offsetChangeRate = 0.5f; // ✅ How fast the offset shifts (smoothness)

    void Start()
    {
        GenerateNewOffset(); // ✅ Pick a starting variation
    }

    void Update()
    {
        if (meterObject == null || meterData == null) return;

        // ✅ Adjust speed based on meter value
        float rotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, (float)meterData.value / maxDoublesNeeded);

        // ✅ Rotate on a main axis (Y) with a subtle random variation
        meterObject.Rotate((baseRotationAxis + randomOffset) * rotationSpeed * Time.deltaTime);

        // ✅ Gradually shift the offset for a natural feel
        randomOffset = Vector3.Lerp(randomOffset, GenerateNewOffset(), offsetChangeRate * Time.deltaTime);
    }

    private Vector3 GenerateNewOffset()
    {
        // ✅ Creates a slight variation, but avoids sudden resets
        return new Vector3(
            Random.Range(-20f, 20f), // Subtle variation on X
            0, // Keeps Y stable for primary rotation
            Random.Range(-20f, 20f) // Subtle variation on Z
        );
    }
}

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShakeToRoll : MonoBehaviour
{
    private Rigidbody rb;
    private Camera mainCamera;
    private float cameraZDistance;
    private Vector3 lastMousePosition;
    private float shakeIntensity = 0f;
    
    [Header("Shake Settings")]
    public float shakeThreshold = 8f;  // ✅ How much shaking is needed to roll
    public float shakeDecay = 2f;  // ✅ How quickly shake fades when not moving
    public float forceMultiplier = 5f;  // ✅ Roll force strength
    public float torqueMultiplier = 10f;  // ✅ Spin strength

    private bool isShaking = false;
    private static ShakeToRoll[] allDice; // ✅ Reference to all dice in the scene

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        cameraZDistance = mainCamera.WorldToScreenPoint(transform.position).z;

        // ✅ Find all dice that have ShakeToRoll
        if (allDice == null || allDice.Length == 0)
        {
            allDice = FindObjectsByType<ShakeToRoll>(FindObjectsSortMode.None);
        }
    }

    void Update()
    {
        if (isShaking)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            shakeIntensity += mouseDelta.magnitude;  // ✅ Increase shake intensity based on movement
            shakeIntensity = Mathf.Clamp(shakeIntensity, 0, shakeThreshold * 2);  // ✅ Prevent extreme values

            lastMousePosition = Input.mousePosition;

            if (shakeIntensity >= shakeThreshold)
            {
                RollAllDice();  // ✅ Roll both dice together
                isShaking = false;
                shakeIntensity = 0;
            }
        }
        else
        {
            // ✅ Gradually decay shake intensity if not shaking
            shakeIntensity = Mathf.Max(0, shakeIntensity - (shakeDecay * Time.deltaTime));
        }
    }

    private void OnMouseDown()
    {
        isShaking = true;
        lastMousePosition = Input.mousePosition;
        Debug.Log("🎲 Dice shaking started!");
    }

    private void OnMouseUp()
    {
        isShaking = false;
        Debug.Log("🎲 Dice shaking stopped!");
    }

    private void RollAllDice()
    {
        foreach (ShakeToRoll die in allDice) // ✅ Roll ALL dice in the scene
        {
            die.RollDice();
        }
    }

    private void RollDice()
    {
        rb.linearVelocity = Vector3.zero;  
        rb.angularVelocity = Vector3.zero;

        // ✅ Apply random roll force and torque
        Vector3 rollForce = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)) * forceMultiplier;
        Vector3 rollTorque = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)) * torqueMultiplier;

        rb.AddForce(rollForce, ForceMode.Impulse);
        rb.AddTorque(rollTorque, ForceMode.Impulse);

        Debug.Log("🎲 Dice rolled after shaking!");
    }
}

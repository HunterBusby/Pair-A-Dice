using UnityEngine;
using UnityEngine.Events; // ✅ Required for Unity Events

[RequireComponent(typeof(Rigidbody))]
public class ShakeToRoll : MonoBehaviour
{
    private Rigidbody rb;
    private Camera mainCamera;
    private float cameraZDistance;
    private Vector3 lastMousePosition;
    private float shakeIntensity = 0f;
    
    [Header("Shake Settings")]
    public float shakeThreshold = 8f;  
    public float shakeDecay = 2f;  
    public float forceMultiplier = 5f;  
    public float torqueMultiplier = 10f;  

    private bool isShaking = false;
    private static ShakeToRoll[] allDice; 

    [Header("Events")]
    public UnityEvent onDiceRolled; // ✅ Unity Event to trigger when dice roll

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        cameraZDistance = mainCamera.WorldToScreenPoint(transform.position).z;

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
            shakeIntensity += mouseDelta.magnitude;
            shakeIntensity = Mathf.Clamp(shakeIntensity, 0, shakeThreshold * 2);

            lastMousePosition = Input.mousePosition;

            if (shakeIntensity >= shakeThreshold)
            {
                RollAllDice();
                isShaking = false;
                shakeIntensity = 0;
            }
        }
        else
        {
            shakeIntensity = Mathf.Max(0, shakeIntensity - (shakeDecay * Time.deltaTime));
        }
    }

    private void OnMouseDown()
    {
        isShaking = true;
        lastMousePosition = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        isShaking = false;

    }

    public void AdjustShakeThreshold(float amount)
    {
        shakeThreshold += amount;
        shakeThreshold = Mathf.Max(1f, shakeThreshold);
    }

    private void RollAllDice()
    {
        foreach (ShakeToRoll die in allDice)
        {
            die.RollDice();
        }
        onDiceRolled.Invoke(); // ✅ Trigger event when dice roll
    }

    private void RollDice()
    {
        rb.linearVelocity = Vector3.zero;  
        rb.angularVelocity = Vector3.zero;

        Vector3 rollForce = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)) * forceMultiplier;
        Vector3 rollTorque = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)) * torqueMultiplier;

        rb.AddForce(rollForce, ForceMode.Impulse);
        rb.AddTorque(rollTorque, ForceMode.Impulse);
    }
}

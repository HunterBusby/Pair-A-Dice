using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class ShakeToRoll : MonoBehaviour
{
    private Rigidbody rb;
    private Camera mainCamera;
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
    public UnityEvent onDiceRolled;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

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

    // 🔹 Called externally from ShakeDiceManager
    public void StartShakingFromZone()
    {
        isShaking = true;
        lastMousePosition = Input.mousePosition;
    }

    public void StopShakingFromZone()
    {
        isShaking = false;
    }

    private void RollAllDice()
    {
        foreach (ShakeToRoll die in allDice)
        {
            die.RollDice();
        }

        onDiceRolled.Invoke();

        // Reset detectors and trigger result collection
        ShakeDiceManager manager = FindObjectOfType<ShakeDiceManager>();
        if (manager != null)
        {
            foreach (ShakeToRoll die in allDice)
            {
                var detector = die.GetComponent<DiceFaceDetector>();
                if (detector != null)
                    detector.hasStoppedRolling = false;
            }

            manager.StartCoroutine("WaitForDiceToStop");
        }
    }

    private void RollDice()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 rollForce = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)) * forceMultiplier;
        Vector3 rollTorque = new Vector3(
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f)
        ) * torqueMultiplier;

        rb.AddForce(rollForce, ForceMode.Impulse);
        rb.AddTorque(rollTorque, ForceMode.Impulse);
    }


    public void AdjustShakeThreshold(float amount)
{
    shakeThreshold += amount;
    shakeThreshold = Mathf.Max(1f, shakeThreshold); // Prevent it from going below 1
}


    // Optional if you still want manual shaking by clicking directly on dice
    private void OnMouseDown()
    {
        StartShakingFromZone();
    }

    private void OnMouseUp()
    {
        StopShakingFromZone();
    }
}

// ShakeToRoll.cs (only showing changes/additions)

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;   // 👈 add this

[RequireComponent(typeof(Rigidbody))]
public class ShakeToRoll : MonoBehaviour
{
    private Rigidbody rb;
    private Camera mainCamera;
    private Vector3 lastMousePosition;
    private float shakeIntensity = 0f;

    [Header("Shake Settings")]
    public float shakeThreshold = 100f;
    public float shakeDecay = 2f;
    public float forceMultiplier = 5f;
    public float torqueMultiplier = 10f;

    private bool isShaking = false;
    private static ShakeToRoll[] allDice;

    [Header("Events")]
    public UnityEvent onDiceRolled;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // When a new scene loads, clear the static cache so it gets rebuilt
        SceneManager.sceneLoaded -= OnSceneLoaded; // avoid dupes if re-enabled
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private static void RefreshAllDice()
    {
        allDice = FindObjectsByType<ShakeToRoll>(FindObjectsSortMode.None);
    }

    private static void EnsureDiceList()
    {
        if (allDice == null || allDice.Length == 0)
            RefreshAllDice();
    }

    private static void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        // Fresh scene = fresh dice set
        allDice = null;
    }

    void Start()
    {
        mainCamera = Camera.main;
        EnsureDiceList(); // ok to build now, and also rebuilt after scene loads
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
                RollAllDice();              // 👈 unchanged API
                isShaking = false;
                shakeIntensity = 0;
            }
        }
        else
        {
            shakeIntensity = Mathf.Max(0, shakeIntensity - (shakeDecay * Time.deltaTime));
        }
    }

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
        EnsureDiceList();

        bool sawNull = false;
        foreach (ShakeToRoll die in allDice)
        {
            if (!die) { sawNull = true; continue; } // destroyed component
            die.RollDiceSafe();
        }

        // If we saw any destroyed entries, rebuild for next time
        if (sawNull) RefreshAllDice();

        onDiceRolled.Invoke();

        // Reset detectors and trigger result collection
        ShakeDiceManager manager = FindObjectOfType<ShakeDiceManager>();
        if (manager != null)
        {
            foreach (ShakeToRoll die in allDice)
            {
                if (!die) continue;
                var detector = die.GetComponent<DiceFaceDetector>();
                if (detector != null) detector.hasStoppedRolling = false;
            }

            // keep your existing string-based StartCoroutine
            manager.StartWaitForDiceToStopOnce();

        }
    }

    // 🔐 Safe wrapper with guards; keeps Unity 6 linearVelocity
    private void RollDiceSafe()
    {
        if (!this) return; // component destroyed
        if (!rb) rb = GetComponent<Rigidbody>();
        if (!rb) return;

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

    public void AdjustShakeThresholdFromFloatData(FloatData data)
    {
        if (!data) return;
        shakeThreshold += data.value;
        shakeThreshold = Mathf.Max(1f, shakeThreshold);
    }

    public void SetShakeThresholdFromIntData(IntData data)
    {
        if (!data) return;
        shakeThreshold = Mathf.Max(1f, data.value);
    }




    private void OnMouseDown() { StartShakingFromZone(); }
    private void OnMouseUp()   { StopShakingFromZone();  }
}

using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class BurnAwayEffect : MonoBehaviour
{
    public Material burnMaterial;  // 🔹 Assign this in the Inspector
    public string cutoffProperty = "_CutoffHeight"; // 🔹 The shader property name
    public float burnSpeed = 0.5f; // 🔹 Adjust speed of burning
    public float maxCutoffHeight = 1.0f; // 🔹 Value at which the object disappears

    [Header("Burning Events")]
    public UnityEvent onBurnStart; // 🔹 Fires when burning begins
    public UnityEvent onBurnComplete; // 🔹 Fires when fully burned

    private bool isBurning = false;
    private float currentCutoffHeight = 0f;

    private void Start()
    {
        if (burnMaterial.HasProperty(cutoffProperty))
        {
            currentCutoffHeight = burnMaterial.GetFloat(cutoffProperty);
        }
    }

    public void StartBurning()
    {
        if (!isBurning)
        {
            isBurning = true;
            onBurnStart.Invoke(); // 🔥 Notify that burning has started
            StartCoroutine(BurnEffect());
        }
    }

    private IEnumerator BurnEffect()
    {
        while (currentCutoffHeight < maxCutoffHeight)
        {
            currentCutoffHeight += Time.deltaTime * burnSpeed;
            burnMaterial.SetFloat(cutoffProperty, currentCutoffHeight);
            yield return null;
        }

        // 🔹 Ensure it’s fully transparent before triggering event
        burnMaterial.SetFloat(cutoffProperty, maxCutoffHeight);
        onBurnComplete.Invoke(); // 🔥 Object is fully burned
        Destroy(gameObject); // 🔥 Remove object
    }
}

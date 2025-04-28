using UnityEngine;
using System.Collections;

public class BrandonMachine : MonoBehaviour
{
    [Header("Wheel Settings")]
    public Transform wheel1;
    public Transform wheel2;
    public float spinTime = 1.0f; // 🔥 How long the wheel takes to reach the target
    private const float degreesPerNumber = 60f;

    [Header("Dice References")]
    public DiceFaceDetector dice1;
    public DiceFaceDetector dice2;

    private int wheel1CurrentNumber = 1;
    private int wheel2CurrentNumber = 1;

    public void OnSingleDiceStopped(DiceFaceDetector dice)
    {
        if (dice == dice1)
        {
            StartCoroutine(SpinWheelLerp(wheel1, dice1.GetFaceUpValue(), 1));
        }
        else if (dice == dice2)
        {
            StartCoroutine(SpinWheelLerp(wheel2, dice2.GetFaceUpValue(), 2));
        }
    }

    private IEnumerator SpinWheelLerp(Transform wheel, int rolledNumber, int wheelID)
{
    int currentNumber = (wheelID == 1) ? wheel1CurrentNumber : wheel2CurrentNumber;

    float startAngle = -(currentNumber - 1) * degreesPerNumber;
    float targetAngle = -(rolledNumber - 1) * degreesPerNumber;

    float distance = Mathf.Abs(Mathf.DeltaAngle(startAngle, targetAngle));

    // 🔥 Calculate overshoot amount based on how far the wheel spins
    float minOvershoot = 2f;
    float maxOvershoot = 10f;
    float overshootAmount = Mathf.Lerp(minOvershoot, maxOvershoot, Mathf.InverseLerp(0f, 300f, distance));

    // 🔥 Create the overshoot target
    float overshootTarget = targetAngle + (targetAngle >= startAngle ? overshootAmount : -overshootAmount);

    float elapsed = 0f;
    float totalSpinTime = spinTime; // Main spin time

    // === First Phase: Spin to overshoot target ===
    while (elapsed < totalSpinTime)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / totalSpinTime);
        t = Mathf.SmoothStep(0f, 1f, t);

        float currentAngle = Mathf.Lerp(startAngle, overshootTarget, t);
        wheel.localRotation = Quaternion.Euler(currentAngle, 0f, 0f);

        yield return null;
    }

    // === Second Phase: Quick settle back to perfect target ===
    float settleTime = 0.15f;
    elapsed = 0f;
    while (elapsed < settleTime)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / settleTime);
        t = Mathf.SmoothStep(0f, 1f, t);

        float currentAngle = Mathf.Lerp(overshootTarget, targetAngle, t);
        wheel.localRotation = Quaternion.Euler(currentAngle, 0f, 0f);

        yield return null;
    }

    // Final snap to make sure
    wheel.localRotation = Quaternion.Euler(targetAngle, 0f, 0f);

    if (wheelID == 1)
        wheel1CurrentNumber = rolledNumber;
    else
        wheel2CurrentNumber = rolledNumber;
}


}
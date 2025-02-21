using System.Collections;
using UnityEngine;

public class DiceManagerAI : MonoBehaviour
{
    [Header("AI Dice Settings")]
    public DiceFaceDetector aiDice1;
    public DiceFaceDetector aiDice2;

    [Header("Roll Force Settings")]
    public float rollForce = 8f;  // ✅ Adjustable force applied to dice
    public float torqueForce = 10f;  // ✅ Adjustable spin force
    public float rollDelay = 1.5f;  // ✅ How long to wait for dice to settle

    [Header("AI Speed Settings")]
public float aiReactionTime = 2f; // ✅ Delay before AI rolls again

    private int lastDiceSum;

  void Start()
{
    Debug.Log("DiceManagerAI has started!"); // ✅ This confirms the script is active.
    StartCoroutine(AutoRollDice());
}


private IEnumerator AutoRollDice()
{
    Debug.Log("🔄 AutoRollDice() has started!"); // ✅ Confirms AI rolling is running
    while (true)
    {
        Debug.Log("🎲 AI is rolling dice...");
        RollBothDice();

        float elapsedTime = 0f;
        Debug.Log("⏳ Waiting for dice to stop...");

        while (!(aiDice1.hasStoppedRolling && aiDice2.hasStoppedRolling) && elapsedTime < 5f) // Max wait 5s
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!(aiDice1.hasStoppedRolling && aiDice2.hasStoppedRolling))
        {
            Debug.LogWarning("⚠ AI dice timeout reached! Forcing calculation.");
        }
        else
        {
            Debug.Log("✅ AI dice have stopped rolling!");
        }

        lastDiceSum = GetDiceSum();
        Debug.Log("🎯 AI Dice Roll Sum: " + lastDiceSum);

        yield return new WaitForSeconds(aiReactionTime); // ✅ AI wait time before rolling again
    }
}


    public void RollBothDice()
    {
        if (aiDice1 != null && aiDice2 != null)
        {
            Rigidbody rb1 = aiDice1.GetComponent<Rigidbody>();
            Rigidbody rb2 = aiDice2.GetComponent<Rigidbody>();

            if (rb1 != null && rb2 != null)
            {
                // Reset velocity before applying new forces
                rb1.linearVelocity = Vector3.zero;
                rb1.angularVelocity = Vector3.zero;
                rb2.linearVelocity = Vector3.zero;
                rb2.angularVelocity = Vector3.zero;

                // Randomized force directions
                Vector3 forceDirection1 = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)) * rollForce;
                Vector3 forceDirection2 = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)) * rollForce;

                // Randomized torque (spin)
                Vector3 torque1 = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)) * torqueForce;
                Vector3 torque2 = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)) * torqueForce;

                // Apply forces
                rb1.AddForce(forceDirection1, ForceMode.Impulse);
                rb1.AddTorque(torque1, ForceMode.Impulse);
                rb2.AddForce(forceDirection2, ForceMode.Impulse);
                rb2.AddTorque(torque2, ForceMode.Impulse);
            }
            else
            {
                Debug.LogError("One or both AI dice are missing a Rigidbody component!");
            }
        }
        else
        {
            Debug.LogError("AI Dice not assigned in DiceManagerAI.");
        }
    }
public int GetDiceSum()
{
    if (aiDice1 != null && aiDice2 != null)
    {
        int dice1Value = aiDice1.GetFaceUpValue();
        int dice2Value = aiDice2.GetFaceUpValue();
        int sum = dice1Value + dice2Value;

        Debug.Log("📝 AI Dice Face Values: " + dice1Value + " + " + dice2Value + " = " + sum);
        return sum;
    }
    Debug.LogError("⚠ AI Dice not assigned properly!");
    return 0;
}


    public int GetLastDiceSum()
    {
        return lastDiceSum;
    }
}
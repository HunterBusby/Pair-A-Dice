using System.Collections;
using UnityEngine;
using UnityEngine.Events; // ✅ Required for Unity Events

public class DiceManagerAI : MonoBehaviour
{
    [Header("AI Dice Settings")]
    public DiceFaceDetector aiDice1;
    public DiceFaceDetector aiDice2;

    [Header("Roll Force Settings")]
    public float rollForce = 8f;
    public float torqueForce = 10f;
    public float rollDelay = 1.5f;

    private int lastDiceSum;
    public bool isRolling = false; // ✅ Track if AI is currently rolling dice

    [Header("Doubles Event")]
    public UnityEvent onAIDoublesRolled; // ✅ Event triggered when AI rolls doubles

    void Start()
    {
        Debug.Log("🎲 DiceManagerAI Started!");
        StartCoroutine(AutoRollDice());
    }

    private IEnumerator AutoRollDice()
    {
        while (true)
        {
            isRolling = true;
            Debug.Log("🔄 AI Rolling Dice...");
            RollBothDice();

            yield return new WaitUntil(() => aiDice1.hasStoppedRolling && aiDice2.hasStoppedRolling);

            lastDiceSum = GetDiceSum();
            Debug.Log("🎯 AI Dice Roll Sum: " + lastDiceSum);

            // ✅ Check if AI rolled doubles
            if (DidRollDoubles())
            {
                Debug.Log("🎉 AI ROLLED DOUBLES!");
                onAIDoublesRolled.Invoke(); // ✅ Triggers event if doubles occur
            }

            isRolling = false;
            yield return null;
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
                rb1.linearVelocity = Vector3.zero;
                rb1.angularVelocity = Vector3.zero;
                rb2.linearVelocity = Vector3.zero;
                rb2.angularVelocity = Vector3.zero;

                Vector3 forceDirection1 = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)) * rollForce;
                Vector3 forceDirection2 = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)) * rollForce;

                Vector3 torque1 = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)) * torqueForce;
                Vector3 torque2 = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)) * torqueForce;

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

    private bool DidRollDoubles()
    {
        if (aiDice1 != null && aiDice2 != null)
        {
            return aiDice1.GetFaceUpValue() == aiDice2.GetFaceUpValue();
        }
        return false;
    }
}

using System.Collections;
using UnityEngine;

public class AIOpponent : MonoBehaviour
{
    [Header("AI Settings")]
    public float rollInterval = 3.0f; // Time between AI rolls
    [Range(0f, 1f)] public float mistakeChance = 0.2f; // AI error rate (0 = perfect, 1 = always fails)
    public float moveSpeed = 2.0f; // Delay before activating a match

    [Header("References")]
    public DiceFaceDetector aiDice1; // First AI dice
    public DiceFaceDetector aiDice2; // Second AI dice
    public CardManager cardManager; // Reference to card management system

    private int lastDiceSum;

    void Start()
    {
        StartCoroutine(AIPlayLoop());
    }

    private IEnumerator AIPlayLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(rollInterval); // Wait before rolling again

            RollBothDice(); // AI rolls dice
            yield return new WaitUntil(() => aiDice1.hasStoppedRolling && aiDice2.hasStoppedRolling); // Wait for dice to stop

            lastDiceSum = GetDiceSum();
            Debug.Log("🎯 AI Dice Roll Sum: " + lastDiceSum);

            if (ShouldMakeMistake())
            {
                Debug.Log("AI made a mistake and ignored a match.");
                continue; // Skip this turn due to mistake
            }

            AttemptMatch(); // Try to find and play a matching card
        }
    }

    private void RollBothDice()
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
        Vector3 forceDirection1 = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)) * 8f;
        Vector3 forceDirection2 = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)) * 8f;

        // Randomized torque (spin)
        Vector3 torque1 = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)) * 10f;
        Vector3 torque2 = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)) * 10f;

        // Apply forces
        rb1.AddForce(forceDirection1, ForceMode.Impulse);
        rb1.AddTorque(torque1, ForceMode.Impulse);
        rb2.AddForce(forceDirection2, ForceMode.Impulse);
        rb2.AddTorque(torque2, ForceMode.Impulse);

        Debug.Log("🎲 AI rolled the dice!");
    }
    else
    {
        Debug.LogError("One or both AI dice are missing a Rigidbody component!");
    }
}


    private int GetDiceSum()
    {
        int dice1Value = aiDice1.GetFaceUpValue();
        int dice2Value = aiDice2.GetFaceUpValue();
        int sum = dice1Value + dice2Value;

        Debug.Log("📝 AI Dice Face Values: " + dice1Value + " + " + dice2Value + " = " + sum);
        return sum;
    }

    private void AttemptMatch()
    {
        MatchBehaviour matchingCard = FindCardByValue(lastDiceSum);
        if (matchingCard != null)
        {
            StartCoroutine(ActivateCard(matchingCard));
        }
        else
        {
            Debug.Log("AI did NOT find a matching card for value: " + lastDiceSum);
        }
    }

    private IEnumerator ActivateCard(MatchBehaviour card)
    {
        yield return new WaitForSeconds(moveSpeed); // AI delay before playing move

        card.matchEvent.Invoke(); // ✅ Trigger same event as player
        yield return new WaitForSeconds(0.1f); // ✅ Allow Unity event to process

        // ✅ Manually move the card in case matchEvent fails
        Debug.Log("AI manually calling TransferCard for: " + card.idObj.name);
        cardManager.TransferCard(card.transform, false); // ✅ Move to player's side
    }

    private bool ShouldMakeMistake()
    {
        return Random.value < mistakeChance; // Roll a random value, compare with mistake chance
    }

    private MatchBehaviour FindCardByValue(int value)
    {
        foreach (Transform card in cardManager.enemyCards) // ✅ AI only searches its side
        {
            MatchBehaviour matchBehaviour = card.GetComponent<MatchBehaviour>();
            IDContainerBehaviour idContainer = card.GetComponent<IDContainerBehaviour>();

            if (matchBehaviour != null && idContainer != null && idContainer.idObj != null)
            {
                if (int.TryParse(idContainer.idObj.name.Replace("ID_", ""), out int cardID))
                {
                    if (cardID == value) // ✅ AI correctly identifies card using ID name
                    {
                        return matchBehaviour; // ✅ Return MatchBehaviour
                    }
                }
            }
        }
        return null; // No match found
    }
}

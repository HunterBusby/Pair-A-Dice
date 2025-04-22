using System.Collections;
using UnityEngine;

public class AIOpponent : MonoBehaviour
{
    [Header("AI Settings")]
    public float rollInterval = 3.0f; // Time between AI rolls
    [Range(0f, 1f)] public float mistakeChance = 0.2f; // AI error rate

    [Header("References")]
    public DiceFaceDetector aiDice1; // First AI die
    public DiceFaceDetector aiDice2; // Second AI die
    public CardManager cardManager;  // Manages cards in the scene

    private int lastDiceSum;

    void Start()
    {
        StartCoroutine(AIPlayLoop());
    }

    private IEnumerator AIPlayLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(rollInterval);

            RollBothDice();
            yield return new WaitUntil(() => aiDice1.hasStoppedRolling && aiDice2.hasStoppedRolling);

            lastDiceSum = GetDiceSum();
            Debug.Log("🎯 AI Dice Roll Sum: " + lastDiceSum);

            if (ShouldMakeMistake())
            {
                Debug.Log("😵 AI made a mistake and ignored a match.");
                continue;
            }

            AttemptMatch();
        }
    }

    private void RollBothDice()
    {
        Rigidbody rb1 = aiDice1.GetComponent<Rigidbody>();
        Rigidbody rb2 = aiDice2.GetComponent<Rigidbody>();

        if (rb1 != null && rb2 != null)
        {
            rb1.linearVelocity = rb2.linearVelocity = Vector3.zero;
            rb1.angularVelocity = rb2.angularVelocity = Vector3.zero;

            rb1.AddForce(RandomDirection() * 8f, ForceMode.Impulse);
            rb1.AddTorque(RandomTorque(), ForceMode.Impulse);

            rb2.AddForce(RandomDirection() * 8f, ForceMode.Impulse);
            rb2.AddTorque(RandomTorque(), ForceMode.Impulse);

            Debug.Log("🎲 AI rolled the dice!");
        }
        else
        {
            Debug.LogError("❌ One or both AI dice are missing Rigidbody components.");
        }
    }

    private Vector3 RandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f));
    }

    private Vector3 RandomTorque()
    {
        return new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)) * 10f;
    }

    private int GetDiceSum()
    {
        int v1 = aiDice1.GetFaceUpValue();
        int v2 = aiDice2.GetFaceUpValue();
        int sum = v1 + v2;

        Debug.Log($"📝 AI Dice Values: {v1} + {v2} = {sum}");
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
            Debug.Log($"❌ No matching card found for value {lastDiceSum}");
        }
    }

    private IEnumerator ActivateCard(MatchBehaviour card)
{
    // 🔔 Fire visual responders (robot arm, etc.)
    AIOpponentEvents.OnCardMatched?.Invoke(card.transform);

    // ⏱ Wait before letting the card animate
    yield return new WaitForSeconds(0.35f); // <- tweak this

    // ✅ Trigger animation & transfer
    card.matchEvent.Invoke();
    yield return new WaitForSeconds(0.1f);
    cardManager.TransferCard(card.transform, false);
}


    private bool ShouldMakeMistake()
    {
        return Random.value < mistakeChance;
    }

    private MatchBehaviour FindCardByValue(int value)
    {
        foreach (Transform card in cardManager.enemyCards)
        {
            MatchBehaviour mb = card.GetComponent<MatchBehaviour>();
            IDContainerBehaviour id = card.GetComponent<IDContainerBehaviour>();

            if (mb != null && id != null && id.idObj != null)
            {
                if (int.TryParse(id.idObj.name.Replace("ID_", ""), out int idValue) && idValue == value)
                {
                    return mb;
                }
            }
        }

        return null;
    }
}

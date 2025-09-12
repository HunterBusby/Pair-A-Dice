using System.Collections;
using UnityEngine;

public class AIOpponent : MonoBehaviour
{
    [Header("AI Settings")]
    public float rollInterval = 3.0f;         // Time between AI rolls
    [Range(0f, 1f)] public float mistakeChance = 0.2f; // AI error rate

    [Header("References")]
    public DiceFaceDetector aiDice1; // First AI die
    public DiceFaceDetector aiDice2; // Second AI die
    public CardManager cardManager;  // Manages cards in the scene

    private int lastDiceSum;

    // ---- Pause control (Approach #1) ----
    private bool isPaused = false;

    /// <summary>Call this when your tutorial double-roll line starts.</summary>
    public void PauseAI()
    {
        isPaused = true;
        StopDiceMotion(); // freeze dice if they were mid-roll
        // Debug.Log("[AI] Paused");
    }

    /// <summary>Call this when the line / ability finishes.</summary>
    public void ResumeAI()
    {
        isPaused = false;
        // Debug.Log("[AI] Resumed");
    }

    void Start()
    {
        StartCoroutine(AIPlayLoop());
    }

    private IEnumerator AIPlayLoop()
    {
        while (true)
        {
            // If paused, park the loop here until we resume.
            yield return new WaitUntil(() => !isPaused);

            // Wait for the next roll interval (unless paused during the wait)
            yield return new WaitForSeconds(rollInterval);
            if (isPaused) continue; // if paused during the interval, skip this cycle

            // Roll dice (guard again)
            if (isPaused) continue;
            RollBothDice();

            // Wait for dice to stop, but also bail out if we get paused mid-wait
            yield return new WaitUntil(() =>
                !isPaused && aiDice1.hasStoppedRolling && aiDice2.hasStoppedRolling);
            if (isPaused) continue;

            // Read results
            lastDiceSum = GetDiceSum();
            if (isPaused) continue;

            // Optional: mistake behavior
            if (ShouldMakeMistake())
            {
                // Debug.Log("😵 AI made a mistake and ignored a match.");
                continue;
            }

            // Attempt to match a card
            AttemptMatch();
        }
    }

    private void RollBothDice()
    {
        Rigidbody rb1 = aiDice1 ? aiDice1.GetComponent<Rigidbody>() : null;
        Rigidbody rb2 = aiDice2 ? aiDice2.GetComponent<Rigidbody>() : null;

        if (rb1 != null && rb2 != null)
        {
            // Zero current motion
            rb1.linearVelocity = Vector3.zero;
            rb2.linearVelocity = Vector3.zero;
            rb1.angularVelocity = Vector3.zero;
            rb2.angularVelocity = Vector3.zero;

            // Add randomized impulses/torques
            rb1.AddForce(RandomDirection() * 8f, ForceMode.Impulse);
            rb1.AddTorque(RandomTorque(), ForceMode.Impulse);

            rb2.AddForce(RandomDirection() * 8f, ForceMode.Impulse);
            rb2.AddTorque(RandomTorque(), ForceMode.Impulse);

            // Debug.Log("🎲 AI rolled the dice!");
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
        return new Vector3(
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f)
        ) * 10f;
    }

    private int GetDiceSum()
    {
        int v1 = aiDice1.GetFaceUpValue();
        int v2 = aiDice2.GetFaceUpValue();
        int sum = v1 + v2;

        // Debug.Log($"📝 AI Dice Values: {v1} + {v2} = {sum}");
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
            // Debug.Log($"❌ No matching card found for value {lastDiceSum}");
        }
    }

    private IEnumerator ActivateCard(MatchBehaviour card)
    {
        // 🔔 Fire visual responders (robot arm, etc.)
        AIOpponentEvents.OnCardMatched?.Invoke(card.transform);

        // ⏱ Small pre-animation delay
        yield return new WaitForSeconds(0.35f);
        if (isPaused) yield break; // if paused in the middle, don't continue

        // ✅ Trigger animation & transfer
        card.matchEvent?.Invoke();

        // Give a short moment for the event to propagate
        yield return new WaitForSeconds(0.1f);
        if (isPaused) yield break;

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
            if (!card) continue;

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

    /// <summary>
    /// Safety: immediately stop dice drift (called when pausing).
    /// </summary>
    private void StopDiceMotion()
    {
        if (aiDice1)
        {
            Rigidbody rb = aiDice1.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();
            }
        }
        if (aiDice2)
        {
            Rigidbody rb = aiDice2.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();
            }
        }
    }
}

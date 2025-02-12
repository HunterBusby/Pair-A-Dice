using System.Collections;
using UnityEngine;

public class AIOpponent : MonoBehaviour
{
    [Header("AI Settings")]
    public float moveSpeed = 2.0f; // Adjust AI move speed
    public float rollInterval = 3.0f; // Time between AI rolls
    [Range(0f, 1f)] public float mistakeChance = 0.2f; // 0 = perfect AI, 1 = always fails

    [Header("References")]
    public DiceManagerAI diceManagerAI; // Reference to AI's separate Dice Manager
    public CardManager cardManager; // Reference to the shared CardManager

    private int diceSum;
    private bool isRolling = false;

    void Start()
    {
        StartCoroutine(AIPlayLoop());
    }

    private IEnumerator AIPlayLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(rollInterval); // Wait before rolling again

            RollAIDice(); // AI rolls dice
            yield return new WaitUntil(() => !isRolling); // Wait for dice to stop rolling

            if (ShouldMakeMistake())
            {
                Debug.Log("AI made a mistake and ignored a match.");
                continue; // Skip this turn due to a mistake
            }

            AttemptMatch(); // Try to find and play a matching card
        }
    }

    private void RollAIDice()
    {
        if (diceManagerAI == null)
        {
            Debug.LogError("DiceManagerAI not assigned to AIOpponent.");
            return;
        }

        isRolling = true;
        diceManagerAI.RollBothDice(); // AI rolls dice
        StartCoroutine(WaitForDiceStop());
    }

    private IEnumerator WaitForDiceStop()
    {
        yield return new WaitForSeconds(1.5f); // Adjusted wait time for rolling to finish
        diceSum = diceManagerAI.GetLastDiceSum(); // Get the sum of AI's dice
        isRolling = false;
    }

  private void AttemptMatch()
{
    if (cardManager == null)
    {
        Debug.LogError("CardManager not assigned to AIOpponent.");
        return;
    }

    MatchBehaviour matchingCard = FindCardByValue(diceSum);
    if (matchingCard != null)
    {
        Debug.Log("AI found a matching card: " + matchingCard.idObj.name);
        StartCoroutine(ActivateCard(matchingCard)); // ✅ Ensure this calls ActivateCard(MatchBehaviour)
    }
    else
    {
        Debug.Log("AI did NOT find a matching card for value: " + diceSum);
    }
}


   private IEnumerator ActivateCard(MatchBehaviour card)
{
    yield return new WaitForSeconds(moveSpeed); // ✅ AI delay before playing move
    card.matchEvent.Invoke(); // ✅ Trigger the same event as when the player clicks the card
    Debug.Log("AI activated and transferred card: " + card.idObj.name);
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

using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class MatchBehaviour : MonoBehaviour
{
    public ID idObj;
    public UnityEvent matchEvent, noMatchEvent, noMatchDelayedEvent;
    
    private object diceManager; // ✅ Can hold either DiceManager or ShakeDiceManager
    private CardManager cardManager;

    void Start()
    {
        // ✅ Try to find a DiceManager first
        diceManager = FindFirstObjectByType<DiceManager>();
        
        // ✅ If no DiceManager exists, try to find ShakeDiceManager instead
        if (diceManager == null)
        {
            diceManager = FindFirstObjectByType<ShakeDiceManager>();
        }

        cardManager = FindFirstObjectByType<CardManager>();

        if (diceManager == null) Debug.LogError("❌ No valid Dice Manager found!");
        if (cardManager == null) Debug.LogError("❌ CardManager not found in scene!");
    }

 void OnMouseDown()
{
    if (idObj == null)
    {
        Debug.LogError("❌ idObj is NULL on " + gameObject.name + "!");
        return; // ✅ Prevents errors if idObj is missing
    }

    int latestRoll = GetLatestDiceSum(); // ✅ Get dice sum from either dice manager

    if (int.TryParse(idObj.name.Replace("ID_", ""), out int cardValue))
    {
        if (cardValue == latestRoll) // ✅ If card matches the dice roll
        {
            matchEvent.Invoke(); // ✅ Trigger the match event
            cardManager.TransferCard(transform, true); // ✅ Move card to enemy side

            Debug.Log(gameObject.name + " matched and transferred!");

            // ✅ Reset the dice sum to prevent multiple matches
            ResetDiceSum();
        }
        else 
        {
            Debug.Log(gameObject.name + " does NOT match!");
        }
    }
}

// ✅ Function to reset dice sum after a successful match
private void ResetDiceSum()
{
    if (diceManager is ShakeDiceManager sdm)
    {
        sdm.ResetDiceSum(); // ✅ Call a reset function inside ShakeDiceManager
    }
}


    private int GetLatestDiceSum()
    {
        if (diceManager is DiceManager dm) // ✅ If using normal DiceManager
        {
            return dm.GetLatestDiceSum();
        }
        else if (diceManager is ShakeDiceManager sdm) // ✅ If using ShakeDiceManager
        {
            return sdm.GetLatestDiceSum();
        }
        return 0; // Default to 0 if no dice manager is found
    }
}

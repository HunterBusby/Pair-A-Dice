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
            return; // ✅ Prevents the game from freezing
        }

        int latestRoll = GetLatestDiceSum(); // ✅ Get dice sum from either dice manager

        if (int.TryParse(idObj.name.Replace("ID_", ""), out int cardValue))
        {
            if (cardValue == latestRoll)
            {
                matchEvent.Invoke();
                cardManager.TransferCard(transform, true);
                Debug.Log(gameObject.name + " matched and transferred!");
            }
            else
            {
                Debug.Log(gameObject.name + " does NOT match!");
            }
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

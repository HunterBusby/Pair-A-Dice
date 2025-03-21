using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class MatchBehaviour : MonoBehaviour
{
    public ID idObj;
    public UnityEvent matchEvent, noMatchEvent, noMatchDelayedEvent;
    
    private object diceManager; // ✅ Can hold either DiceManager or ShakeDiceManager
    private CardManager cardManager;
    private IDContainerBehaviour idContainer; // ✅ Reference to the updated ID system

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
        idContainer = GetComponent<IDContainerBehaviour>(); // ✅ Get ID reference

        if (diceManager == null) Debug.LogError("❌ No valid Dice Manager found!");
        if (cardManager == null) Debug.LogError("❌ CardManager not found in scene!");
        if (idContainer == null) Debug.LogError("❌ IDContainerBehaviour missing on " + gameObject.name);
    }

    public bool isPendingMove = false; // ✅ Tracks if card should move after repositioning

    void OnMouseDown()
    {
        if (!cardManager.playerCards.Contains(transform))
        {
            Debug.Log("🚫 You cannot activate this card! It is not on your side.");
            return; // ✅ Prevents interaction if the card is NOT on the player's side
        }

        int latestRoll = GetLatestDiceSum(); // ✅ Get dice sum from either dice manager
        UpdateID(); // ✅ Ensure we check the latest ID before comparing

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

    public void UpdateID()
    {
        if (idContainer != null && idContainer.idObj != null)
        {
            idObj = idContainer.idObj; // ✅ Update the match behavior's ID
            Debug.Log($"🔄 {gameObject.name} MatchBehaviour ID updated to {idObj.name}");
        }
    }

    public void ExecuteCardTransfer()
    {
        isPendingMove = false; // ✅ Move is no longer pending
        matchEvent.Invoke();
        cardManager.TransferCard(transform, true);
        Debug.Log(gameObject.name + " matched and transferred!");

        // ✅ Reset the dice sum AFTER a successful move
        ShakeDiceManager shakeDiceManager = FindFirstObjectByType<ShakeDiceManager>();
        if (shakeDiceManager != null)
        {
            shakeDiceManager.ResetDiceSum();
            Debug.Log("🎲 Dice sum reset after successful match!");
        }
    }

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

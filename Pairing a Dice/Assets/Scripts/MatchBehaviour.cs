using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class MatchBehaviour : MonoBehaviour
{
    [Header("IDs & Events (existing)")]
    public ID idObj;
    public UnityEvent matchEvent, noMatchEvent, noMatchDelayedEvent;

    [Header("New: Player-only Match Event")]
    [Tooltip("Invoked when THIS card is successfully matched by the PLAYER.")]
    public UnityEvent onMatchByPlayer;

    // ✅ Can hold either DiceManager or ShakeDiceManager
    private object diceManager;
    private CardManager cardManager;
    private IDContainerBehaviour idContainer; // ✅ Reference to the updated ID system

    public bool isPendingMove = false; // ✅ Tracks if card should move after repositioning

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

    void OnMouseDown()
    {
        // ✅ Only allow player interaction with cards on the player's side
        if (!cardManager.playerCards.Contains(transform))
        {
            Debug.Log("🚫 You cannot activate this card! It is not on your side.");
            return;
        }

        int latestRoll = GetLatestDiceSum(); // ✅ Get dice sum from either dice manager
        UpdateID(); // ✅ Ensure we check the latest ID before comparing

        if (int.TryParse(idObj.name.Replace("ID_", ""), out int cardValue))
        {
            if (cardValue == latestRoll)
            {
                // ✅ Player successful match path
                matchEvent?.Invoke();

                // 🔔 Player-only match event
                onMatchByPlayer?.Invoke();

                cardManager.TransferCard(transform, true);
                Debug.Log(gameObject.name + " matched and transferred!");

                // ❌ DO NOT reset the dice sum here — we want multiple matches on one roll.
                // Reset should happen when a NEW roll/turn begins (inside DiceManager/ShakeDiceManager).
                // ResetDiceSum();
            }
            else
            {
                Debug.Log(gameObject.name + " does NOT match!");
                noMatchEvent?.Invoke();
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

    /// <summary>
    /// Call this from animations or delayed flows to complete a player-initiated match.
    /// </summary>
    public void ExecuteCardTransfer()
    {
        isPendingMove = false; // ✅ Move is no longer pending

        // Ensure we still treat this as a player match
        matchEvent?.Invoke();

        // 🔔 Player-only match event (covers animation/event-driven matches)
        onMatchByPlayer?.Invoke();

        cardManager.TransferCard(transform, true);
        Debug.Log(gameObject.name + " matched and transferred!");

        // ❌ DO NOT reset here either; keep the value for additional matches on the same roll.
        // If you later want to clear after *all* matches are done, trigger a reset from a Turn manager or DiceManager.
        // Example (elsewhere): ShakeDiceManager.ResetDiceSum() when Roll starts.
    }

    // Keep this utility if you need it elsewhere, but don't call it on each match.
    private void ResetDiceSum()
    {
        if (diceManager is ShakeDiceManager sdm)
        {
            sdm.ResetDiceSum(); // ✅ Provided for controlled use by your roll/turn system
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

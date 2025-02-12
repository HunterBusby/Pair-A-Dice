using UnityEngine;
using UnityEngine.Events;
using System.Collections; 

public class MatchBehaviour : MonoBehaviour
{
    public ID idObj;
    public UnityEvent matchEvent, noMatchEvent, noMatchDelayedEvent;
    private DiceManager diceManager;
    private CardManager cardManager;

    void Start()
    {
        diceManager = FindFirstObjectByType<DiceManager>(); // Get DiceManager
        cardManager = FindFirstObjectByType<CardManager>(); // Get CardManager
    }

   void OnMouseDown()
{
    int latestRoll = diceManager.GetLatestDiceSum();

    if (idObj != null && int.TryParse(idObj.name.Replace("ID_", ""), out int cardValue))
    {
        if (cardValue == latestRoll) // If the card matches the dice sum
        {
            Debug.Log(gameObject.name + " player clicked and triggered matchEvent.");
            matchEvent.Invoke(); // ✅ This is what AI also triggers
            cardManager.TransferCard(transform, true); // ✅ Move card to enemy side
        }
        else
        {
            Debug.Log(gameObject.name + " does NOT match!");
        }
    }
}


    private IEnumerator NoMatchDelay()
    {
        yield return new WaitForSeconds(0.5f);
        noMatchDelayedEvent.Invoke();
    }
}
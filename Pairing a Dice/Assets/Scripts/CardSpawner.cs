using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // ✅ Allows event-based triggering

public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab; 
    public Transform playerSide;
    public Transform enemySide;
    public List<int> predefinedPlayerCards;
    public List<int> predefinedEnemyCards;
    public bool useRandomCards = false;
    public int minCardValue = 2;
    public int maxCardValue = 12;
    public int numCardsPerSide = 4;

    private CardManager cardManager;

    public UnityEvent onCardsSpawned; // ✅ Event for when cards spawn

    void Start()
    {
        cardManager = FindFirstObjectByType<CardManager>(); // ✅ Find CardManager
        // 🚫 No automatic card spawning at start
    }

    // ✅ Call this function when the player clicks an item
    public void TriggerCardSpawn()
    {
        if (cardManager == null)
        {
            Debug.LogError("CardManager not found!");
            return;
        }

        SpawnCards(); // ✅ Spawn cards only when this function is called
        onCardsSpawned.Invoke(); // ✅ Trigger any additional events (e.g., animations)
    }

    private void SpawnCards()
    {
        List<int> playerCardsToSpawn = useRandomCards ? GenerateRandomCards() : predefinedPlayerCards;
        List<int> enemyCardsToSpawn = useRandomCards ? GenerateRandomCards() : predefinedEnemyCards;

        SpawnSideCards(playerCardsToSpawn, playerSide, true);
        SpawnSideCards(enemyCardsToSpawn, enemySide, false);
    }

    private void SpawnSideCards(List<int> cardValues, Transform side, bool isPlayer)
{
    List<Transform> spawnedCards = new List<Transform>();

    for (int i = 0; i < cardValues.Count; i++)
    {
        Vector3 cardPosition = cardManager.GetCardPosition(side, i);
        GameObject card = Instantiate(cardPrefab, cardPosition, Quaternion.identity);
        card.transform.SetParent(side, false);

        int cardValue = cardValues[i]; // ✅ Get the value for this card

        IDContainerBehaviour idContainer = card.GetComponent<IDContainerBehaviour>();
        CardBehaviour cardBehaviour = card.GetComponent<CardBehaviour>(); // ✅ Get the CardBehaviour
        if (idContainer != null)
        {
            idContainer.idObj = Resources.Load<ID>("CardNumberID/ID_" + cardValue);
        }

        if (cardBehaviour != null)
        {
            cardBehaviour.cardValue = cardValue; // ✅ Set the card's value to match the assigned ID
        }

        MatchBehaviour matchBehaviour = card.GetComponent<MatchBehaviour>();
        if (matchBehaviour != null && idContainer != null)
        {
            matchBehaviour.idObj = idContainer.idObj;
        }

        CardColorDebug colorDebug = card.GetComponent<CardColorDebug>();
        if (colorDebug != null && idContainer != null)
        {
            colorDebug.idObj = idContainer.idObj;
            colorDebug.ApplyMaterialBasedOnID();
        }

        if (isPlayer)
        {
            cardManager.playerCards.Add(card.transform);
        }
        else
        {
            cardManager.enemyCards.Add(card.transform);
        }

        spawnedCards.Add(card.transform);
    }

    if (isPlayer)
    {
        cardManager.RepositionCards(cardManager.playerCards, playerSide);
    }
    else
    {
        cardManager.RepositionCards(cardManager.enemyCards, enemySide);
    }
}


    private List<int> GenerateRandomCards()
    {
        List<int> randomCards = new List<int>();
        for (int i = 0; i < numCardsPerSide; i++)
        {
            int randomValue = Random.Range(minCardValue, maxCardValue + 1);
            randomCards.Add(randomValue);
        }
        return randomCards;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab; // Assign your card prefab in the Inspector
    public Transform playerSide;
    public Transform enemySide;
    public List<int> predefinedPlayerCards; // Set specific cards for the player
    public List<int> predefinedEnemyCards; // Set specific cards for the enemy
    public bool useRandomCards = false; // Toggle for random card generation
    public int minCardValue = 2; // Smallest card number (default 2)
    public int maxCardValue = 12; // Largest card number (default 12)
    public int numCardsPerSide = 4; // Number of cards each side should start with

    private CardManager cardManager;

    void Start()
    {
        cardManager = FindFirstObjectByType<CardManager>(); // Reference the CardManager
        SpawnCards();
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
    for (int i = 0; i < cardValues.Count; i++)
    {
        Vector3 cardPosition = cardManager.GetCardPosition(side, i);
        GameObject card = Instantiate(cardPrefab, cardPosition, Quaternion.identity);
        card.transform.SetParent(side, false); // ✅ Ensure correct parent assignment

        int cardValue = cardValues[i]; // ✅ Get predefined value

        // ✅ Assign ID using IDContainerBehaviour
        IDContainerBehaviour idContainer = card.GetComponent<IDContainerBehaviour>();
        if (idContainer != null)
        {
            idContainer.idObj = Resources.Load<ID>("CardNumberID/ID_" + cardValue); // ✅ Load correct ID
        }
        else
        {
            Debug.LogError("IDContainerBehaviour is missing on card prefab!");
        }

        // ✅ Assign ID in MatchBehaviour for interaction
        MatchBehaviour matchBehaviour = card.GetComponent<MatchBehaviour>();
        if (matchBehaviour != null && idContainer != null)
        {
            matchBehaviour.idObj = idContainer.idObj;
        }
        else
        {
            Debug.LogError("MatchBehaviour or IDContainerBehaviour missing on card!");
        }

        // ✅ Add the card to the CardManager tracking system
        if (isPlayer)
        {
            cardManager.playerCards.Add(card.transform);
        }
        else
        {
            cardManager.enemyCards.Add(card.transform);
        }
    }

    cardManager.RepositionCards(isPlayer ? cardManager.playerCards : cardManager.enemyCards, side);
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

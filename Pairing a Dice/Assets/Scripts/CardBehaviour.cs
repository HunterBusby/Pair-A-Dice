using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    public int cardValue; // The number on the card
    private CardManager cardManager;

  void Start()
{
    cardManager = FindFirstObjectByType<CardManager>(); // Updated method
}


    public void ActivateCard(bool toEnemy)
    {
        if (cardManager != null)
        {
            cardManager.TransferCard(transform, toEnemy);
        }
    }
}

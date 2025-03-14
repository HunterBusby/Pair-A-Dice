using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    public int cardValue; // ✅ Stores the card's numeric value
    private CardManager cardManager;

    void Start()
    {
        cardManager = FindFirstObjectByType<CardManager>(); 
    }

    public void ActivateCard(bool toEnemy)
    {
        if (cardManager != null)
        {
            cardManager.TransferCard(transform, toEnemy);
        }
    }

    public void ModifyCardValue(int amount)
    {
        int newValue = cardValue + amount;

        // ✅ Keep values within valid range (adjustable if needed)
        if (newValue < 1) newValue = 1; 
        if (newValue > 12) newValue = 12;

        cardValue = newValue;
        Debug.Log($"🔄 Card {gameObject.name} changed to {cardValue}!");

        // ✅ Update the displayed number on the card (if needed)
        UpdateCardVisual();
    }

    private void UpdateCardVisual()
    {
        TextMesh textMesh = GetComponentInChildren<TextMesh>(); // Assuming number is displayed with TextMesh
        if (textMesh != null)
        {
            textMesh.text = cardValue.ToString();
        }
    }
}

using UnityEngine;
using System.Collections.Generic; // Only needed if you use List<T>

public class GameRestartManager : MonoBehaviour
{
    public CardManager cardManager; // Assign this in the Inspector

    public void RestartGame()
    {
        // Make copies of the lists to avoid modification errors
        var playerCardsCopy = new List<Transform>(cardManager.playerCards);
        var enemyCardsCopy = new List<Transform>(cardManager.enemyCards);

        foreach (Transform card in playerCardsCopy)
        {
            if (card != null)
                Destroy(card.gameObject);
        }

        foreach (Transform card in enemyCardsCopy)
        {
            if (card != null)
                Destroy(card.gameObject);
        }

        // Clear the lists
        cardManager.playerCards.Clear();
        cardManager.enemyCards.Clear();

        Debug.Log("Game restarted. All cards removed.");
    }
}

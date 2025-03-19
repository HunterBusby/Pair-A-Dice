using UnityEngine;

public class DoubleArrowManager : MonoBehaviour
{
    public void ToggleArrows()
    {
        CardBehaviour[] cards = FindObjectsByType<CardBehaviour>(FindObjectsSortMode.None); // ✅ New method

        if (cards.Length == 0)
        {
            Debug.LogWarning("⚠️ No cards found! Make sure they exist in the scene.");
            return;
        }

        bool isActive = cards[0].GetArrowsActiveState(); // ✅ Check arrow state
        SetArrowsActive(!isActive);
    }

    public void HideArrows()
    {
        SetArrowsActive(false);
    }

    private void SetArrowsActive(bool state)
    {
        CardBehaviour[] cards = FindObjectsByType<CardBehaviour>(FindObjectsSortMode.None); // ✅ New method
        foreach (CardBehaviour card in cards)
        {
            card.SetArrowsActive(state); // ✅ Calls the method inside each card
        }
    }
}

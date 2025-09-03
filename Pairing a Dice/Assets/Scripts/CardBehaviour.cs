using UnityEngine;
using UnityEngine.Events;


public class CardBehaviour : MonoBehaviour
{
    public int cardValue; 
    public GameObject upArrow;
    public GameObject downArrow;
    private CardManager cardManager;
    
    private IDContainerBehaviour idContainer;  // ✅ Reference to ID system
    private CardColorDebug cardColorDebug;  // ✅ Reference to material system

    public UnityEvent onCardValueChanged; // ✅ Event for value changes (bb)

    void Start()
    {
        cardManager = FindFirstObjectByType<CardManager>(); 
        idContainer = GetComponent<IDContainerBehaviour>(); 
        cardColorDebug = GetComponent<CardColorDebug>(); 

        SetArrowsActive(false); // ✅ Ensure they start hidden
    }

public void ModifyCardValue(int amount)
{
    int newValue = cardValue + amount;

    // ✅ Keep values within valid range (adjustable if needed)
    if (newValue < 1) newValue = 1; 
    if (newValue > 12) newValue = 12;

    cardValue = newValue;
    Debug.Log($"🔄 Card {gameObject.name} changed to {cardValue}!");

    UpdateCardID();  // ✅ Update the ID system
    UpdateCardVisual(); // ✅ Update text on the card
    UpdateCardMaterial(); // ✅ Apply new material

    // ✅ Ensure MatchBehaviour also updates its ID
    MatchBehaviour matchBehaviour = GetComponent<MatchBehaviour>();
    if (matchBehaviour != null)
    {
        matchBehaviour.UpdateID();
    }
}



    private void UpdateCardID()
{
    if (idContainer != null)
    {
        string idPath = "CardNumberID/ID_" + cardValue;
        ID newID = Resources.Load<ID>(idPath);

        if (newID != null)
        {
            idContainer.idObj = newID; // ✅ Update the ID
            Debug.Log($"🆔 {gameObject.name} updated ID to: {idContainer.idObj.name}");

            onCardValueChanged?.Invoke(); // ✅ Trigger event for any listeners (bb)

            UpdateCardMaterial(); // ✅ Immediately update material
        }
        else
        {
            Debug.LogError($"❌ Could not find new ID at path: {idPath}");
        }
    }
}

    private void UpdateCardVisual()
    {
        TextMesh textMesh = GetComponentInChildren<TextMesh>(); 
        if (textMesh != null)
        {
            textMesh.text = cardValue.ToString();
        }
    }

  private void UpdateCardMaterial()
{
    if (cardColorDebug != null)
    {
        cardColorDebug.idObj = idContainer.idObj; // ✅ Ensure material updates with new ID
        cardColorDebug.ApplyMaterialBasedOnID(); // ✅ Force the material update
    }
}

    public void SetArrowsActive(bool state)
    {
        if (upArrow != null) upArrow.SetActive(state);
        if (downArrow != null) downArrow.SetActive(state);
    }

    public bool GetArrowsActiveState()
    {
        return (upArrow != null && upArrow.activeSelf) || (downArrow != null && downArrow.activeSelf);
    }
}

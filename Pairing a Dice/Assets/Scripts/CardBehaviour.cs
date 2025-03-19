using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    public int cardValue; 
    public GameObject upArrow;
    public GameObject downArrow;
    private CardManager cardManager;
    
    private IDContainerBehaviour idContainer;  // ✅ Reference to ID system
    private CardColorDebug cardColorDebug;  // ✅ Reference to material system

    void Start()
    {
        cardManager = FindFirstObjectByType<CardManager>(); 
        idContainer = GetComponent<IDContainerBehaviour>(); 
        cardColorDebug = GetComponent<CardColorDebug>(); 

        SetArrowsActive(false); // ✅ Ensure they start hidden
    }

    public void ModifyCardValue(int amount)
{
    Debug.Log($"🔄 Before Change: {gameObject.name} cardValue = {cardValue} (Adding {amount})");

    int newValue = cardValue + amount; // ✅ Should increase/decrease correctly

    // ✅ Keep values within valid range (adjustable if needed)
    if (newValue < 2) newValue = 2; 
    if (newValue > 12) newValue = 12;

    cardValue = newValue;
    Debug.Log($"✅ After Change: {gameObject.name} cardValue = {cardValue}");

    UpdateCardID();  // ✅ Update the ID system
    UpdateCardVisual(); // ✅ Update text on the card
    UpdateCardMaterial(); // ✅ Apply new material
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

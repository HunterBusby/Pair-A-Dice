using UnityEngine;

public class CardColorDebug : MonoBehaviour
{
    public Renderer cardRenderer; // Assign the card's Renderer in the Inspector
    public Color[] debugColors; // Assign colors in the Inspector (for sums 2-12)
    public ID idObj; // Reference to the card's ID

    void Start()
    {
        ApplyDebugColor();
    }

    void ApplyDebugColor()
{
    if (idObj == null)
    {
        Debug.LogError(gameObject.name + " is missing an ID!");
        return;
    }

    int idNumber;
    if (int.TryParse(idObj.name.Replace("ID_", ""), out idNumber))
    {
        if (idNumber >= 2 && idNumber <= 12 && debugColors.Length >= (idNumber - 2))
        {
            if (cardRenderer != null)
            {
                Color newColor = debugColors[idNumber - 2];
                newColor.a = 1f; // Ensure full opacity
                cardRenderer.material.color = newColor;

                Debug.Log(gameObject.name + " assigned debug color for ID_" + idNumber);
            }
        }
        else
        {
            Debug.LogError("Debug color array is missing an entry for ID_" + idNumber);
        }
    }
    else
    {
        Debug.LogError("Invalid ID name format on " + gameObject.name);
    }
}

}

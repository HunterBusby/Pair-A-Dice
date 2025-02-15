using UnityEngine;

public class CardColorDebug : MonoBehaviour
{
    public Renderer cardRenderer; // Assign the card's Renderer in the Inspector
    public ID idObj; // Reference to the card's ID

    public void ApplyMaterialBasedOnID() // ✅ Call this after ID is assigned
    {
        if (idObj == null)
        {
            Debug.LogError(gameObject.name + " is missing an ID!");
            return;
        }

        int idNumber;
        if (int.TryParse(idObj.name.Replace("ID_", ""), out idNumber))
        {
            string materialPath = "Materials/CardMaterials/ID_" + idNumber + "_Mat"; // ✅ Path to materials
            Material mat = Resources.Load<Material>(materialPath); // ✅ Load material

            if (mat != null && cardRenderer != null)
            {
                cardRenderer.material = mat; // ✅ Apply material
                Debug.Log(gameObject.name + " assigned material: " + mat.name);
            }
            else
            {
                Debug.LogError("Material not found for ID_" + idNumber);
            }
        }
        else
        {
            Debug.LogError("Invalid ID name format on " + gameObject.name);
        }
    }
}

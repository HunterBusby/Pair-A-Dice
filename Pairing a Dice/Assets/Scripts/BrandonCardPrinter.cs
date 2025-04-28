using UnityEngine;

public class BrandonCardPrinter : MonoBehaviour
{
    [Header("Card Material Settings")]
    public Renderer cardRenderer; // The MeshRenderer of your card

    private ShakeDiceManager shakeDiceManager;

    void Start()
    {
        shakeDiceManager = FindFirstObjectByType<ShakeDiceManager>();

        if (shakeDiceManager == null)
        {
            Debug.LogError("❌ No ShakeDiceManager found in scene!");
            return;
        }

        PrintCard(); // Try printing on Start (later we'll trigger automatically)
    }

    public void PrintCard()
    {
        if (shakeDiceManager == null)
        {
            Debug.LogError("❌ ShakeDiceManager reference missing!");
            return;
        }

        int sum = shakeDiceManager.GetLatestDiceSum();

        if (sum == 0)
        {
            Debug.LogWarning("❌ Sum is 0 or no dice rolled yet. Skipping print.");
            return;
        }

        Debug.Log("🖨️ Trying to print card for dice sum: " + sum);

        // 🔥 Build the material path based on the dice sum
        string materialPath = "Materials/CardMaterials/ID_" + sum + "_Mat"; // Match your folder structure

        Material mat = Resources.Load<Material>(materialPath);

        if (mat != null && cardRenderer != null)
        {
            cardRenderer.material = mat;
            Debug.Log("✅ Card material changed to: " + mat.name);
        }
        else
        {
            Debug.LogWarning("❌ Material not found for sum: " + sum + " at path: " + materialPath);
        }

        if (mat != null)
{
    Debug.Log("✅ Material FOUND: " + mat.name);
}
else
{
    Debug.LogWarning("❌ Material NOT FOUND at: " + materialPath);
}


        Debug.Log("Material path trying to load: " + materialPath);
    }


    

}

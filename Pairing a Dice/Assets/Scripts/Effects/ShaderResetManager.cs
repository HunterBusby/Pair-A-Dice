using UnityEngine;

public class ShaderResetManager : MonoBehaviour
{
    public Material[] materialsToReset;  // 🔹 Assign all burnable materials here
    public string cutoffProperty = "_CutoffHeight"; // 🔹 Shader property name
    public float defaultCutoffValue = -0.2f; // 🔹 Set this to your default value

    private void Start()
    {
        ResetAllMaterials();
    }

    private void ResetAllMaterials()
    {
        foreach (Material mat in materialsToReset)
        {
            if (mat != null)
            {
                mat.SetFloat(cutoffProperty, defaultCutoffValue);
                Debug.Log($"🔄 Reset {mat.name} CutoffHeight to {defaultCutoffValue}");
            }
        }
    }

    private void OnApplicationQuit()
    {
        ResetAllMaterials(); // ✅ Ensures reset happens when game closes
    }
}

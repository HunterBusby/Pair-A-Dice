using UnityEngine;

public class CardGlowOnHover : MonoBehaviour
{
    private Renderer rend;
    private Material mat;

    [Header("Glow Settings")]
    public Color glowColor = Color.cyan;
    public float idleGlowIntensity = 0f;
    public float hoverGlowIntensity = 5f;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        mat = rend.material;

        // Ensure Emission is active at all times
        mat.EnableKeyword("_EMISSION");
        SetGlow(idleGlowIntensity);
    }

    private void OnMouseEnter()
    {
        SetGlow(hoverGlowIntensity);
    }

    private void OnMouseExit()
    {
        SetGlow(idleGlowIntensity);
    }

    void SetGlow(float intensity)
    {
        Color finalColor = glowColor * intensity;
        mat.SetColor("_EmissionColor", finalColor);
    }
}

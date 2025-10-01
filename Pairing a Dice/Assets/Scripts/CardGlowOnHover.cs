using UnityEngine;

public class CardGlowOnHover : MonoBehaviour
{
    private Renderer rend;
    private Material mat;

    [Header("Glow Settings")]
    public Color glowColor = Color.cyan;
    public float idleGlowIntensity = 0f;
    public float hoverGlowIntensity = 5f;

    // Track the last-applied intensity so color changes keep the same brightness
    private float _currentGlowIntensity = 0f;

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
        _currentGlowIntensity = intensity;
        Color finalColor = glowColor * intensity;
        mat.SetColor("_EmissionColor", finalColor);
    }

    public void SetGlowExternally(bool isHovering)
    {
        SetGlow(isHovering ? hoverGlowIntensity : idleGlowIntensity);
    }
    
    public void rerunStart() // ✅ Public method to re-run Start logic (bb)
    {
        Start();
    }

    // ✅ NEW: change glowColor via hex string, then re-apply current intensity
    public void SetGlowColorFromHex(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex)) return;
        if (hex[0] != '#') hex = "#" + hex;

        Color parsed;
        if (ColorUtility.TryParseHtmlString(hex, out parsed))
        {
            glowColor = parsed;
            // Re-apply using the last intensity that was set
            SetGlow(_currentGlowIntensity);
        }
        else
        {
            Debug.LogWarning($"[CardGlowOnHover] Invalid hex '{hex}' on {name}.");
        }
    }
}

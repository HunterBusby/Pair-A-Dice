using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightColorCycler : MonoBehaviour
{
    public float cycleSpeed = 1.0f; // Speed of color change (higher = faster)

    private Light lightSource;
    private float hue;

    void Start()
    {
        lightSource = GetComponent<Light>();
        hue = 0f;
    }

    void Update()
    {
        // Increment hue value
        hue += Time.deltaTime * cycleSpeed;
        if (hue > 1f)
            hue -= 1f;

        // Convert hue to RGB color
        Color color = Color.HSVToRGB(hue, 1f, 1f);
        lightSource.color = color;
    }
}

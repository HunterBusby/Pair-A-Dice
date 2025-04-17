using UnityEngine;

public class BossMusicManager : MonoBehaviour
{
    public AudioSource baseLayer;
    public AudioSource[] additionalLayers;
    public int maxPlayerCards = 12;
    public float fadeSpeed = 1.5f;

    private int targetLayerCount = 0;

    void Start()
    {
        baseLayer.Play();
        foreach (var layer in additionalLayers)
        {
            layer.volume = 0f;
            layer.Play();
        }
    }

    public void OnCardCountChanged(int remainingCards)
    {
        float progress = 1f - Mathf.Clamp01((float)remainingCards / maxPlayerCards);
        targetLayerCount = Mathf.RoundToInt(progress * additionalLayers.Length);
        Debug.Log($"🎵 Music layer target set to {targetLayerCount} based on {remainingCards} cards.");
    }


public void StartMusic(int initialCardCount)
{
    Debug.Log("🎵 Starting Boss Music!");

    baseLayer.Play();

    foreach (var layer in additionalLayers)
    {
        layer.volume = 0f;
        layer.Play();
    }

    OnCardCountChanged(initialCardCount); // 🔁 Sync layers based on starting hand
}

    void Update()
    {
        // Smoothly adjust volumes toward target state
        for (int i = 0; i < additionalLayers.Length; i++)
        {
            float targetVolume = (i < targetLayerCount) ? 1f : 0f;
            additionalLayers[i].volume = Mathf.MoveTowards(additionalLayers[i].volume, targetVolume, fadeSpeed * Time.deltaTime);
        }
    }
}

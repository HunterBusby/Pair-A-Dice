using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DiceImpactSound : MonoBehaviour
{
    public string boardTag = "Board";
    public float impactThreshold = 1.5f;
    public float rollingThreshold = 0.2f; // smaller bumps while rolling
    public float cooldown = 0.2f; // time between sounds
    public AudioClip[] impactClips;
    public float minPitch = 0.85f;
    public float maxPitch = 1.2f;

    private AudioSource audioSource;
    private Rigidbody rb;
    private float lastSoundTime = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        TryPlayImpactSound(collision, impactThreshold);
    }

    void OnCollisionStay(Collision collision)
    {
        TryPlayImpactSound(collision, rollingThreshold);
    }

    void TryPlayImpactSound(Collision collision, float threshold)
    {
        if (!collision.collider.CompareTag(boardTag)) return;

        // Only trigger for strong enough movement
        float velocity = collision.relativeVelocity.magnitude;
        if (velocity < threshold) return;

        // Debounce: prevent constant sounds
        if (Time.time - lastSoundTime < cooldown) return;

        // Play the sound
        if (impactClips.Length > 0)
        {
            audioSource.clip = impactClips[Random.Range(0, impactClips.Length)];
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.Play();
            lastSoundTime = Time.time;

            Debug.Log($"🎲 Playing impact at velocity {velocity} (threshold: {threshold})");
        }
    }
}

using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    public Animator animator;        // Assign your Animator in the Inspector
    public string triggerName = "Play"; // Name of the trigger in the Animator

    // Call this method when you want to activate the animation
    public void TriggerAnimation()
    {
        if (animator != null && !string.IsNullOrEmpty(triggerName))
        {
            animator.SetTrigger(triggerName);
        }
        else
        {
            Debug.LogWarning("Animator or triggerName not set on " + gameObject.name);
        }
    }
}

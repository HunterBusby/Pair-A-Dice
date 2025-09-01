using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public string sceneName;

    public Animator transition;                    // Assign in Inspector
    public string fadeOutTrigger = "StartTransition";
    public float fadeOutDuration = 1.0f;           // seconds; match your CrossFade length

    void Awake()
    {
        // Keep the transition object alive between scenes (recommended if this GO holds the Animator)
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene()
    {
        StartCoroutine(LoadSceneRoutine());
    }

    private IEnumerator LoadSceneRoutine()
    {
        // If there's a valid Animator + Controller, play the OUT animation first
        if (transition != null && transition.runtimeAnimatorController != null)
        {
            // Optional: verify the trigger exists to avoid typos
            if (!HasParameter(transition, fadeOutTrigger, AnimatorControllerParameterType.Trigger))
            {
                Debug.LogWarning($"Trigger '{fadeOutTrigger}' not found on Animator '{transition.name}'. " +
                                 "Continuing without fade.");
            }
            else
            {
                transition.ResetTrigger(fadeOutTrigger);
                transition.SetTrigger(fadeOutTrigger);
                yield return new WaitForSecondsRealtime(fadeOutDuration); // wait until screen is covered
            }
        }
        else
        {
            Debug.LogWarning("Transition Animator missing or has no Controller. Loading scene without fade.");
        }

        // Load AFTER screen is covered
        var op = SceneManager.LoadSceneAsync(sceneName);
        // Optionally wait for completion:
        // while (!op.isDone) yield return null;
    }

    private bool HasParameter(Animator anim, string paramName, AnimatorControllerParameterType type)
    {
        foreach (var p in anim.parameters)
        {
            if (p.type == type && p.name == paramName) return true;
        }
        return false;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour {
    [Tooltip("Set the dialogue IDs to be played in order.")]
    public List<string> dialogueIDs;

    [Tooltip("This event will fire after the dialogue sequence finishes.")]
    public UnityEvent onDialogueFinished;

    public void PlayDialogue() {
        if (dialogueIDs != null && dialogueIDs.Count > 0) {
            DialogueManager.Instance.OnDialogueComplete.AddListener(HandleDialogueFinished);
            DialogueManager.Instance.ShowDialogueSequence(dialogueIDs);
        } else {
            Debug.LogWarning("No dialogue IDs assigned to DialogueTrigger on " + gameObject.name);
        }
    }

    private void HandleDialogueFinished() {
        onDialogueFinished?.Invoke();
        DialogueManager.Instance.OnDialogueComplete.RemoveListener(HandleDialogueFinished); // Clean up
    }
}

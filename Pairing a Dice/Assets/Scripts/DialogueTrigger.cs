using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    [Tooltip("Set the dialogue IDs to be played in order.")]
    public List<string> dialogueIDs;

    public void PlayDialogue() {
        if (dialogueIDs != null && dialogueIDs.Count > 0) {
            DialogueManager.Instance.ShowDialogueSequence(dialogueIDs);
        } else {
            Debug.LogWarning("No dialogue IDs assigned to DialogueTrigger on " + gameObject.name);
        }
    }
}

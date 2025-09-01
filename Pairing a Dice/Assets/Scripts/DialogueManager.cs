using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour {
    public static DialogueManager Instance;

    [Header("UI Elements")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public Button nextButton;

    [Header("Typewriter Settings")]
    public float typingSpeed = 0.02f;

    [Header("Events")]
    public UnityEvent OnDialogueComplete;  // 🔹 Add this

    private Queue<string> dialogueQueue = new Queue<string>();
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentLine = "";

    void Awake() {
        Instance = this;
        if (OnDialogueComplete == null) OnDialogueComplete = new UnityEvent(); // 🔹 Initialize if not set in Inspector
        dialogueBox.SetActive(false);
        nextButton.onClick.AddListener(OnNextClicked);
    }

    // Load a whole dialogue sequence
    public void ShowDialogueSequence(List<string> ids) {
        dialogueQueue.Clear();
        foreach (string id in ids) {
            dialogueQueue.Enqueue(LocalizationManager.Instance.GetText(id));
        }

        dialogueBox.SetActive(true);
        DisplayNextLine();
    }

    private void DisplayNextLine() {
        if (dialogueQueue.Count == 0) {
            EndDialogue();
            return;
        }

        currentLine = dialogueQueue.Dequeue();

        if (typingCoroutine != null) {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeLine(currentLine));
    }

    IEnumerator TypeLine(string line) {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line.ToCharArray()) {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    public void OnNextClicked() {
        if (isTyping) {
            // Instantly finish the line
            if (typingCoroutine != null) {
                StopCoroutine(typingCoroutine);
            }
            dialogueText.text = currentLine;
            isTyping = false;
            typingCoroutine = null;
        } else {
            // Proceed to next line
            DisplayNextLine();
        }
    }

    private void EndDialogue() {
        dialogueBox.SetActive(false);
        dialogueText.text = "";

        // 🔹 Trigger the event to notify other scripts
        OnDialogueComplete.Invoke();
    }
}

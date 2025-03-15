using UnityEngine;
using UnityEngine.Events; // Required for UnityEvent

public class PauseManager : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    public GameObject exitConfirmationUI; // Assign in Inspector
    public GameObject settingsMenuUI; // Assign in Inspector

    // Public UnityEvent to trigger when pausing/unpausing
    public UnityEvent<bool> OnPauseStateON;
    public UnityEvent<bool> OnPauseStateOFF;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (exitConfirmationUI.activeSelf) 
            {
                // Close exit confirmation instead of resuming
                exitConfirmationUI.SetActive(false);
                pauseMenuUI.SetActive(true); // Bring back main pause menu
            }
            else if (settingsMenuUI.activeSelf)
            {
                // Close settings menu instead of resuming
                settingsMenuUI.SetActive(false);
                pauseMenuUI.SetActive(true);
            }
            else if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
        
        Debug.Log("Game Paused - Event Triggered");
        OnPauseStateON.Invoke(true); // Trigger event for pause
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        exitConfirmationUI.SetActive(false); // Ensure exit menu closes
        settingsMenuUI.SetActive(false); // Ensure settings menu closes
        
        Debug.Log("Game Resumed - Event Triggered");
        OnPauseStateOFF.Invoke(true); // Trigger event for resume
    }
    public void ExitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}

using UnityEngine;
using UnityEngine.Events; // Required for UnityEvent

public class PauseManager : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuUI;

    // Public UnityEvent to trigger when pausing/unpausing
    public UnityEvent<bool> OnPauseStateON;
    public UnityEvent<bool> OnPauseStateOFF;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
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
        
        Debug.Log("Game Resumed - Event Triggered");
        OnPauseStateOFF.Invoke(true); // Trigger event for resume
    }
public void ExitGame()
{
    Debug.Log("Quitting Game..."); // Logs when quitting (for testing)
    Application.Quit(); // Closes the game

    // If testing in the Unity Editor:
    #if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
    #endif
}


    
}

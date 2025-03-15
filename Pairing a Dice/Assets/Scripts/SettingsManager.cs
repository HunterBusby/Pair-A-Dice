using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public Slider fpsSlider; // Assign in Inspector
    public TMP_Text fpsText; // Assign in Inspector
    private int pendingFPS; // Stores value before applying

    // Define snap points (Only values within 30-144)
    private int[] snapPoints = { 30, 60, 120, 144 };

    private void Start()
    {
        // Load saved FPS setting, default to 60
        int savedFPS = PlayerPrefs.GetInt("FPSLimit", 60);
        Application.targetFrameRate = savedFPS;

        // Set slider range
        if (fpsSlider != null)
        {
            fpsSlider.minValue = 30;
            fpsSlider.maxValue = 144; // Changed max value
            fpsSlider.wholeNumbers = true; // Only whole numbers
            fpsSlider.value = savedFPS;
            pendingFPS = savedFPS;
            fpsSlider.onValueChanged.AddListener(UpdatePendingFPS);
        }

        UpdateFPSText(savedFPS);
    }

    // Updates the pending FPS value before applying
    public void UpdatePendingFPS(float value)
    {
        // Round to nearest valid FPS
        pendingFPS = Mathf.RoundToInt(value);

        // Snap to predefined FPS values when close
        pendingFPS = GetClosestSnapValue(pendingFPS, snapPoints, 10);

        // Update slider position visually
        fpsSlider.value = pendingFPS;

        UpdateFPSText(pendingFPS);
    }

    // Called when "Apply" is clicked
    public void ApplySettings()
    {
        Application.targetFrameRate = pendingFPS;
        PlayerPrefs.SetInt("FPSLimit", pendingFPS);
        PlayerPrefs.Save();

        Debug.Log("Applied FPS Cap: " + pendingFPS);
    }

    private void UpdateFPSText(int fps)
    {
        if (fpsText != null)
        {
            fpsText.text = "FPS: " + fps.ToString();
        }
    }

    // Helper function to snap values
    private int GetClosestSnapValue(int value, int[] snapPoints, int snapRange)
    {
        foreach (int snap in snapPoints)
        {
            if (Mathf.Abs(value - snap) <= snapRange)
            {
                return snap; // Snap to the closest predefined value
            }
        }
        return value; // Otherwise, use the normal value
    }
}

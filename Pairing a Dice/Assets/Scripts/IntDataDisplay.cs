using UnityEngine;
using TMPro;   // 👈 if you’re using TextMeshPro

public class IntDataDisplay : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text uiText;   // assign your TextMeshPro component in Inspector

    // --- IntData ---
    public void UpdateFromIntData(IntData data)
    {
        if (uiText != null && data != null)
        {
            uiText.text = data.value.ToString();
        }
    }

    public void UpdateFromInt(int value)
    {
        if (uiText != null)
        {
            uiText.text = value.ToString();
        }
    }

    // --- FloatData ---
    public void UpdateFromFloatData(FloatData data)
    {
        if (uiText != null && data != null)
        {
            uiText.text = data.value.ToString("F2"); // F2 = 2 decimal places
        }
    }

    public void UpdateFromFloat(float value)
    {
        if (uiText != null)
        {
            uiText.text = value.ToString("F2");
        }
    }
}
using UnityEngine;

public class DataSaver : MonoBehaviour
{
    [Header("Tracked Data")]
    public IntData[] ints;
    public FloatData[] floats;

    [Header("Behavior")]
    public bool autoLoadOnStart = true;
    public bool autoSaveOnQuit = true;
    public bool autoSaveOnPause = true;

    private void Start()
    {
        if (autoLoadOnStart) LoadAll();
    }

    private void OnApplicationQuit()
    {
        if (autoSaveOnQuit) SaveAll();
    }

    private void OnApplicationPause(bool paused)
    {
        if (autoSaveOnPause && paused) SaveAll();
    }

    // ---- UnityEvent-friendly methods ----
    public void SaveAll()
    {
        if (ints != null)   foreach (var d in ints)   if (d) d.SaveNow();
        if (floats != null) foreach (var d in floats) if (d) d.SaveNow();
        Debug.Log("[DataSaver] Saved all tracked data.");
    }

    public void LoadAll()
    {
        if (ints != null)   foreach (var d in ints)   if (d) d.LoadNow();
        if (floats != null) foreach (var d in floats) if (d) d.LoadNow();
        Debug.Log("[DataSaver] Loaded all tracked data.");
    }

    public void ResetAllToDefaults()
    {
        if (ints != null)   foreach (var d in ints)   if (d) d.ResetToDefault();
        if (floats != null) foreach (var d in floats) if (d) d.ResetToDefault();
        Debug.Log("[DataSaver] Reset all to defaults (and saved if autoSave=true on each SO).");
    }

    public void DeleteAllSaved()
    {
        if (ints != null)   foreach (var d in ints)   if (d) d.DeleteSaved();
        if (floats != null) foreach (var d in floats) if (d) d.DeleteSaved();
        Debug.Log("[DataSaver] Deleted all saved keys.");
    }
}

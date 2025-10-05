using UnityEngine;

public static class ActiveStateStore
{
    private static string Key(string id) => $"obj:{id}:active";

    public static bool TryGetSaved(string id, out bool isActive)
    {
        if (string.IsNullOrEmpty(id)) { isActive = true; return false; }
        if (!PlayerPrefs.HasKey(Key(id))) { isActive = true; return false; }
        isActive = PlayerPrefs.GetInt(Key(id), 1) == 1;
        return true;
    }

    public static void Save(string id, bool isActive)
    {
        if (string.IsNullOrEmpty(id)) return;
        PlayerPrefs.SetInt(Key(id), isActive ? 1 : 0);
        PlayerPrefs.Save();
    }
}

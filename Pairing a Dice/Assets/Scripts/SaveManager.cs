using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Assign your SaveRegistry asset")]
    public SaveRegistry registry;

    [Header("Save Settings")]
    public string gameId = "Pair-a-Dice";
    public int saveVersion = 1;     // bump if your schema changes
    public int defaultSlot = 0;     // 0,1,2... user-selectable if you like

    [Serializable]
    private class SavePayload
    {
        public string gameId;
        public int version;
        public string savedAtIsoUtc;
        public Dictionary<string, int> ints = new();
        public Dictionary<string, float> floats = new();
    }

    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ---------- Public API ----------
    public void NewGame(int slot)
    {
        // Reset all to defaults, then immediately save.
        ResetAllToDefaults();
        Save(slot);
    }

    public void Save(int slot) => Write(ToPayload(), GetPath(slot));

    public bool Load(int slot)
    {
        string path = GetPath(slot);
        if (!File.Exists(path)) return false;

        var payload = Read(path);
        if (payload == null) return false;

        if (payload.gameId != gameId)
        {
            Debug.LogWarning($"Save file gameId mismatch. Expected {gameId}, got {payload.gameId}.");
            return false;
        }

        // If versions differ, you could add migration here.
        Apply(payload);
        return true;
    }

    public bool HasSave(int slot) => File.Exists(GetPath(slot));

    public void DeleteSave(int slot)
    {
        string path = GetPath(slot);
        if (File.Exists(path)) File.Delete(path);
    }

    // ---------- Internals ----------
    private string GetPath(int slot)
    {
        string file = $"save_slot_{slot}.json";
        return Path.Combine(Application.persistentDataPath, file);
    }

    private SavePayload ToPayload()
    {
        var p = new SavePayload
        {
            gameId = gameId,
            version = saveVersion,
            savedAtIsoUtc = DateTime.UtcNow.ToString("o")
        };

        if (registry)
        {
            if (registry.ints != null)
                foreach (var d in registry.ints)
                    if (d) p.ints[KeyOf(d.saveKey, d.name)] = d.value;

            if (registry.floats != null)
                foreach (var d in registry.floats)
                    if (d) p.floats[KeyOf(d.saveKey, d.name)] = d.value;
        }
        return p;
    }

    private void Apply(SavePayload p)
    {
        if (registry)
        {
            if (registry.ints != null)
                foreach (var d in registry.ints)
                {
                    if (!d) continue;
                    string key = KeyOf(d.saveKey, d.name);
                    if (p.ints.TryGetValue(key, out int v))
                        d.SetValue(v);            // fires events
                    else
                        d.ResetToDefault();       // missing -> default
                }

            if (registry.floats != null)
                foreach (var d in registry.floats)
                {
                    if (!d) continue;
                    string key = KeyOf(d.saveKey, d.name);
                    if (p.floats.TryGetValue(key, out float v))
                        d.SetValue(v);           // fires events
                    else
                        d.ResetToDefault();
                }
        }
    }

    private void ResetAllToDefaults()
    {
        if (!registry) return;

        if (registry.ints != null)
            foreach (var d in registry.ints)
                if (d) d.ResetToDefault();

        if (registry.floats != null)
            foreach (var d in registry.floats)
                if (d) d.ResetToDefault();
    }

    private static string KeyOf(string saveKey, string assetName)
        => string.IsNullOrEmpty(saveKey) ? assetName : saveKey;

    private void Write(SavePayload payload, string path)
    {
        var json = JsonUtility.ToJson(payload, prettyPrint: true);
        File.WriteAllText(path, json);
#if UNITY_EDITOR
        Debug.Log($"[SaveManager] Saved to {path}\n{json}");
#endif
    }

    private SavePayload Read(string path)
    {
        try
        {
            var json = File.ReadAllText(path);
            var payload = JsonUtility.FromJson<SavePayload>(json);
#if UNITY_EDITOR
            Debug.Log($"[SaveManager] Loaded from {path}\n{json}");
#endif
            return payload;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Failed to read {path}: {e}");
            return null;
        }
    }
}

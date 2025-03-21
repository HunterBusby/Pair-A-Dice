using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour {
    public static LocalizationManager Instance;

    private Dictionary<string, string> localizedText;
    public string currentLanguage = "en";

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLocalizedText(currentLanguage);
        } else {
            Destroy(gameObject);
        }
    }

    public void LoadLocalizedText(string languageCode) {
        currentLanguage = languageCode;
        localizedText = new Dictionary<string, string>();

        TextAsset jsonFile = Resources.Load<TextAsset>($"Localization/{languageCode}");
        if (jsonFile == null) {
            Debug.LogError($"Localization file not found: {languageCode}.json");
            return;
        }

        LocalizationData data = JsonUtility.FromJson<LocalizationData>(jsonFile.text);
        if (data != null) {
            foreach (var entry in data.entries) {
                localizedText[entry.key] = entry.value;
            }
        }
    }

    public string GetText(string id) {
        if (localizedText.ContainsKey(id)) {
            return localizedText[id];
        }
        return $"<Missing text: {id}>";
    }
}

[System.Serializable]
public class LocalizationData {
    public List<LocalizationEntry> entries;
}

[System.Serializable]
public class LocalizationEntry {
    public string key;
    public string value;
}

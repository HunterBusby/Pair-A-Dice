using UnityEngine;

[DisallowMultipleComponent]
public class ActiveStatePersistence : MonoBehaviour
{
    [Tooltip("Reference to the PersistentObjectId on this or another object. If null, we'll GetComponent on this object.")]
    public PersistentObjectId targetId;
    [Tooltip("Default fallback if nothing saved yet.")]
    public bool defaultActive = true;

    private string id;

    private void Awake()
    {
        if (!targetId) targetId = GetComponent<PersistentObjectId>();
        id = targetId ? targetId.id : null;

        // Apply saved state (or default if none saved yet)
        if (ActiveStateStore.TryGetSaved(id, out var saved))
            gameObject.SetActive(saved);
        else
            gameObject.SetActive(defaultActive);
    }

    // ----- Call these from your UnityEvents instead of SetActive -----

    public void SetActiveFromEvent(bool value)
    {
        // Save first, THEN change active state (so we persist even if we disable ourselves)
        ActiveStateStore.Save(id, value);
        gameObject.SetActive(value);
    }

    public void SetActiveTrue()  { SetActiveFromEvent(true);  }
    public void SetActiveFalse() { SetActiveFromEvent(false); }
    public void ToggleActive()   { SetActiveFromEvent(!gameObject.activeSelf); }

    // (Optional) If someone else called SetActive directly, you can manually resync:
    public void SaveCurrentNow()
    {
        ActiveStateStore.Save(id, gameObject.activeSelf);
    }
}

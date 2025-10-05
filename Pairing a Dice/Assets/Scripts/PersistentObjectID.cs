using UnityEngine;

[DisallowMultipleComponent]
public class PersistentObjectId : MonoBehaviour
{
    [Tooltip("Unique id for this object. Must be stable across sessions (e.g., Dice_Red_Button, Dice_Red_Preview).")]
    public string id;
}

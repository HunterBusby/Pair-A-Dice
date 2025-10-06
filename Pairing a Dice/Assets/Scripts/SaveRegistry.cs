using UnityEngine;

[CreateAssetMenu(menuName = "Saving/Save Registry")]
public class SaveRegistry : ScriptableObject
{
    [Tooltip("All IntData assets to include in save files.")]
    public IntData[] ints;

    [Tooltip("All FloatData assets to include in save files.")]
    public FloatData[] floats;
}

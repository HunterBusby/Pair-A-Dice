using UnityEngine;
using UnityEngine.Events;

public class RollListener : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent OnRollCounted;

    // Call this from DiceManager.OnRollFinished
    public void HandleRollFinished()
    {
        OnRollCounted?.Invoke();
    }
}

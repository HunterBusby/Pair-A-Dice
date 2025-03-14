using UnityEngine;
using UnityEngine.Events;

public class DoubleRollMeter : MonoBehaviour
{
    public int maxMeter = 5; // ✅ How many doubles needed for a power-up
    private int currentMeter = 0;

    public UnityEvent onMeterFull; // ✅ Event when meter reaches max

    public void IncreaseMeter()
    {
        currentMeter++;
        Debug.Log($"🔥 Double Meter: {currentMeter}/{maxMeter}");

        if (currentMeter >= maxMeter)
        {
            onMeterFull.Invoke(); // ✅ Trigger ability
            currentMeter = 0; // ✅ Reset the meter
        }
    }
}

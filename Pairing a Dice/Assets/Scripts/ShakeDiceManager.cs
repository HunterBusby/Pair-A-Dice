using UnityEngine;
using System.Collections;

public class ShakeDiceManager : MonoBehaviour
{
    public ShakeToRoll[] dice; // ✅ Assign Shake Dice in the Inspector
    private int latestDiceSum = 0;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ✅ Roll dice when clicked
        {
            TryRollDice();
        }
    }

    void TryRollDice()
    {
        

        foreach (ShakeToRoll die in dice)
        {
            die.GetComponent<DiceFaceDetector>().hasStoppedRolling = false; // ✅ Reset rolling status
        }

        StartCoroutine(WaitForDiceToStop()); // ✅ Wait for new values
    }

    private IEnumerator WaitForDiceToStop()
    {
        bool allDiceStopped = false;
        while (!allDiceStopped)
        {
            allDiceStopped = true;
            foreach (ShakeToRoll die in dice)
            {
                DiceFaceDetector detector = die.GetComponent<DiceFaceDetector>();
                if (!detector.hasStoppedRolling) // ✅ Wait until both dice report they have stopped
                {
                    allDiceStopped = false;
                    break;
                }
            }
            yield return null;
        }

        int totalSum = 0;
        foreach (ShakeToRoll die in dice)
        {
            totalSum += die.GetComponent<DiceFaceDetector>().GetFaceUpValue();
        }

        latestDiceSum = totalSum;
        Debug.Log("🌀 Shake Dice Final Sum: " + latestDiceSum);
    }

public void ResetDiceSum()
{
    latestDiceSum = 0;
    Debug.Log("🔄 Dice Sum Reset to 0 after match!");
}

    public int GetLatestDiceSum()
{
    return latestDiceSum;
}

}


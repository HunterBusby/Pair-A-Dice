using UnityEngine;
using System.Collections;
using UnityEngine.Events; // ✅ Required for Unity Events

public class ShakeDiceManager : MonoBehaviour
{
    public ShakeToRoll[] dice; // ✅ Assign Shake Dice in the Inspector
    private int latestDiceSum = 0;

    [Header("Doubles Event")]
    public UnityEvent onDoublesRolled; // ✅ Event triggered when doubles are rolled

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
        int dice1Value = dice[0].GetComponent<DiceFaceDetector>().GetFaceUpValue();
        int dice2Value = dice[1].GetComponent<DiceFaceDetector>().GetFaceUpValue();

        totalSum = dice1Value + dice2Value;
        latestDiceSum = totalSum;

        Debug.Log("Dice Final Sum: " + latestDiceSum);

        // ✅ Check if doubles were rolled
        if (DidRollDoubles(dice1Value, dice2Value))
        {
            Debug.Log("🎉 DOUBLES ROLLED!");
            onDoublesRolled.Invoke(); // ✅ Trigger the event
        }
    }

    public void ResetDiceSum()
    {
        latestDiceSum = 0;
        Debug.Log("🎲 Dice sum manually reset.");
    }

    public int GetLatestDiceSum()
    {
        return latestDiceSum;
    }

    private bool DidRollDoubles(int value1, int value2)
    {
        return value1 == value2;
    }
    
}

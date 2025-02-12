using UnityEngine;
using System.Collections;

public class DiceManager : MonoBehaviour
{
    public DiceRoll[] dice;
    private int latestDiceSum = 0;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryRollDice();
        }
    }

    void TryRollDice()
{
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    // ✅ Only roll dice if the player clicks directly on a dice object
    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
    {
        DiceRoll clickedDie = hit.collider.GetComponent<DiceRoll>();
        if (clickedDie != null) // ✅ Ensure a dice was clicked
        {
            latestDiceSum = 0; // ✅ Reset sum before rolling

            foreach (DiceRoll die in dice)
            {
                die.GetComponent<DiceFaceDetector>().hasStoppedRolling = false; // ✅ Reset stop state
                die.Roll();
            }

            StartCoroutine(WaitForDiceToStop()); // ✅ Start checking when dice stop
        }
    }
}


    private IEnumerator WaitForDiceToStop()
    {
        bool allDiceStopped = false;
        while (!allDiceStopped)
        {
            allDiceStopped = true;
            foreach (DiceRoll die in dice)
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
        foreach (DiceRoll die in dice)
        {
            totalSum += die.GetComponent<DiceFaceDetector>().GetFaceUpValue();
        }

        latestDiceSum = totalSum;
        Debug.Log("Final Dice Sum: " + latestDiceSum);
    }

    public int GetLatestDiceSum()
    {
        return latestDiceSum;
    }
}

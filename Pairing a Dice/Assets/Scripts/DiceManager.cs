using UnityEngine;
using System.Collections;

public class DiceManager : MonoBehaviour
{
    public DiceRoll[] dice;  // Assign both dice in the Inspector
    public LayerMask diceLayerMask;  // Assign the "Dice" layer
    public float checkDelay = 2f; // Wait time before checking dice value
    private int latestDiceSum = 0; // Store the latest rolled sum

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // Left-click rolls dice
        {
            TryRollDice();
        }
    }

    void TryRollDice()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if a dice was clicked before rolling
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, diceLayerMask))
        {
            RollBothDice();
        }
    }

    void RollBothDice()
    {
        foreach (DiceRoll die in dice)
        {
            die.Roll(); // Roll each die
        }

        // Start coroutine to check the rolled value after a delay
        StartCoroutine(CheckDiceAfterDelay());
    }

    IEnumerator CheckDiceAfterDelay()
    {
        yield return new WaitForSeconds(checkDelay); // Wait for dice to land

        int totalSum = 0;
        foreach (DiceRoll die in dice)
        {
            totalSum += die.GetComponent<DiceFaceDetector>().GetFaceUpValue();
        }
        latestDiceSum = totalSum; // Store the latest dice roll result
        Debug.Log("Latest Dice Sum: " + latestDiceSum);
    }

    public int GetLatestDiceSum()
    {
        return latestDiceSum;
    }
}

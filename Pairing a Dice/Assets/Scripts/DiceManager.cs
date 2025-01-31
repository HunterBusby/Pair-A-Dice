using UnityEngine;
using System.Collections;

public class DiceManager : MonoBehaviour
{
    public DiceRoll[] dice;  // Assign both dice in the Inspector
    public LayerMask diceLayerMask;  // Assign the "Dice" layer
    public float checkDelay = 2f; // Wait time before checking dice value

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // Left-click
        {
            TryRollDice();
        }
    }

    void TryRollDice()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if a dice was clicked
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, diceLayerMask))
        {
            RollBothDice();
        }
    }

    void RollBothDice()
    {
        foreach (DiceRoll die in dice)
        {
            die.Roll();  // Roll each die
        }

        // Start coroutine to check the rolled value after a delay
        StartCoroutine(CheckDiceAfterDelay());
    }

    IEnumerator CheckDiceAfterDelay()
    {
        yield return new WaitForSeconds(checkDelay); // Wait for the dice to land

        foreach (DiceRoll die in dice)
        {
            int rolledValue = die.GetComponent<DiceFaceDetector>().GetFaceUpValue();
            Debug.Log("The dice rolled a: " + rolledValue);
        }
    }
}

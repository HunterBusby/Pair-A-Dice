using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class ShakeDiceManager : MonoBehaviour
{
    public ShakeToRoll[] dice;
    private int latestDiceSum = 0;

    [Header("Doubles Event")]
    public UnityEvent onDoublesRolled;

    [Header("Roll Zone Settings")]
    public Collider diceRollZone; // Assign in Inspector

    private bool isCursorInZone = false;
    private bool hasInitiatedShake = false;

    // 👇 Guard fields
    private Coroutine waitRoutine;
    private bool doublesHandledThisRoll = false;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool currentlyHoveringZone = false;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == diceRollZone)
            {
                currentlyHoveringZone = true;

                if (!isCursorInZone)
                {
                    EnterZone(); // Trigger glow
                }

                // ✅ Clicked while in zone → start shake
                if (Input.GetMouseButtonDown(0))
                {
                    hasInitiatedShake = true;

                    foreach (ShakeToRoll die in dice)
                    {
                        die.StartShakingFromZone();
                    }
                }
            }
        }

        // ✅ Stop shaking anywhere on screen if mouse released
        if (hasInitiatedShake && Input.GetMouseButtonUp(0))
        {
            hasInitiatedShake = false;

            foreach (ShakeToRoll die in dice)
            {
                die.StopShakingFromZone();
            }
        }

        // ✅ If mouse leaves zone, stop glow but not shaking
        if (!currentlyHoveringZone && isCursorInZone)
        {
            ExitZone();
        }
    }

    void EnterZone()
    {
        isCursorInZone = true;

        foreach (ShakeToRoll die in dice)
        {
            var glow = die.GetComponent<CardGlowOnHover>();
            if (glow != null)
                glow.SetGlowExternally(true);
        }
    }

    void ExitZone()
    {
        isCursorInZone = false;

        foreach (ShakeToRoll die in dice)
        {
            var glow = die.GetComponent<CardGlowOnHover>();
            if (glow != null)
                glow.SetGlowExternally(false);
        }
    }

    // 👇 Guarded starter
    public void StartWaitForDiceToStopOnce()
    {
        if (waitRoutine != null) return; // already running
        doublesHandledThisRoll = false;  // reset per roll
        waitRoutine = StartCoroutine(WaitForDiceToStop());
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
                if (!detector.hasStoppedRolling)
                {
                    allDiceStopped = false;
                    break;
                }
            }

            yield return null;
        }

        // Once all dice have stopped
        int sum = 0;
        foreach (ShakeToRoll die in dice)
        {
            sum += die.GetComponent<DiceFaceDetector>().GetFaceUpValue();
        }

        NotifyRollComplete(sum);

        // Clear guard
        waitRoutine = null;
    }

    public void NotifyRollComplete(int sum)
    {
        latestDiceSum = sum;
        Debug.Log("Dice Final Sum: " + latestDiceSum);

        // Optional: trigger printer card
        BrandonCardPrinter printer = FindFirstObjectByType<BrandonCardPrinter>();
        if (printer != null)
        {
            printer.PrintCard();
        }

        // Handle doubles
        int v1 = dice[0].GetComponent<DiceFaceDetector>().GetFaceUpValue();
        int v2 = dice[1].GetComponent<DiceFaceDetector>().GetFaceUpValue();
        if (v1 == v2)
        {
            if (doublesHandledThisRoll) return; // 👈 debounce
            doublesHandledThisRoll = true;

            Debug.Log("🎉 DOUBLES ROLLED!");
            onDoublesRolled.Invoke();
        }
    }

    public int GetLatestDiceSum()
    {
        return latestDiceSum;
    }

    public void ResetDiceSum()
    {
        latestDiceSum = 0;
        Debug.Log("🎲 Dice sum manually reset.");
    }
}

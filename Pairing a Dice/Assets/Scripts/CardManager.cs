using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{

    [Header("Robot Arm Visual Sync")]
public Transform followProxy; // Assign this in Inspector
public RobotArmController robotArm; // Assign this too

    public List<Transform> playerCards = new List<Transform>();
    public List<Transform> enemyCards = new List<Transform>();
    public Transform playerSide;
    public Transform enemySide;
    public float spacing = 1.5f;
    public float liftHeight = 0.5f;
    public float moveSpeed = 2f;
    public int maxCardsPerRow = 5;

    private Dictionary<Transform, bool> isMoving = new Dictionary<Transform, bool>();

    [Header("Win Condition Events")]
    public UnityEvent onPlayerWin;
    public UnityEvent onAIWin;

    [Header("UNO Alert Events")]
    public UnityEvent onPlayerUnoStart;  
    public UnityEvent onPlayerUnoStop;   
    public UnityEvent onEnemyUnoStart;   
    public UnityEvent onEnemyUnoStop;    
    private bool sixtyNineTriggered = false; // ✅ Prevents multiple activations
    private bool playerUnoTriggered = false;
    private bool enemyUnoTriggered = false;

    [Header("Nice! (69 Detection)")]
    public UnityEvent onSixtyNineDetected; // ✅ Event triggered when "69" appears

    public Vector3 GetCardPosition(Transform side, int index)
    {
        int row = index / maxCardsPerRow;
        int column = index % maxCardsPerRow;
        return side.position + new Vector3(column * spacing, 0, -row * spacing);
    }

    public void TransferCard(Transform card, bool toEnemy, bool triggerRobotArm = false)

{
    if (isMoving.ContainsKey(card) && isMoving[card])
    {
        Debug.Log("🚫 Cannot move card: " + card.name + " is still animating!");
        return;
    }
    isMoving[card] = true; // Set immediately to ensure accurate proxy behavior


    Transform targetSide = toEnemy ? enemySide : playerSide;
    List<Transform> targetList = toEnemy ? enemyCards : playerCards;

    playerCards.Remove(card);
    enemyCards.Remove(card);
    targetList.Add(card);

    // 🎵 Update music based on current player card count
    BossMusicManager musicManager = FindFirstObjectByType<BossMusicManager>();
    if (musicManager != null)
    {
        musicManager.OnCardCountChanged(playerCards.Count);
    }

    if (triggerRobotArm && robotArm != null && followProxy != null)
{
    followProxy.position = card.position;
    StartCoroutine(FollowProxyDuringMove(card));
}


    // 🟢 Now start moving the actual card
    StartCoroutine(MoveCard(card, targetSide, targetList));

    CheckUnoCondition();
    CheckWinCondition();
    CheckForSixtyNine(targetList);
}



    private IEnumerator MoveCard(Transform card, Transform targetSide, List<Transform> targetList)
    {
        isMoving[card] = true;

        Vector3 startPosition = card.position;
        Vector3 liftedPosition = startPosition + Vector3.up * liftHeight;
        Vector3 targetPosition = GetCardPosition(targetSide, targetList.Count - 1);

        float elapsedTime = 0f;
        while (elapsedTime < moveSpeed / 2)
        {
            card.position = Vector3.Lerp(startPosition, liftedPosition, elapsedTime / (moveSpeed / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < moveSpeed)
        {
            card.position = Vector3.Lerp(liftedPosition, targetPosition, elapsedTime / moveSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        card.position = targetPosition;
        isMoving[card] = false;

        RepositionCards(targetList, targetSide);
    }

    public void RepositionCards(List<Transform> cards, Transform side)
{
    for (int i = 0; i < cards.Count; i++)
    {
        Vector3 targetPosition = GetCardPosition(side, i);
        StartCoroutine(SmoothMove(cards[i], targetPosition));
    }

    CheckForSixtyNine(cards); // ✅ Check for 69 after repositioning
}


    private IEnumerator SmoothMove(Transform card, Vector3 targetPosition)
    {
        if (isMoving.ContainsKey(card) && isMoving[card])
            yield break;

        isMoving[card] = true;

        Vector3 startPosition = card.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveSpeed)
        {
            card.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        card.position = targetPosition;
        isMoving[card] = false;
    }

    private void CheckWinCondition()
    {
        if (playerCards.Count == 0)
        {
            Debug.Log("🎉 Player Wins! Triggering onPlayerWin Event.");
            onPlayerWin.Invoke();
        }
        else if (enemyCards.Count == 0)
        {
            Debug.Log("💀 AI Wins! Triggering onAIWin Event.");
            onAIWin.Invoke();
        }
    }

    private void CheckUnoCondition()
    {
        if (playerCards.Count == 1 && !playerUnoTriggered)
        {
            Debug.Log("⚠️ Player UNO! Starting looped sound.");
            onPlayerUnoStart.Invoke();
            playerUnoTriggered = true;
        }
        else if (playerCards.Count > 1 && playerUnoTriggered)
        {
            Debug.Log("✅ Player no longer at UNO. Stopping sound.");
            onPlayerUnoStop.Invoke();
            playerUnoTriggered = false;
        }

        if (enemyCards.Count == 1 && !enemyUnoTriggered)
        {
            Debug.Log("⚠️ Enemy UNO! Starting looped sound.");
            onEnemyUnoStart.Invoke();
            enemyUnoTriggered = true;
        }
        else if (enemyCards.Count > 1 && enemyUnoTriggered)
        {
            Debug.Log("✅ Enemy no longer at UNO. Stopping sound.");
            onEnemyUnoStop.Invoke();
            enemyUnoTriggered = false;
        }
    }
private void CheckForSixtyNine(List<Transform> cards)
{
    if (cards.Count < 2) return; // ✅ Need at least 2 cards to check

    for (int i = 0; i < cards.Count - 1; i++) // ✅ Check only adjacent pairs
    {
        int leftCardValue = GetCardValue(cards[i]);
        int rightCardValue = GetCardValue(cards[i + 1]);

        if (leftCardValue == 6 && rightCardValue == 9)
        {
            if (!sixtyNineTriggered) // ✅ Only trigger once
            {
                Debug.Log("🎉 NICE! 69 DETECTED!");
                onSixtyNineDetected.Invoke();
                sixtyNineTriggered = true;
            }
            return;
        }
    }

    // ✅ Reset if "69" no longer exists
    sixtyNineTriggered = false;
}


    private int GetCardValue(Transform card)
    {
        IDContainerBehaviour idContainer = card.GetComponent<IDContainerBehaviour>();
        if (idContainer != null && idContainer.idObj != null)
        {
            if (int.TryParse(idContainer.idObj.name.Replace("ID_", ""), out int value))
            {
                return value;
            }
        }
        return -1;
    }

    public Vector3 GetPlayerSideDropPosition()
{
    // Return the position where cards land on the player side
    // This can be the center point or average position of player cards
    if (playerCards.Count > 0)
        return playerCards[playerCards.Count - 1].position;
    else
        return playerSide.position + Vector3.right * 2f; // Fallback position
}

public bool IsCardMoving(Transform card)
{
    return isMoving.ContainsKey(card) && isMoving[card];
}

private IEnumerator FollowProxyDuringMove(Transform card)
{
    if (robotArm != null && followProxy != null)
        robotArm.FollowProxy(followProxy);

    while (isMoving.ContainsKey(card) && isMoving[card])
    {
        followProxy.position = card.position + Vector3.up * 0.2f; // slight lift
        yield return null;
    }

    yield return new WaitForSeconds(0.3f);
    robotArm.ReturnToRest();
}



}

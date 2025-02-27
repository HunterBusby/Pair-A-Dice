using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public List<Transform> playerCards = new List<Transform>();
    public List<Transform> enemyCards = new List<Transform>();
    public Transform playerSide;
    public Transform enemySide;
    public float spacing = 1.5f;
    public float liftHeight = 0.5f;
    public float moveSpeed = 2f;
    public int maxCardsPerRow = 5;

    private Dictionary<Transform, bool> isMoving = new Dictionary<Transform, bool>(); // ✅ Tracks card movement status

    [Header("Win Condition Events")]
    public UnityEvent onPlayerWin; // ✅ Fires when player wins
    public UnityEvent onAIWin; // ✅ Fires when AI wins

    public Vector3 GetCardPosition(Transform side, int index)
    {
        int row = index / maxCardsPerRow;
        int column = index % maxCardsPerRow;
        return side.position + new Vector3(column * spacing, 0, -row * spacing);
    }

    public void TransferCard(Transform card, bool toEnemy)
    {
        if (isMoving.ContainsKey(card) && isMoving[card])
        {
            Debug.Log("🚫 Cannot move card: " + card.name + " is still animating!");
            return; // ✅ Prevents teleporting if animation is already running
        }

        Transform targetSide = toEnemy ? enemySide : playerSide;
        List<Transform> targetList = toEnemy ? enemyCards : playerCards;

        playerCards.Remove(card);
        enemyCards.Remove(card);
        targetList.Add(card);

        StartCoroutine(MoveCard(card, targetSide, targetList));

        // ✅ Check win conditions after transferring the card
        CheckWinCondition();
    }

    private IEnumerator MoveCard(Transform card, Transform targetSide, List<Transform> targetList)
    {
        isMoving[card] = true; // ✅ Mark as moving

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
        isMoving[card] = false; // ✅ Allow interaction again

        RepositionCards(targetList, targetSide);
    }

    public void RepositionCards(List<Transform> cards, Transform side)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Vector3 targetPosition = GetCardPosition(side, i);
            StartCoroutine(SmoothMove(cards[i], targetPosition));
        }
    }

    private IEnumerator SmoothMove(Transform card, Vector3 targetPosition)
    {
        if (isMoving.ContainsKey(card) && isMoving[card])
            yield break; // ✅ Don't reposition if it's already moving

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
        isMoving[card] = false; // ✅ Unlock movement
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
}

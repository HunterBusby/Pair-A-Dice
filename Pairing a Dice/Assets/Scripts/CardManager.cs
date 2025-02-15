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

    // ✅ Events for game win conditions
    public UnityEvent onPlayerWin;
    public UnityEvent onAIWin;

    public void TransferCard(Transform card, bool toEnemy)
    {
        Debug.Log("TransferCard Called for: " + card.name + " | To Enemy: " + toEnemy);
        Transform targetSide = toEnemy ? enemySide : playerSide;
        List<Transform> targetList = toEnemy ? enemyCards : playerCards;

        playerCards.Remove(card);
        enemyCards.Remove(card);
        targetList.Add(card);

        StartCoroutine(MoveCard(card, targetSide, targetList));
        CheckForWin(); // ✅ Check if someone has won after every transfer
    }

    private IEnumerator MoveCard(Transform card, Transform targetSide, List<Transform> targetList)
    {
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
        RepositionCards(targetList, targetSide);
    }

    private void CheckForWin()
    {
        if (playerCards.Count == 0)
        {
            Debug.Log("Player Wins! 🎉");
            onPlayerWin.Invoke(); // ✅ Trigger Player Win Event
        }
        else if (enemyCards.Count == 0)
        {
            Debug.Log("AI Wins! 🤖");
            onAIWin.Invoke(); // ✅ Trigger AI Win Event
        }
    }

    public void RepositionCards(List<Transform> cards, Transform side)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Vector3 targetPosition = GetCardPosition(side, i);
            StartCoroutine(SmoothMove(cards[i], targetPosition));
        }
    }

    public Vector3 GetCardPosition(Transform side, int index)
    {
        int row = index / maxCardsPerRow;
        int column = index % maxCardsPerRow;
        return side.position + new Vector3(column * spacing, 0, -row * spacing);
    }

    private IEnumerator SmoothMove(Transform card, Vector3 targetPosition)
    {
        Vector3 startPosition = card.position;
        float elapsedTime = 0f;
        float duration = moveSpeed > 0 ? (1f / moveSpeed) : 0.5f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            card.position = Vector3.Lerp(startPosition, targetPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        card.position = targetPosition;
    }
}

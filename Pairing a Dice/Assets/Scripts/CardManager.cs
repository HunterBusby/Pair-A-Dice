using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public void TransferCard(Transform card, bool toEnemy)
{
    Debug.Log("TransferCard Called for: " + card.name + " | To Enemy: " + toEnemy);
    Transform targetSide = toEnemy ? enemySide : playerSide;
    List<Transform> targetList = toEnemy ? enemyCards : playerCards;

    playerCards.Remove(card);
    enemyCards.Remove(card);
    targetList.Add(card);

    StartCoroutine(MoveCard(card, targetSide, targetList));
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

        // ✅ Ensure final positioning is updated for all cards on this side
        RepositionCards(targetList, targetSide);
    }

    public Vector3 GetCardPosition(Transform side, int index) 
{
    int row = index / maxCardsPerRow;
    int column = index % maxCardsPerRow;
    return side.position + new Vector3(column * spacing, 0, -row * spacing);
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
    Vector3 startPosition = card.position;
    float elapsedTime = 0f;
    float duration = moveSpeed; // ✅ Now moveSpeed directly controls duration

    while (elapsedTime < duration)
    {
        float t = elapsedTime / duration; // Normalize time (0 to 1)
        card.position = Vector3.Lerp(startPosition, targetPosition, t); // Apply smooth movement

        elapsedTime += Time.deltaTime;
        yield return null; // ✅ Ensures real-time updates
    }

    card.position = targetPosition; // Ensure exact final position
}



}
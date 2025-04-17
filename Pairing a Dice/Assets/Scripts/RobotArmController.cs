using System.Collections;
using UnityEngine;

public class RobotArmController : MonoBehaviour
{
    [Header("References")]
    public Transform ikTarget;
    public Transform restPosition;

    public CardManager cardManager;  // Needed to check if the card is still moving

    public float moveSpeed = 6f; // Higher speed to keep up with card

    private Coroutine trackingRoutine;

  public void FollowProxy(Transform followTarget)
{
    if (trackingRoutine != null)
        StopCoroutine(trackingRoutine);

    trackingRoutine = StartCoroutine(FollowTransform(followTarget));
}

private IEnumerator FollowTransform(Transform target)
{
    while (target != null)
    {
        ikTarget.position = Vector3.Lerp(ikTarget.position, target.position, moveSpeed * Time.deltaTime);
        yield return null;
    }

    yield return new WaitForSeconds(0.3f);
    ReturnToRest();
}


    public void ReturnToRest()
    {
        if (trackingRoutine != null)
            StopCoroutine(trackingRoutine);

        trackingRoutine = StartCoroutine(MoveToPosition(restPosition.position));
    }

    private IEnumerator FollowCardPosition(Transform card)
{
    // Wait a short delay to let the robot reach the card before it starts moving
    yield return new WaitForSeconds(0.2f);

    // Follow the card while CardManager says it's still moving
    while (card != null && cardManager.IsCardMoving(card))
    {
        ikTarget.position = Vector3.Lerp(ikTarget.position, card.position, moveSpeed * Time.deltaTime);
        yield return null;
    }

    // One final position update at the end
    if (card != null)
        ikTarget.position = card.position;

    yield return new WaitForSeconds(0.3f);
    ReturnToRest();
}



    private IEnumerator MoveToPosition(Vector3 target)
    {
        while (Vector3.Distance(ikTarget.position, target) > 0.05f)
        {
            ikTarget.position = Vector3.Lerp(ikTarget.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}

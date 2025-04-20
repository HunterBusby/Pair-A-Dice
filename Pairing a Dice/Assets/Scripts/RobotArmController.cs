using System.Collections;
using UnityEngine;

public class RobotArmController : MonoBehaviour
{
    [Header("References")]
    public Transform ikTarget;
    public Transform restPosition;

    public CardManager cardManager;  // Needed to check if the card is still moving

    private Coroutine trackingRoutine;

     [Header("Speeds")]
public float followSpeed = 100f;     // For tracking the moving card
public float grabSpeed = 6f;   

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
        ikTarget.position = Vector3.Lerp(ikTarget.position, target.position, followSpeed * Time.deltaTime);

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
        ikTarget.position = Vector3.Lerp(ikTarget.position, card.position, grabSpeed * Time.deltaTime);
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
            ikTarget.position = Vector3.Lerp(ikTarget.position, target, grabSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator MoveProxyToHome(Transform proxy)
{
    if (restPosition == null || proxy == null)
        yield break;

    Vector3 start = proxy.position;
    Vector3 target = restPosition.position;
    float duration = 0.4f;
    float elapsed = 0f;

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        proxy.position = Vector3.Lerp(start, target, elapsed / duration);
        yield return null;
    }

    proxy.position = target;
}

}
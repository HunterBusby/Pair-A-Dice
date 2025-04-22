using System.Collections;
using UnityEngine;

public class RobotArmController : MonoBehaviour
{
    [Header("References")]
    public Transform ikTarget;            // The IK target the arm follows
    public Transform restPosition;        // Where the arm returns to after a grab
    public Transform followProxy;         // Invisible object the IK follows
    public CardManager cardManager;       // For checking movement & initiating transfer

    [Header("Speeds")]
    public float followSpeed = 100f;      // Fast tracking while card moves
    public float grabSpeed = 6f;          // Initial grab/retract speed

    private Coroutine trackingRoutine;

    private void OnEnable()
    {
        AIOpponentEvents.OnCardMatched += AnimateFullSend;
    }

    private void OnDisable()
    {
        AIOpponentEvents.OnCardMatched -= AnimateFullSend;
    }

    private void AnimateFullSend(Transform cardTransform)
    {
        MatchBehaviour card = cardTransform.GetComponent<MatchBehaviour>();
        if (card != null)
            StartCoroutine(AnimateCardSendAndThenMoveCard(card));
    }

    public IEnumerator AnimateCardSendAndThenMoveCard(MatchBehaviour card)
    {
        if (followProxy == null || card == null || cardManager == null)
            yield break;

        Transform cardTransform = card.transform;

        // Step 1: Start following the proxy immediately
FollowProxy(followProxy);

// Step 2: Move the proxy to the card
Vector3 start = followProxy.position;
Vector3 target = cardTransform.position + Vector3.up * 0.2f;
float speed = 10f;

while (Vector3.Distance(followProxy.position, target) > 0.01f)
{
    followProxy.position = Vector3.MoveTowards(followProxy.position, target, speed * Time.deltaTime);
    yield return null;
}

followProxy.position = target;


        // Step 3: Activate card, delay, then move it
        card.matchEvent.Invoke();
        yield return new WaitForSeconds(0.1f);
        cardManager.TransferCard(cardTransform, false);

        // Step 4: Follow card while it moves
        StartCoroutine(cardManager.FollowProxyDuringMove(cardTransform));
    }

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

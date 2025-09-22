using UnityEngine;

public class AnimStatesBehaviour : StateMachineBehaviour
{
    [Tooltip("Unique key for THIS state (e.g., 'Entrance', 'Exit')")]
    public string stateKey;

    AnimStateRelay GetRelay(Animator animator) => animator ? animator.GetComponent<AnimStateRelay>() : null;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetRelay(animator)?.InvokeEnter(stateKey);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetRelay(animator)?.InvokeUpdate(stateKey);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetRelay(animator)?.InvokeExit(stateKey);
    }
}

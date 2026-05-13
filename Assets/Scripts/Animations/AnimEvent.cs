using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.UIElements;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class AnimEvent : StateMachineBehaviour
{
    [SerializeField] int eventIndex;
    [SerializeField] UnitTime time;
    [SerializeField] bool invokeOnExit = true;
    AnimEventChannel channel;
    bool invoked = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (channel == null) channel = animator.GetComponent<AnimEventChannel>();
        invoked = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (channel == null || invoked) return;
        if(stateInfo.normalizedTime * stateInfo.length >= time)
        {
            channel.CallEvent(eventIndex);
            invoked = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!invoked && invokeOnExit)
        {
            channel.CallEvent(eventIndex);
            invoked = true;
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
using System.Collections;
using System.Collections.Generic;
using Lullaby.Entities;
using UnityEngine;

public class RandomStateSMB : StateMachineBehaviour
{
    
    public int numberOfStates = 3;
    public float minNormTime = 3f;
    public float maxNormTime = 7f;

    protected float m_RandomNormTime;
    protected Player _player;
    readonly int m_HashRandomIdle = Animator.StringToHash("RandomIdle");
    readonly int m_HashInRandomIdle = Animator.StringToHash("InRandomIdle");
    readonly int m_StateHash = Animator.StringToHash("State");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Randomly decide a time at which to transition.
        m_RandomNormTime = Random.Range(minNormTime, maxNormTime);
        _player = FindObjectOfType<Player>();
        
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If trainsitioning away from this state reset the random idle parameter to -1.
        if (animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).fullPathHash == stateInfo.fullPathHash)
        {
            animator.SetInteger(m_HashRandomIdle, -1);
            animator.SetBool(m_HashInRandomIdle, false);
        }

        // If the state is beyond the randomly decided normalised time and not yet transitioning then set a random idle.
        if (stateInfo.normalizedTime > m_RandomNormTime && !animator.IsInTransition(0))
        {
            int randomProb = Random.Range(0, 10);
            int randomState = randomProb > 5? 0:1;
            
            animator.SetBool(m_HashInRandomIdle, true);
            animator.SetInteger(m_HashRandomIdle, randomState);
            animator.SetInteger(m_StateHash, -2);
            _player.playerEvents.OnRandomIdleEnter.Invoke(randomState);
        }
    }
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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

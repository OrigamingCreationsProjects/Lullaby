using Lullaby.Entities;
using Lullaby.Entities.States;
using Unity.VisualScripting;
using UnityEngine;

namespace Lullaby
{
    public class AnimatorChangingTransitions : StateMachineBehaviour
    {
        // [ClassTypeName(typeof(PlayerState))]
        // public string nextState;
        //
        // public Player player;
        // public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //     player = FindObjectOfType<Player>();
        // }
        //
        // public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //     //if(animator.GetInteger("State") == 0) player.states.Change<IdlePlayerState>();
        // }
    }
}
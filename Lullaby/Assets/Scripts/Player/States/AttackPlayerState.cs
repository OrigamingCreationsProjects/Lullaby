using Unity.VisualScripting;
using UnityEngine;

namespace Lullaby.Entities.States
{
    [AddComponentMenu("Lullaby/CustomMovement/Player/States/Attack Player State")]
    public class AttackPlayerState: PlayerState
    {
        protected GameObject _skinAttackPosSlot;
        protected override void OnEnter(Player player)
        {
            player.playerEvents.OnAttackStarted?.Invoke();
            //
            // if (!_skinAttackPosSlot)
            // {
            //     _skinAttackPosSlot = new GameObject();
            //     _skinAttackPosSlot.gameObject.name = "AttackPosSlot";
            // }
            //
            // _skinAttackPosSlot.transform.position = player.transform.position; // Asignamos la posicion del player a la del skin
            // _skinAttackPosSlot.transform.rotation = player.transform.rotation; // Asignamos la rotacion del player a la del skin
            // _skinAttackPosSlot.transform.parent = player.transform.parent; // Asignamos el padre del player al del skin
            
            player.velocity = Vector3.zero;
            // player.SetSkinParent(_skinAttackPosSlot.transform); // Asignamos el padre del skin al del player
        }

        protected override void OnExit(Player player)
        {
            player.playerEvents.OnAttackFinished?.Invoke();
            //player.transform.position = _skinAttackPosSlot.GetComponentInChildren<Animator>().transform.position;
            player.ResetSkinParent();
        }

        public override void OnStep(Player player)
        {
            player.ApplyGravity();
            player.SnapToGround();
            //player.AccelerateToInputDirection();
            //player.transform.position = _skinAttackPosSlot.GetComponentInChildren<Animator>().transform.position;
            // MANEJAREMOS ESTO DESDE EL ANIMATOR
            if (player.inputs.GetAttackDown())
            {
                timeSinceEntered = 0;
            }
            
            if (timeSinceEntered >= player.stats.current.attackDuration)
            {
                if(player.isGrounded)
                {
                    player.states.Change<IdlePlayerState>();
                }
                else
                { 
                    player.states.Change<FallPlayerState>();
                }
            }
        }

        public override void OnContact(Player player, Collider other) { }
        
    }
}
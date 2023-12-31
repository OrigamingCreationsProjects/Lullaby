﻿using UnityEngine;

namespace Lullaby.Entities.States
{
    [AddComponentMenu("Lullaby/CustomMovement/Player/States/Fall Player State")]
    public class FallPlayerState : PlayerState
    {
        protected override void OnEnter(Player player){}

        protected override void OnExit(Player player){}

        public override void OnStep(Player player)
        {
            player.ApplyGravity();
            player.SnapToGround();
            player.FaceDirectionSmooth(player.lateralVelocity);
            player.AccelerateToInputDirection();
            player.Jump();
            player.Attack(); //Se podria quitar si al final no se deja atacar en el aire
            player.PickAndThrow();
            player.LedgeGrab();
            player.Dash();
            player.HandleMoonLauncher();
            //player.Glide(); //De momento desactivamos al no haber planeo (de momento)
            
            if (player.isGrounded)
            {
                player.states.Change<IdlePlayerState>();
            }
        }

        public override void OnContact(Player player, Collider other)
        {
            player.PushRigidbody(other);
            player.WallDrag(other);
        }
    }
}
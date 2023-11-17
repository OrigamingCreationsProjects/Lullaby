using UnityEngine;

namespace Lullaby.Entities.States
{
    [AddComponentMenu("Lullaby/CustomMovement/Player/States/Idle Player State")]
    public class WallDragPlayerState: PlayerState
    {
        protected override void OnEnter(Player player)
        {
            player.ResetJumps();
            //player.ResetAirDash();
            player.velocity = Vector3.zero;
            // Calculamos la posición de la skin en funcion de la rotación del player y el offset de la skin al agarrar la pared
            player.skin.position += player.transform.rotation * player.stats.current.wallDragSkinOffset; 
            // Calculamos la dirección a la que mira el player cuando está pegado a la pared
            var faceDirection = player.lastWallNormal - player.transform.up * Vector3.Dot(player.lastWallNormal, player.transform.up); 
            player.FaceDirection(faceDirection, Space.World); // Rotamos el player para que mire a la dirección calculada
                                                                                                         
        }

        protected override void OnExit(Player player)
        {
            player.skin.position -= player.transform.rotation * player.stats.current.wallDragSkinOffset;
            
            if(!player.isGrounded && player.transform.parent != player.initialParent)
                player.transform.parent = player.initialParent;
        }

        public override void OnStep(Player player)
        {
            // Aplicamos el deslizamiento hacia abajo de la pared para que descienda más lentamente
            player.verticalVelocity += Vector3.down * (player.stats.current.wallDragGravity * Time.deltaTime);
            
            var maxWallDistance = player.radius + player.stats.current.ledgeMaxForwardDistance;
            var detectingWall = player.SphereCast(-player.transform.forward, maxWallDistance,
                player.stats.current.wallDragLayers);
            
            if(player.isGrounded || !detectingWall)
            {
                player.states.Change<IdlePlayerState>();
                return;
            }

            if (player.inputs.GetJumpDown())
            {
                if(player.stats.current.wallJumpLockMovement)
                    // Bloqueamos el movimiento del player para que al saltar en el wallDrag se mueva recto y no hacia donde apunta el joystick
                    player.inputs.LockedMovementDirection(); 
                // Saltamos en la dirección a la que mira el personaje
                player.DirectionalJump(player.localForward, player.stats.current.wallJumpHeight, player.stats.current.wallJumpDistance); 
                player.states.Change<FallPlayerState>();
            }
        }

        public override void OnContact(Player player, Collider other){}
    }
}
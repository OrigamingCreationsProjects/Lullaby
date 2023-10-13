using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lullaby.Entities.States
{
    [AddComponentMenu("Lullaby/CustomMovement/Player/States/Ledge Hanging Player State")]
    public class LedgeHangingPlayerState: PlayerState
    {
        protected bool keepParent;
        protected Coroutine clearParentRoutine;

        protected const float clearParentDelay = 0.25f;
        
        protected override void OnEnter(Player player)
        {
            if(clearParentRoutine != null)
                player.StopCoroutine(clearParentRoutine);
            
            keepParent = false;
            player.velocity = Vector3.zero;
            // Colocamos la skin en función del offset para ajustarlo a los bordes
            player.skin.position += player.transform.rotation * player.stats.current.ledgeHangingSkinOffset; 
            player.ResetJumps();
            player.ResetAirDash();
        }

        protected override void OnExit(Player player)
        {
            clearParentRoutine = player.StartCoroutine(ClearParentRoutine(player));
            player.skin.position -= player.transform.rotation * player.stats.current.ledgeHangingSkinOffset;
        }


        public override void OnStep(Player player)
        {
            var ledgeTopMaxDistance = player.radius + player.stats.current.ledgeMaxForwardDistance; // Distancia máxima a la que podemos engancharnos del borde
            var ledgeTopHeightOffset = player.height * 0.5f + player.stats.current.ledgeMaxDownwardDistance; // Altura máxima a la que podemos engancharnos del borde
            var topOrigin = player.position + player.transform.up * ledgeTopHeightOffset +
                            player.transform.forward * ledgeTopMaxDistance; // Origen del rayo que detecta el borde
            var sideOrigin = player.position + player.transform.up * (player.height * 0.5f) - 
                             player.transform.up * player.stats.current.ledgeSideHeightOffset; // Origen del rayo que detecta la pared
            
            var rayDistance = player.radius + player.stats.current.ledgeSideMaxDistance; // Distancia máxima a la que llega el rayo para engancharnos
            var rayRadius = player.stats.current.ledgeSideCollisionRadius; // Radio del rayo que detecta la pared

            var detectingBorder = Physics.Raycast(topOrigin, -player.transform.up, out var topHit, player.height,
                player.stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore); // Detectamos el borde
            var detectingWall = Physics.SphereCast(sideOrigin, rayRadius, player.transform.forward, out var sideHit, 
                rayDistance, player.stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore); // Detectamos la pared

            if(!detectingBorder || !detectingWall) // Si no detectamos el borde o la pared, cambiamos de estado
            {
                player.states.Change<FallPlayerState>();
                return;
            }
            
            var inputDirection = player.inputs.GetMovementDirection(); // Dirección de movimiento del jugador
            var ledgeSideOrigin = sideOrigin + player.transform.right * (Mathf.Sign(inputDirection.x) * player.radius); // Origen del rayo que detecta el borde lateral
            var sideForward = -(sideHit.normal - player.transform.up * Vector3.Dot(player.transform.up, sideHit.normal)).normalized; // Dirección del rayo que detecta el borde lateral
            var destinationHeight = player.height * 0.5f + Physics.defaultContactOffset; // Altura a la que queremos llegar cuando se vaya hacia adelante
            var climbDestination = topHit.point + player.transform.up * destinationHeight +
                                   player.transform.forward * player.radius; // Punto al que queremos llegar cuando se vaya hacia adelante
            
            player.FaceDirection(sideForward, Space.World); // Miramos hacia adelante
            
            //Detectamos si te puedes mover hacia al lado porque todavia no has llegado a la esquina
            if (Physics.Raycast(ledgeSideOrigin, sideForward, rayDistance,
                    player.stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore)) 
                player.lateralVelocity = player.localRight * (inputDirection.x * player.stats.current.ledgeMovementSpeed); // Movemos al jugador lateralmente
            else 
                player.lateralVelocity = Vector3.zero; // Si no detectamos el borde lateral, no nos movemos lateralmente

            var verticalOffset = topHit.point - player.transform.up * (player.height * 0.5f); // Offset vertical para colocar al jugador en el borde
            var lateralOffset = sideForward * (player.radius + topHit.distance); // Offset lateral para colocar al jugador en el borde
            var finalPosition = verticalOffset - lateralOffset - player.center; // Posición final del jugador

            player.transform.position = finalPosition; // Colocamos al jugador en el borde
            
            // Si el jugador suelta el botón de agarrarse al borde, cambiamos de estado y miramos hacia el otro lado.
            if(player.inputs.GetReleaseLedgeDown()) 
            {
                player.FaceDirection(-sideForward, Space.World);
                player.states.Change<FallPlayerState>();
            } else if (player.inputs.GetJumpDown()) // Si el jugador salta, cambiamos de estado y saltamos
            {
                player.Jump(player.stats.current.maxJumpHeight);
                player.states.Change<FallPlayerState>();
            } 
            // Si el jugador se mueve hacia adelante, cambiamos de estado y escalamos el borde
            else if (inputDirection.z > 0 && player.stats.current.canClimbLedges &&
                     ((1 << topHit.collider.gameObject.layer) & player.stats.current.ledgeClimbingLayers) != 0 &&
                     player.FitsIntoPosition(climbDestination - player.transform.forward * (player.radius * 0.5f))) 
            {
                keepParent = true;
                player.states.Change<LedgeClimbingPlayerState>();
            }
        }

        public override void OnContact(Player player, Collider other){}
        protected virtual IEnumerator ClearParentRoutine(Player player)
        {
            if(keepParent) yield break;
            yield return new WaitForSeconds(clearParentDelay); // Esperamos un tiempo para desengancharnos
            player.transform.parent = player.initialParent; // Volvemos a poner el padre inicial
        }
    }
}

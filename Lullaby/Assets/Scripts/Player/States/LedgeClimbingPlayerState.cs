using UnityEngine;
using System.Collections;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;

namespace Lullaby.Entities.States
{
    [AddComponentMenu("Lullaby/CustomMovement/Player/States/Ledge Climbing Player State")]
    public class LedgeClimbingPlayerState: PlayerState
    {
        protected IEnumerator _routine;
        protected GameObject _skinClimbSlot;
        
        protected override void OnEnter(Player player)
        {
            player.playerEvents.OnLedgeClimbing.Invoke();
            
            if (!_skinClimbSlot)
                _skinClimbSlot = new GameObject();

            _skinClimbSlot.transform.position = player.transform.position; // Asignamos la posicion del player a la del skin
            _skinClimbSlot.transform.rotation = player.transform.rotation; // Asignamos la rotacion del player a la del skin
            _skinClimbSlot.transform.parent = player.transform.parent; // Asignamos el padre del player al del skin
 
            player.velocity = Vector3.zero; 
            player.SetSkinParent(_skinClimbSlot.transform); // Asignamos el padre del skin al del player
            _routine = SetPositionRoutine(player); 
            player.StartCoroutine(_routine); // Iniciamos la corrutina desde el script player
        }

        protected override void OnExit(Player player)
        {
            player.ResetSkinParent();
            player.StopCoroutine(_routine); // Paramos la corrutina desde el script player
        }
        public override void OnStep(Player player){}
        
        protected virtual IEnumerator SetPositionRoutine(Player player)
        {
            var elapsedTime = 0f;
            var totalDuration = player.stats.current.ledgeClimbingDuration;
            var halfDuration = totalDuration / 2f;
            
            var initialPosition = player.transform.localPosition;
            // Calculamos la posicion vertical a la que queremos mover el player
            var targetVerticalPosition = player.transform.position +
                                         player.transform.up * (player.height + Physics.defaultContactOffset); 
            // Calculamos la posicion lateral a la que queremos mover el player
            var targetLateralPosition = targetVerticalPosition + player.transform.forward * (player.radius * 2f); 
            
            if (player.transform.parent != player.initialParent) // Si el padre del player no es el inicial
            {
                // Calculamos la posicion vertical y lateral a la que queremos mover el player si el padre no es el inicial pasando de coordenadas globales a locales
                targetVerticalPosition = player.transform.parent.InverseTransformPoint(targetVerticalPosition);
                targetLateralPosition = player.transform.parent.InverseTransformPoint(targetLateralPosition);
            }

            player.skin.position += player.transform.rotation * player.stats.current.ledgeClimbingSkinOffset; // Movemos el skin a la posicion de escala del edge

            while (elapsedTime <= halfDuration)
            {
                elapsedTime += Time.deltaTime;
                player.transform.localPosition = Vector3.Lerp(initialPosition, targetVerticalPosition, elapsedTime / halfDuration);
                yield return null;
            }
            
            elapsedTime = 0f;
            player.transform.localPosition = targetVerticalPosition; // Asignamos la posicion vertical final al player

            while (elapsedTime <= halfDuration)
            {
                elapsedTime += Time.deltaTime;
                player.transform.localPosition = Vector3.Lerp(targetVerticalPosition, targetLateralPosition, elapsedTime / halfDuration);
                yield return null;
            }
            
            player.transform.localPosition = targetLateralPosition; // Asignamos la posicion lateral final al player
            player.states.Change<IdlePlayerState>(); 
        }


        public override void OnContact(Player player, Collider other)
        {
            var direction = player.transform.forward;
            // Empujamos a la entidad con la que colisionamos en la direccion del player
            player.PushRigidbody(other, direction *  player.stats.current.pushForce); 
        }
    }
}
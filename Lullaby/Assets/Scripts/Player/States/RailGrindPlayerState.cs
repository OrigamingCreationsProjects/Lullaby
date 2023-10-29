using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Lullaby.Entities.States
{
    public class RailGrindPlayerState : PlayerState
    {
        protected bool backwards;
        protected float speed;
        protected float lastDashTime;
        
        protected override void OnEnter(Player player)
        {
           // Colocamos al jugador en la posicion mas cercana a la curva de donde ha entrado
           Evaluate(player, out var point, out var forward, out var upward, out _);
           UpdatePosition(player, point, upward);
           
           backwards = Vector3.Dot(player.transform.forward, forward) < 0; // Comprobamos si el jugador va hacia delante o hacia atras
           speed = Mathf.Max(player.lateralVelocity.magnitude, player.stats.current.minGrindInitialSpeed);
           // COMPROBAR LUEGO SI PASA ALGO NO SETEANDO LA VELOCIDAD A 0
           player.velocity = Vector3.zero;
           player.UseCustomCollision(player.stats.current.useCustomCollision);
        }

        protected override void OnExit(Player player)
        {
            ResetRotation(player);
            player.ExitRail();
            player.UseCustomCollision(false);
        }

        public override void OnStep(Player player)
        {
            player.Jump();

            if (player.onRails)
            {
                Evaluate(player, out var point, out var forward, out var upward, out var t);
                
                var direction = backwards ? -forward : forward;
                // Calculamos el dot product entre el vector up y la direccion en la que se mueve el jugador
                // esto nos dice si se mueve hacia arriba o hacia abajo
                var factor = Vector3.Dot(Vector3.up, direction); 
                var multiplier = factor <= 0
                    ? player.stats.current.slopeDownwardForce
                    : player.stats.current.slopeUpwardForce; 
                
                HandleDeceleration(player);
                HandleDash(player);

                if (player.stats.current.applyGrindingSlopeFactor)
                    speed -= factor * multiplier * Time.deltaTime;

                speed = Mathf.Clamp(speed, 
                    player.stats.current.minGrindSpeed, 
                    player.stats.current.grindTopSpeed);
                
                Rotate(player, direction, upward); // Rotamos al jugador para que este recto respecto a la curva
                player.velocity = direction * speed; // Movemos al jugador en la direccion en la que se mueve la curva
                
                // Comprobamos si la curva esta cerrada o si el jugador esta en un punto en el que no se sale de la curva
                if(player.rails.Spline.Closed || (t > 0 && t < 0.9f)) 
                    UpdatePosition(player, point, upward);
            }
            else
            {
                player.states.Change<FallPlayerState>();
            }
        }

        public override void OnContact(Player player, Collider other) { }

        /// <summary>
        /// Evaluates the player's position on the curve and the direction the player will move
        /// </summary>
        protected virtual void Evaluate(Player player, out Vector3 point,
            out Vector3 forward, out Vector3 upward, out float t)
        {
            // Transformamos la posicion del jugador a  coordenadas relativas a la posicion del spline
            var origin = player.rails.transform.InverseTransformPoint(player.transform.position); 

            SplineUtility.GetNearestPoint(player.rails.Spline, origin, out var nearest, out t);

            point = player.rails.transform.TransformPoint(nearest);
            // Sacamos la direccion en la que se movera el objeto por la curva evaluando la el angulo desde el eje x a la tangente en el punto
            forward = Vector3.Normalize(player.rails.EvaluateTangent(t));
            upward = Vector3.Normalize(player.rails.EvaluateUpVector(t));
        }
        
        /// <summary>
        /// Update the player position on the rail
        /// </summary>
        /// <param name="point">Neearest point on the rail from the player position</param>
        /// <param name="upward">Upward player's vector relative to the curve</param>
        protected virtual void UpdatePosition(Player player, Vector3 point, Vector3 upward) =>
            player.transform.position = point + upward * GetDistanceToRail(player);

        /// <summary>
        /// Returns the vertical distance from the player's center to the rail
        /// </summary>
        protected virtual float GetDistanceToRail(Player player) =>
            player.originalHeight * 0.5f + player.stats.current.grindRadiusOffset;
        
        protected virtual void HandleDeceleration(Player player)
        {
            if(player.stats.current.canGrindBrake && player.inputs.GetGrindBrake()) // Comprobamos si puede frenar
            {
                var decelerationDelta = player.stats.current.grindBrakeDeceleration * Time.deltaTime; // Máximo de frenada por frame
                speed = Mathf.MoveTowards(speed, 0, decelerationDelta); //Desaceleramos pasando la velocidad a 0 con un maximo por frame
            }
        }

        protected virtual void HandleDash(Player player)
        {
            if (player.stats.current.canGrindDash &&
                player.inputs.GetDashDown() &&
                Time.time >= lastDashTime + player.stats.current.grindDashCoolDown)
            {
                lastDashTime = Time.time;
                speed = player.stats.current.grindDashForce;
                player.playerEvents.OnDashStarted.Invoke();
            }
        }

        protected virtual void Rotate(Player player, Vector3 forward, Vector3 upward)
        {
            if (forward != Vector3.zero) // Comprobamos que el jugador no este parado
                player.transform.rotation = Quaternion
                    .LookRotation(forward, player.transform.up); // Rotamos el jugador hacia la direccion en la que se mueve
            // Rotamos el jugador para que este recto respecto a la curva
            player.transform.rotation = Quaternion
                .FromToRotation(player.transform.up, upward) * player.transform.rotation; 
        }

        protected virtual void ResetRotation(Player player)
        {
            // Calculamos la rotacion que hay que aplicar al jugador para que este recto respecto al mundo
            var rotation = Quaternion.FromToRotation(player.transform.up, Vector3.up); 
            player.transform.rotation = rotation * player.transform.rotation; // Aplicamos la rotacion
        }
        
        
    }
}
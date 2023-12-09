using DG.Tweening;
using UnityEngine;

namespace Lullaby.Entities.Enemies.States
{
    public class SurpriseEnemyState : EnemyState
    {
        protected override void OnEnter(Enemy enemy)
        {
            enemy.velocity = Vector3.zero;
            var head = enemy.player.position - enemy.position; // Direction to player
            var upOffset = Vector3.Dot(enemy.transform.up, head); // Sacamos la direccion a la que mirar manteniendo nuestro eje Y
            var direction = head - enemy.transform.up * upOffset; // Direction to player without up offset
            var localDirection = Quaternion.FromToRotation(enemy.transform.up, Vector3.up) * direction; // Direction to player without up offset in local space
            
            localDirection = localDirection.normalized; // Normalizamos porque solo nos interesa la direccion
            enemy.FaceDirection(localDirection);
            Sequence s = DOTween.Sequence();
            s.AppendCallback(() => enemy.GetComponent<FaceChanger>().ChangeFobosExpression(FobosEmotion.Surprised));
            s.InsertCallback(0f , () => enemy.ApplySurpriseForces());
            //s.AppendInterval(0.5f);
            //s.AppendCallback(() => enemy.GetComponent<FaceChanger>().ChangeFobosExpression(FobosEmotion.Angry));
            s.AppendCallback(() => enemy.states.Change<FollowEnemyState>());
            

        }

        protected override void OnExit(Enemy enemy){}

        public override void OnStep(Enemy enemy)
        {
            enemy.ApplyGravity();
            // enemy.SnapToGround();
            
            // var head = enemy.player.position - enemy.position; // Direction to player
            // var upOffset = Vector3.Dot(enemy.transform.up, head); // Sacamos la direccion a la que mirar manteniendo nuestro eje Y
            // var direction = head - enemy.transform.up * upOffset; // Direction to player without up offset
            // var localDirection = Quaternion.FromToRotation(enemy.transform.up, Vector3.up) * direction; // Direction to player without up offset in local space
            //
            // localDirection = localDirection.normalized; // Normalizamos porque solo nos interesa la direccion
            // // porque si no al usar el dato sera mas grande cuanto mas lejos este
            //
            // enemy.FaceDirectionSmooth(localDirection);
        }

        public override void OnContact(Enemy enemy, Collider other)
        {
            
        }
    }
}
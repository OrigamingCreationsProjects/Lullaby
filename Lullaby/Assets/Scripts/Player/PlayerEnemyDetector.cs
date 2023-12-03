using Lullaby.Entities.Enemies;
using UnityEngine;

namespace Lullaby.Entities
{
    [AddComponentMenu("Lullaby/Combat System/Player Enemy Detector")]
    public class PlayerEnemyDetector : MonoBehaviour
    {
        public LayerMask targetLayerMask;
        [SerializeField] private Enemy currentTarget;
        [SerializeField] private Vector3 inputDirection;
        
        protected Player _player;
        private PlayerWeapon _playerWeapon;
        private void Start()
        {
            _player = GetComponentInParent<Player>();
            _playerWeapon = GetComponentInParent<PlayerWeapon>();
        }

        private void Update()
        {
            // var camera = Camera.main;
            // var forward = camera.transform.forward;
            // var right = camera.transform.right;
            //
            // forward.y = 0f;
            // right.y = 0f;
            //
            // forward.Normalize();
            // right.Normalize();
            
        }

        public Enemy CurrentTarget()
        {
            return currentTarget;
        }
        
        public void SetCurrentTarget(Enemy target)
        {
            currentTarget = target;
        }

        public float GetInputMagnitude()
        {
            return _player.inputs.GetMovementDirection().magnitude;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, inputDirection);
            Gizmos.DrawSphere(transform.position + inputDirection, .2f);
            Gizmos.DrawWireSphere(transform.position, 1);
            if (CurrentTarget() != null)
                Gizmos.DrawSphere(CurrentTarget().stepPosition, 0.5f);
        }
    }
}
using UnityEngine;

namespace Lullaby.Entities.NPC
{
    [AddComponentMenu("Lullaby/Entities/Talker/Look At Nearby")]
    public class LookAtNearby : MonoBehaviour
    {
        public Transform headTransform;
        public Transform aimTargetTransform;
        public PointOfInterest pointOfInterest;

        public Vector3 origin;
        public float visionRadius;
        public float lerpSpeed;
        
        protected Collider[] sightOverlaps = new Collider[32];

        
        private void Start()
        {
            origin = aimTargetTransform.position;
        }
        
        private void Update()
        {
            var overlaps =  Physics.OverlapSphereNonAlloc(headTransform.position + transform.forward, visionRadius, sightOverlaps);
            
            pointOfInterest = null;

            for (int i = 0; i < overlaps; i++)
            {
                if (sightOverlaps[i] != null)
                {
                    if (sightOverlaps[i].TryGetComponent<PointOfInterest>(out var poi))
                    {
                        pointOfInterest = poi;
                        break;
                    }
                }
            }

            Vector3 targetPosition;

            if (pointOfInterest != null)
            {
                targetPosition = pointOfInterest.GetLookTarget().position;
            }
            else
            {
                targetPosition = origin;
            }

            float speed = lerpSpeed / 10;
            aimTargetTransform.position = Vector3.Lerp(aimTargetTransform.position, targetPosition, Time.deltaTime * speed);
            

        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green * 0.5f;
            Gizmos.DrawWireSphere(headTransform.position + transform.forward, visionRadius);
        }
        
    }
}
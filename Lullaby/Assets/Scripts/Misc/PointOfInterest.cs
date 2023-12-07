using UnityEngine;

namespace Lullaby
{
    public class PointOfInterest : MonoBehaviour
    {
        public Transform lookTarget;
        
        public Transform GetLookTarget()
        {
            if (lookTarget != null)
            {
                return lookTarget;
            }
            else
            {
                return transform;
            }
        }
    }
}
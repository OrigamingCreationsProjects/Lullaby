using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lullaby.Entities.Weapons
{
    [DefaultExecutionOrder(9999)]
    [AddComponentMenu("Lullaby/Custom Movement/Player/Weapons/FixedUpdateFollow")]
    public class FixedUpdateFollow : MonoBehaviour
    {
        public Transform toFollow;

        private void FixedUpdate()
        {
            transform.position = toFollow.position;
            transform.rotation = toFollow.rotation;
        }
    }
}

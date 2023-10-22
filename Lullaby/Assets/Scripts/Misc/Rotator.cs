using System;
using UnityEngine;

namespace Lullaby
{
    [AddComponentMenu("Lullaby/Misc/Rotator")]
    public class Rotator : MonoBehaviour
    {
        public Space space = Space.Self; // Otorgamos esta variable desde el inspector para poder elegir respecto a que coordenadas girar
        public Vector3 turningEulerAngles = new Vector3(0, -180, 0);

        protected void LateUpdate()
        {
            transform.Rotate(turningEulerAngles * Time.deltaTime, space);
        }
    }
}
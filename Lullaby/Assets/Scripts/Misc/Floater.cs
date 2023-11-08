using System;
using UnityEngine;

namespace Lullaby
{
    [AddComponentMenu("Lullaby/Misc/Floater")]
    public class Floater : MonoBehaviour
    {
        public float speed = 2.0f;
        public float amplitude = 0.5f;

        protected void LateUpdate()
        {
            var wave = Mathf.Sin(Time.time * speed) * amplitude;
            transform.position += transform.up * wave * Time.deltaTime;
        }
    }
}
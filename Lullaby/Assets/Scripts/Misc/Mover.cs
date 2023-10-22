using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Esta clase nos servira para objetos a los que aplicaremos un offset bajo ciertas condiciones
namespace Lullaby
{
    [AddComponentMenu("Lullaby/Misc/Mover")]
    public class Mover : MonoBehaviour
    {
        public Vector3 offset;
        public float duration;
        public float resetDuration;

        protected Vector3 initialPosition;

        public virtual void ApplyOffset()
        {
            StopAllCoroutines();
            StartCoroutine(ApplyOffsetRoutine(initialPosition, initialPosition + offset, duration));
        }

        public virtual void ResetMover()
        {
            StopAllCoroutines();
            StartCoroutine(ApplyOffsetRoutine(transform.localPosition, initialPosition, resetDuration));
        }
        
        private IEnumerator ApplyOffsetRoutine(Vector3 from, Vector3 to, float duration)
        {
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                var t = elapsedTime / duration;
                transform.localPosition = Vector3.Lerp(from, to, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = to;
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            initialPosition = transform.localPosition;
        }
    }
}
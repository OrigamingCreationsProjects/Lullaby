using System.Collections;
using UnityEngine;

namespace Lullaby
{
    [RequireComponent(typeof(Health))]
    [AddComponentMenu("Lullaby/Misc/Hit Flash")]
    public class HitFlash : MonoBehaviour
    {
        [Header("Flash Settings")]
        public MeshRenderer[] renderers;
        public Color flashColor = Color.red;
        public float flashDuration = 0.5f;

        protected Health _health;

        public virtual void Flash()
        {
            StopAllCoroutines();

            foreach (var renderer in renderers)
            {
                StartCoroutine(FlashRoutine(renderer.material));
            }
        }

        protected virtual IEnumerator FlashRoutine(Material material)
        {
            var elapsedTime = 0f;
            var flashColor = this.flashColor;
            var initialColor = material.color;

            while (elapsedTime < flashDuration)
            {
                elapsedTime += Time.deltaTime;
                material.color = Color.Lerp(flashColor, initialColor, elapsedTime / flashDuration);
                yield return null;
            }

            material.color = initialColor;
        }

        protected virtual void Start()
        {
            _health = GetComponent<Health>();
            _health.onDamage.AddListener(Flash);
        }
    }
}
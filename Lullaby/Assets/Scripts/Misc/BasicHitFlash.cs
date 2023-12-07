using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace Lullaby
{
    [RequireComponent(typeof(Health))]
    [AddComponentMenu("Lullaby/Misc/Basic Hit Flash")]
    public class BasicHitFlash : MonoBehaviour
    {
        [Header("Flash Settings")]
        public MeshRenderer[] renderers;
        public Color flashColor = Color.red;
        public float flashDuration = 0.5f;
        public MaterialType materialType = MaterialType.Unlit;
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
            var localFlashColor = this.flashColor;
            bool isSpecial = materialType != MaterialType.Unlit;
            string propertyName = materialType == MaterialType.Toon ? "_Tint" : "_Primary_Color";
            var initialColor = isSpecial? material.GetColor(propertyName) : material.color;

            if (!isSpecial)
            {
                while (elapsedTime < flashDuration)
                {
                    elapsedTime += Time.deltaTime;
                    material.color = Color.Lerp(localFlashColor, initialColor, elapsedTime / flashDuration);
                    yield return null;
                }
            }
            else
            {
                while (elapsedTime < flashDuration)
                {
                    elapsedTime += Time.deltaTime;
                    material.SetColor(propertyName, Color.Lerp(localFlashColor, initialColor, elapsedTime / flashDuration)); 
                    yield return null;
                }
            }

            material.color = initialColor;
        }

        protected virtual void Start()
        {
            _health = GetComponent<Health>();
            _health.onDamage.AddListener(Flash);
        }
    }
    public enum MaterialType
    {
        Unlit,
        Toon, 
        Fobos
    }
}
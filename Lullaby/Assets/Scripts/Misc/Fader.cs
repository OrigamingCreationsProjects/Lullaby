using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Lullaby
{
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("Lullaby/Misc/Fader")]
    public class Fader : Singleton<Fader>
    {
        public float speed = 1f;

        protected Image _image;

        /// <summary>
        /// Fades out with no callback.
        /// </summary>
        public void FadeOut() => FadeOut(() => { });
        
        /// <summary>
        /// Fades in with no callback.
        /// </summary>
        public void FadeIn() => FadeIn(() => { });
        
        /// <summary>
        /// Fades in with callback.
        /// </summary>
        /// <param name="onFinished">The action that will be invoked in the end of the routine.</param>
        public void FadeOut(Action onFinished)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOutRoutine(onFinished));
        }
        
        /// <summary>
        /// Fades in with callback.
        /// </summary>
        /// <param name="onFinished">The action that will be invoked in the end of the routine.</param>
        public void FadeIn(Action onFinished)
        {
            StopAllCoroutines();
            StartCoroutine(FadeInRoutine(onFinished));
        }

        /// <summary>
        /// Set the fader alpha to a given value.
        /// </summary>
        /// <param name="alpha">The desired alpha value</param>
        public void SetAlpha(float alpha)
        {
            var color = _image.color;
            color.a = alpha;
            _image.color = color;
        }

        /// <summary>
        /// /// Increases the alpha to one and invokes the callback afterwards.
        /// </summary>
        protected virtual IEnumerator FadeOutRoutine(Action onFinished)
        {
            while (_image.color.a < 1)
            {
                var color = _image.color;
                color.a += speed * Time.deltaTime;
                _image.color = color;
                yield return null;
            }
            onFinished?.Invoke();
        }
        
        /// <summary>
        /// Decreases the alpha to zero and invokes the callback afterwards.
        /// </summary>
        protected virtual IEnumerator FadeInRoutine(Action onFinished)
        {
            while (_image.color.a > 0)
            {
                var color = _image.color;
                color.a -= speed * Time.deltaTime;
                _image.color = color;
                yield return null;
            }
            onFinished?.Invoke();
        }


        protected override void Awake()
        {
            base.Awake();
            _image = GetComponent<Image>();
        }
        
    }
}
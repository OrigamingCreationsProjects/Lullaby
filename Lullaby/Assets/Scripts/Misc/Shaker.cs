using System;
using DG.Tweening;
using UnityEngine;

namespace Lullaby
{
    [AddComponentMenu("Lullaby/Misc/Shaker")]
    
    public class Shaker : MonoBehaviour
    {
        [SerializeField] private float duration = 2f;
        [SerializeField] private float strength = 5f;
        [SerializeField] private int vibrato = 3;
        [SerializeField] private float randomness = 60f;
        private void Start()
        {
            Shake();
        }

        private void Shake()
        {
            transform.DOShakePosition(duration, strength, vibrato, randomness, false, false,
                ShakeRandomnessMode.Harmonic).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        }
    }
}
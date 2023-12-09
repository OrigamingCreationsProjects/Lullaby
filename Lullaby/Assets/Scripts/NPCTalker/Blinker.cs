using System.Collections;
using UnityEngine;

namespace NPCTalker
{
    public class Blinker : MonoBehaviour
    {
        public float _minBlinkTime = 2f;
        public float _maxBlinkTime = 5f;
        private Animator _animator;
        private string _blinkTriggerName = "Blink";
        private int _blinkTriggerHash;
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _blinkTriggerHash = Animator.StringToHash(_blinkTriggerName);
        }
        
        private void Start()
        {
            StartCoroutine(BlinkRoutine());
        }
        
        IEnumerator BlinkRoutine()
        {
            while (true)
            {
                _animator.SetTrigger(_blinkTriggerHash);
                yield return new WaitForSeconds(Random.Range(_minBlinkTime, _maxBlinkTime));
            }
        }
    }
}
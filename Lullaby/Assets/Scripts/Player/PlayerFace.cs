using System.Collections;
using Lullaby.LevelManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lullaby.Entities
{
    public class PlayerFace : MonoBehaviour
    {
        [Header("Parameters Names")] 
        public string blinkNeutralName = "Blink_Neutral";
        public string blinkHappyName = "Blink_Happy";
        public string blinkSadName = "Blink_Sad";
        public string blinkAngryName = "Blink_Angry";
        public string mouthSleepyName = "Mouth_Sleepy";
        public string mouthSingingName = "Mouth_Singing";
        [Header("Blink Parameters")]
        public float _minBlinkTime = 2f;
        public float _maxBlinkTime = 5f;
        
        public Animator animator;
        
        [HideInInspector] public int blinkNeutralHash;
        protected int _blinkHappyHash;
        protected int _blinkSadHash;
        protected int _blinkAngryHash;
        protected int _mouthSleepyHash;
        protected int _mouthSingingHash;

        protected int _currentBlinkHash;
        
        protected int[] _randomIdleTriggers;
        
        protected Player _player;
        
        protected virtual void InitializeParametersHash()
        {
            blinkNeutralHash = Animator.StringToHash(blinkNeutralName);
            _blinkHappyHash = Animator.StringToHash(blinkHappyName);
            _blinkSadHash = Animator.StringToHash(blinkSadName);
            _blinkAngryHash = Animator.StringToHash(blinkAngryName);
            _mouthSleepyHash = Animator.StringToHash(mouthSleepyName);
            _mouthSingingHash = Animator.StringToHash(mouthSingingName);

            _randomIdleTriggers = new[] { _mouthSleepyHash, _mouthSingingHash };
        }

        protected virtual void InitializePlayer()
        {
            _player = GetComponentInParent<Player>();
        }
        
        protected virtual void InitializeAnimatorTriggers()
        {
            _player.playerEvents.OnRandomIdleEnter.AddListener(MouthTrigger);
            _player.playerEvents.OnAttackStarted.AddListener(() => UpdateCurrentBlink(_blinkAngryHash));
            _player.playerEvents.OnAttackFinished.AddListener(() => UpdateCurrentBlink(blinkNeutralHash));
            _player.playerEvents.OnHurt.AddListener(() => UpdateCurrentBlink(_blinkSadHash));
            _player.playerEvents.OnDie.AddListener(() => UpdateCurrentBlink(_blinkSadHash));
            _player.playerEvents.OnDie.AddListener(() => StopAllCoroutines());
            LevelRespawner.instance.OnRespawn.AddListener(() => StartCoroutine(BlinkRoutine()));
        }
        protected virtual void MouthTrigger(int x)
        {
            animator.SetTrigger(_randomIdleTriggers[x]);
            if (x == 1)
                UpdateCurrentBlink(_blinkHappyHash);
            else if(x == 0)
                UpdateCurrentBlink(_blinkSadHash);
        }

        public virtual void UpdateCurrentBlink(int newBlinkHash)
        {
            _currentBlinkHash = newBlinkHash;
            animator.SetTrigger(_currentBlinkHash);
        }

        public virtual void SetCurrentBlink(int newBlinkHash)
        {
            _currentBlinkHash = newBlinkHash;
        }
        
        IEnumerator BlinkRoutine()
        {
            while (true)
            {
                animator.SetTrigger(_currentBlinkHash);
                yield return new WaitForSeconds(Random.Range(_minBlinkTime, _maxBlinkTime));
            }
        }
        
        protected virtual void Start()
        {
            InitializePlayer();
            InitializeParametersHash();
            InitializeAnimatorTriggers();
            _currentBlinkHash = blinkNeutralHash;
            StartCoroutine(BlinkRoutine());
        }
        
        
    }
}
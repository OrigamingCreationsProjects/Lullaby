using UnityEngine;

namespace Lullaby.Entities.NPC
{
    public class Speaker : MonoBehaviour
    {
        private Animator _animator;
        private string _speakTriggerName = "Speak";
        private int _speakTriggerHash;
        private Talker _talker;
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _speakTriggerHash = Animator.StringToHash(_speakTriggerName);
            _talker = GetComponentInParent<Talker>();
        }
        
        private void Start()
        {
            //_talker.talkerEvents.OnDialogueBark.AddListener(Speak);
        }
        
        public void Speak(AudioClip a = null)
        {
            _animator.SetTrigger(_speakTriggerHash);
        }
    }
}
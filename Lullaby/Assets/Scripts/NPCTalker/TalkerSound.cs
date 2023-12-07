using Lullaby.LevelManagement;
using UnityEngine;

namespace Lullaby.Entities.NPC
{
    [RequireComponent(typeof(Talker))]
    [AddComponentMenu("Lullaby/Entities/Talker/Talker Sound")]
    public class TalkerSound : MonoBehaviour
    {
        protected Talker _talker;
        protected AudioSource _audioSource;
        
        protected virtual void InitializeTalker() => _talker = GetComponent<Talker>();

        protected virtual void InitializeAudio()
        {
            if (!TryGetComponent(out _audioSource))
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                
            }
        }
        protected virtual void Play(AudioClip audio, bool stopPrevious = true)
        {
            if(audio == null) 
                return;
            
            if(stopPrevious)
                _audioSource.Stop();
            _audioSource.clip = audio;
            _audioSource.PlayOneShot(audio);
        }

        protected virtual void InitializeCallbacks()
        {
            _talker.talkerEvents.OnDialogueBark.AddListener((x) => Play(x));

            LevelPauser.instance?.OnPaused.AddListener(() =>
            {
                _audioSource.Pause();
            });
            LevelPauser.instance?.OnUnpaused.AddListener(() =>
            {
                _audioSource.UnPause();
            });
        }
        
        protected void Start()
        {
            InitializeAudio();
            InitializeTalker();
            InitializeCallbacks();
        }
    }
}
using System.Collections.Generic;
using Lullaby.Entities.States;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lullaby.Entities
{
    [RequireComponent(typeof(Player))]
    [AddComponentMenu("Lullaby/Entities/Player/Player Footsteps")]
    public class PlayerFootsteps : MonoBehaviour
    {
        [System.Serializable]
        public class Surface
        {
            public string tag;
            public AudioClip[] footsteps;
            public AudioClip[] landings;
        }
        
        public Surface[] surfaces;
        public AudioClip[] defaultFootsteps;
        public AudioClip[] defaultLandings;

        [Header("General Settings")] 
        public float stepDistanceOffset = 1.25f;
        public float groundStepVolume = 0.5f;

        protected Vector3 _lastLateralPosition;
        // Almacena los sonidos de pasos y aterrizaje en un diccionario con arrays
        // para acceder a ellos en funcion de cada superficie (que se identifica por la string tag, osea la tag asignada)
        protected Dictionary<string, AudioClip[]> _footsteps = new Dictionary<string, AudioClip[]>();
        protected Dictionary<string, AudioClip[]> _landings = new Dictionary<string, AudioClip[]>();

        protected Player _player;
        protected AudioSource _audioSource;
        
        protected virtual void PlayRandomClip(AudioClip[] clips)
        {
            if (clips.Length > 0)
            {
                var index = Random.Range(0, clips.Length);
                _audioSource.PlayOneShot(clips[index], groundStepVolume);
            }
        }

        protected virtual void Landing()
        {
            //Si el jugador aterrizara en superficie diferente especial como agua u otra cosa hay que comprobarlo para
            //que esto no se haga

            if (_landings.ContainsKey(_player.groundHit.collider.tag))
            {
                PlayRandomClip(_landings[_player.groundHit.collider.tag]);
            }
            else
            {
                PlayRandomClip(defaultLandings);
            }
        }

        protected virtual void Start()
        {
            _player = GetComponent<Player>();
            _player.entityEvents.OnGroundEnter.AddListener(Landing);

            if (!TryGetComponent(out _audioSource))
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }

            foreach (var surface in surfaces)
            {
                _footsteps.Add(surface.tag, surface.footsteps);
                _landings.Add(surface.tag, surface.footsteps);
            }
        }

        protected virtual void Update()
        {
            if(!_player.isGrounded || !_player.states.IsCurrentOfType(typeof(WalkPlayerState)))
                return;

            var currentPosition = transform.position;
            var distance = (_lastLateralPosition - currentPosition).magnitude;
            
            if(distance < stepDistanceOffset) 
                return;

            if (_footsteps.ContainsKey(_player.groundHit.collider.tag))
            {
                PlayRandomClip(_footsteps[_player.groundHit.collider.tag]);   
            }
            else
            {
                PlayRandomClip(defaultFootsteps);
            }

            _lastLateralPosition = currentPosition;
        }
    }
}
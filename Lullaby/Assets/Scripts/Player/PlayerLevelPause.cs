using System;
using Lullaby.LevelManagement;
using UnityEngine;

namespace Lullaby.Entities
{
    [RequireComponent(typeof(Player))]
    [AddComponentMenu("Lullaby/Entities/Player Level Pause")]
    public class PlayerLevelPause : MonoBehaviour
    {
        protected Player _player;
        protected LevelPauser _pauser;

        protected virtual void Start()
        {
            _player = GetComponent<Player>();
            _pauser = LevelPauser.instance;
        }

        protected virtual void Update()
        {
            if (_player.inputs.GetPauseDown())
            {
                Debug.Log("Pause button pressed");
                var value = _pauser.paused;
                _pauser.Pause(!value);
            }
        }
    }
}
using System;
using UnityEngine.Events;

namespace Lullaby.Entities.Events
{
    [Serializable]
    public class TalkerEvents
    {
        /// <summary>
        /// Called when the Player enters this Talker sight.
        /// </summary>
        public UnityEvent OnPlayerDetected;
        
        /// <summary>
        /// Called when the Player leaves this Talker sight.
        /// </summary>
        public UnityEvent OnPlayerGone;

        /// <summary>
        /// Called when this Talker touches the Player.
        /// </summary>
        public UnityEvent OnPlayerContact;

        /// <summary>
        /// Called when this Talker starts a dialogue with the Player.
        /// </summary>
        public UnityEvent OnDialogueStarted;
        
        /// <summary>
        /// Called when this Talker finish a dialogue with the Player.
        /// </summary>
        public UnityEvent OnDialogueFinished;
    }
}
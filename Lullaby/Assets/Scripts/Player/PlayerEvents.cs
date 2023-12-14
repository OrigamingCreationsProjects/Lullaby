using System;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Lullaby.Entities.Events
{
    [Serializable]
    public class PlayerEvents
    {
        /// <summary>
		/// Called when the Player jumps.
		/// </summary>
		public UnityEvent OnJump;

        /// <summary>
        /// Called when the Player starts dashing.
        /// </summary>
        public UnityEvent OnDashStarted;

        /// <summary>
        /// Called when the Player finishes dashing.
        /// </summary>
        public UnityEvent OnDashEnded;
        
		/// <summary>
		/// Called when the Player gets damage.
		/// </summary>
		public UnityEvent OnHurt;

		/// <summary>
		/// Called when the Player died.
		/// </summary>
		public UnityEvent OnDie;

		/// <summary>
		/// Called when the Player uses the regular Attack.
		/// </summary>
		public UnityEvent OnAttackStarted;
		
		/// <summary>
		/// Called when the Player finish the regular Attack.
		/// </summary>
		public UnityEvent OnAttackFinished;
		
		/// <summary>
		/// Called when the Player start a Dialogue.
		/// </summary>
		public UnityEvent OnDialogueStarted;
		
		/// <summary>
		/// Called when the Player finish a Dialogue.
		/// </summary>
		public UnityEvent OnDialogueFinished;
		
		// /// <summary>
		// /// Called when the Player pick up an object.
		// /// </summary>
		public UnityEvent OnPickUp;
		
		// /// <summary>
		// /// Called when the Player throws an object.
		// /// </summary>
		public UnityEvent OnThrow;

		/// <summary>
		/// Called when the player grabs onto a ledge.
		/// </summary>
		public UnityEvent OnLedgeGrabbed;

		/// <summary>
		/// Called when the Player climbs a ledge.
		/// </summary>
		public UnityEvent OnLedgeClimbing;

		
		/// <summary>
		/// Called when the Player air dives.
		/// </summary>
		public UnityEvent OnDeadlyFall;

		/// <summary>
		/// Called when we entry on a random idle
		/// </summary>
		public UnityEvent<int> OnRandomIdleEnter;
		
		
		public UnityEvent OnRandomIdleExit;
		

		#region Eventos descartados

		// /// <summary>
		// /// Called when the Player performs a backflip.
		// /// </summary>
		// public UnityEvent OnBackflip;

		// /// <summary>
		// /// Called when the Player started the Stomp Attack.
		// /// </summary>
		// public UnityEvent OnStompStarted;

		// /// <summary>
		// /// Called when the Player starts moving down with Stomp Attack.
		// /// </summary>
		// public UnityEvent OnStompFalling;

		// /// <summary>
		// /// Called when the Player landed from the Stomp Attack.
		// /// </summary>
		// public UnityEvent OnStompLanding;

		// /// <summary>
		// /// Called when the Player finished the Stomp Attack.
		// /// </summary>
		// public UnityEvent OnStompEnding;

		// /// <summary>
		// /// Called when the Player starts gliding.
		// /// </summary>
		// public UnityEvent OnGlidingStart;
		//
		// /// <summary>
		// /// Called when the Player stops glidig.
		// /// </summary>
		// public UnityEvent OnGlidingStop;

		#endregion
    }
}
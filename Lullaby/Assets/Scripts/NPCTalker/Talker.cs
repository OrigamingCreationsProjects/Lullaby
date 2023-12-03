using System;
using Lullaby.Entities;
using Lullaby.Entities.Events;
using UnityEngine;

namespace Lullaby.Entities.NPC
{
    [RequireComponent(typeof(TalkerStatsManager))]
    [RequireComponent(typeof(TalkerStateManager))]
    public class Talker: Entity<Talker>
    {
        public TalkerEvents talkerEvents;
        
        public NPCDialogueScript[] talkersDialogueScripts;
        
        protected Player _player;

        protected Collider[] sightOverlaps = new Collider[1024];
        
        /// <summary>
        /// Returns the Talker Stats Manager instance.
        /// </summary>
        public TalkerStatsManager stats { get; protected set; }
        
        /// <summary>
        ///  Returns the instance of the Player on the Talker sight.
        /// </summary>
        public Player player { get; protected set; }

        public Quaternion originalRotation { get; protected set; }
       
        
        #region -- INITIALIZERS --
        protected virtual void InitializeStatsManager() => stats = GetComponent<TalkerStatsManager>();
        protected virtual void InitializeTag() => tag = GameTags.Talker;
        
        #endregion
        
        public virtual void HandleSight()
        {
            if (!player)
            {
                var overlaps = Physics.OverlapSphereNonAlloc(position, stats.current.spotRange, sightOverlaps); // Detectamos al jugador

                for (int i = 0; i < overlaps; i++)
                {
                    if (sightOverlaps[i].CompareTag(GameTags.Player)) 
                    {
                        if (sightOverlaps[i].TryGetComponent<Player>(out var player))
                        {
                            this.player = player;
                            //talkerEvents.OnDialogueStarted?.Invoke();
                            return;
                        }
                    }
                }
            }
            else
            {
                var distance = Vector3.Distance(position, player.position);

                if ((player.health.current == 0) || (distance > stats.current.viewRange))
                {
                    player = null;
                    talkerEvents.OnPlayerGone?.Invoke();
                }
            }
        }

        protected override void Update()
        {
            HandleStates();
            OnUpdate();
        }

        protected override void OnUpdate()
        {
            HandleSight();
        }
        
        protected override void Awake()
        {
            base.Awake();
            InitializeTag();
            InitializeStatsManager();
            // TEMPORAL
            talkerEvents.OnDialogueStarted.AddListener(() => Debug.Log("Dialogo Empezado"));
            originalRotation = new Quaternion(
                transform.localRotation.x, 
                transform.localRotation.y, 
                transform.localRotation.z, 
                transform.localRotation.w);
        }
        
        protected void OnTriggerStay(Collider other)
        {
            // if (other.TryGetComponent(out Player player))
            // {
            //     this.player = player;
            //     if (this.player.inputs.GetInteractDown())
            //     {
            //         talkerEvents.OnDialogueStarted?.Invoke();
            //     }
            // }
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Lullaby.Entities.Events
{
    [Serializable]
    public class BossEnemyEvents : EnemyEvents
    {
        public UnityEvent OnPreparedToAttack;
        public UnityEvent OnDivisonFinished;
        public UnityEvent OnAttackPerformed;
        public UnityEvent OnSecondStage;
        public UnityEvent OnFinalStage;
        

        public event EventHandler<OnValueChange> OnRetreat;
        public event EventHandler<OnValueChange> OnAttack;
        public event EventHandler<OnValueChange> OnPlayerSeen;
        public event EventHandler OnSecondStageReached;
        public event EventHandler OnFinalStageReached; 
        public void HandleAttack(bool boolean)
        {
            OnAttack?.Invoke(this,new OnValueChange(){value = boolean});
        }

        public void HandleRetreat(bool boolean)
        {
            OnRetreat?.Invoke(this, new OnValueChange(){value = boolean});
        }

        public void HandlePlayerSeen(bool boolean)
        {  
            Debug.Log("HANDLE PLAYER SEEN");
            OnPlayerSeen?.Invoke(this, new OnValueChange(){value = boolean});
        }

        public void HandleSecondStage()
        {
            OnSecondStageReached?.Invoke(this, EventArgs.Empty);
        }

        public void HandleFinalStage()
        {
            OnFinalStageReached?.Invoke(this, EventArgs.Empty);
        }
    }

    public class OnValueChange : EventArgs
    {
        public bool value;
    }
    
}


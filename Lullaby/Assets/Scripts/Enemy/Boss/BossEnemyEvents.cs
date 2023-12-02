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

        public delegate void OnRetreatAction();

        public event EventHandler<OnValueChange> OnRetreat;
        public event EventHandler<OnValueChange> OnAttack;

        public void HandleAttack(bool boolean)
        {
            OnAttack?.Invoke(this,new OnValueChange(){value = boolean});
        }

        public void HandleRetreat(bool boolean)
        {
            OnRetreat?.Invoke(this, new OnValueChange(){value = boolean});
        }
    }

    public class OnValueChange : EventArgs
    {
        public bool value;
    }
    
}


using System;
using UnityEngine.Events;

namespace Lullaby.Entities.Enemies
{
    [Serializable]
    public class DollyEvents
    {
        public UnityEvent<Dolly> OnDamage;
        
        public UnityEvent<Dolly> OnStopMoving;
        
        public UnityEvent<Dolly> OnStartMoving;
        
    }
}
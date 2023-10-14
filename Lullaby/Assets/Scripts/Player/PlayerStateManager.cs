using System.Collections.Generic;
using Lullaby.Entities;
using UnityEngine;

namespace Lullaby.Entities.States
{
    [AddComponentMenu("Lullaby/Custom Movement/Player/Player State Manager")]
    public class PlayerStateManager : EntityStateManager<Player>
    {
        [ClassTypeName(typeof(PlayerState))]
        public string[] states;
        protected override List<EntityState<Player>> GetStateList()
        {
            return PlayerState.CreateListFromStringArray(states);
        }
    }
}
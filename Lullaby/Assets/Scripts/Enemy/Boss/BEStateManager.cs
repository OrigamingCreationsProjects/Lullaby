using System.Collections.Generic;
using Lullaby.Entities;
using Unity.VisualScripting;

namespace Lullaby.Entities.Enemies
{
    public class BEStateManager : EntityStateManager<BossEnemy>
    {
        [ClassTypeName(typeof(BEState))] 
        public string[] states;
        protected override List<EntityState<BossEnemy>> GetStateList()
        {
            return BEState.CreateListFromStringArray(states);
        }
    }   
}

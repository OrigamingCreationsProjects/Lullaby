using System.Collections.Generic;

namespace Lullaby.Entities.NPC
{
    public class TalkerStateManager : EntityStateManager<Talker>
    {
        [ClassTypeName(typeof(TalkerState))] 
        public string[] states;
        
        protected override List<EntityState<Talker>> GetStateList()
        {
            return TalkerState.CreateListFromStringArray(states);
        }
    }
}
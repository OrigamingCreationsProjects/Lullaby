using System.Collections.Generic;
using Lullaby.Entities;

namespace Lullaby.Entities.Enemies
{
    public class EnemyStateManager: EntityStateManager<Enemy>
    {
        [ClassTypeName(typeof(EnemyState))]
        public string[] states;
        protected override List<EntityState<Enemy>> GetStateList()
        {
            return EnemyState.CreateListFromStringArray(states);
        }
    }
}
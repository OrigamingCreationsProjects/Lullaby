using UnityEngine;
namespace Lullaby.Entities.NPC
{
    [CreateAssetMenu(fileName = "NewTalkerStats", menuName = "Character Stats/Talker/New Talker Stats")]
    public class TalkerStats : EntityStats<TalkerStats>
    {
        [Header("View Stats")]
        public float spotRange = 5f;
        public float viewRange = 8f;
    }
}
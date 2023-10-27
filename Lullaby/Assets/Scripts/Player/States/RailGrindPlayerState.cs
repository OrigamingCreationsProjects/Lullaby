using UnityEngine;

namespace Lullaby.Entities.States
{
    public class RailGrindPlayerState : PlayerState
    {
        protected bool backwards;
        protected float speed;
        protected float lastDashTime;
        
        protected override void OnEnter(Player player)
        {
           
        }

        protected override void OnExit(Player player)
        {
            throw new System.NotImplementedException();
        }

        public override void OnStep(Player player)
        {
            throw new System.NotImplementedException();
        }

        public override void OnContact(Player player, Collider other)
        {
            throw new System.NotImplementedException();
        }
        
        //protected virtual void Evaluate(Player player ...
        
    }
}
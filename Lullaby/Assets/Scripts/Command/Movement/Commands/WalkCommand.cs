using Movement.Components;
using UnityEngine;

namespace Movement.Commands
{
    public class WalkCommand: AMovementCommand
    {
        //public Vector2 direction { get; set; } = Vector2.zero;

        public WalkCommand(IMoveableReceiver client) : base(client)
        {
            
        }
        public override void Execute()
        {
            Client.Move(direction); 
        }
    }
}
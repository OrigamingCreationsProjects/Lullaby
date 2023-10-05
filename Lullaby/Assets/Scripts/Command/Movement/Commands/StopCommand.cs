using Movement.Components;
using UnityEngine;

namespace Movement.Commands
{
    public class StopCommand: AMovementCommand
    {
        public StopCommand(IMoveableReceiver client) : base(client)
        {
        }
        public Vector2 direction { get; set; } = Vector2.zero;
        public override void Execute()
        {
            Client.Move(direction);
        }
            
    }
}
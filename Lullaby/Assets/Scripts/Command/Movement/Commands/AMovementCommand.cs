using Movement.Components;
using UnityEngine;
namespace Movement.Commands
{
    public abstract class AMovementCommand : ICommand
    {
        protected readonly IMoveableReceiver Client;
        protected Vector2 direction { get; set; } = Vector2.zero;

        public void SetDirection(Vector2 direction)
        {
            this.direction = direction;
        }
        protected AMovementCommand(IMoveableReceiver client)
        {
            Client = client;
        }
        
        public abstract void Execute();
    }
}
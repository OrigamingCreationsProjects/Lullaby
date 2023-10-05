using System.Net.Http;
using Movement.Components;

namespace Movement.Commands
{
    public abstract class AJumpCommand : ICommand
    {
        protected readonly IJumperReceiver Client;

        protected AJumpCommand(IJumperReceiver client)
        {
            Client = client;
        }

        public abstract void Execute();
    }
}
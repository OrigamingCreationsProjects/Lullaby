using Movement.Components;

namespace Movement.Commands
{
    public class LandingCommand: AJumpCommand
    {
        public LandingCommand(IJumperReceiver client) : base(client)
        {
            
        }

        public override void Execute()
        {
            Client.Jump(IJumperReceiver.JumpStage.Landing);
        }
    }
}
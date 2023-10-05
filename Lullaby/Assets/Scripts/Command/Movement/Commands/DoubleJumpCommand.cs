using Movement.Components;

namespace Movement.Commands
{
    public class DoubleJumpCommand: AJumpCommand
    {
        public DoubleJumpCommand(IJumperReceiver client) : base(client)
        {
            
        }

        public override void Execute()
        {
            Client.Jump(IJumperReceiver.JumpStage.DoubleJumping);
        }
    }
}
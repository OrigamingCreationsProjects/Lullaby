using Movement.Components;

namespace Movement.Commands
{
    public class JumpCommand: AJumpCommand
    {
        public JumpCommand(IJumperReceiver client) : base(client)
        {
            
        }

        public override void Execute()
        {
            Client.Jump(IJumperReceiver.JumpStage.Jumping);
        }
    }
}
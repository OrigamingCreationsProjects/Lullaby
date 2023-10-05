namespace Movement.Components
{
    public interface IJumperReceiver
    {
        public enum JumpStage
        {
            Jumping,
            DoubleJumping,
            Landing
        }

        public void Jump(JumpStage stage);
    }
}
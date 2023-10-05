using UnityEngine;

namespace Movement.Components
{
    public interface IMoveableReceiver: IReceiver
    {
        public void Move(Vector3 direction);
    }
}
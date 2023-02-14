using UnityEngine;

namespace ConveyorBelts.Scripts
{
    public abstract class ConveyorMovable : MonoBehaviour, IConveyorMovable
    {
        private Vector3 _movement;
        void IConveyorMovable.AddMovement(Vector3 movement) => _movement += movement;
        protected abstract void Move(Vector3 movement);
        public void FixedUpdate() => Move(_movement);
    }
}

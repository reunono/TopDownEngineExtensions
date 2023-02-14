using UnityEngine;

namespace ConveyorBelts.Scripts
{
    public class RigidbodyConveyorMovable : ConveyorMovable
    {
        private Rigidbody _rigidbody;
        private void Awake() => _rigidbody = GetComponent<Rigidbody>();
        protected override void Move(Vector3 movement) => _rigidbody.position += movement * Time.deltaTime;
    }
}

using UnityEngine;

namespace ConveyorBelts.Scripts
{
    public class RigidbodyConveyorMovable : MonoBehaviour, IConveyorMovable
    {
        private Rigidbody _rigidbody;
        private void Awake() => _rigidbody = GetComponent<Rigidbody>();
        void IConveyorMovable.Move(Vector3 movement) => _rigidbody.position += movement * Time.deltaTime;
    }
}

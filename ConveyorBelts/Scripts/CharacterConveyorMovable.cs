using MoreMountains.TopDownEngine;
using UnityEngine;

namespace ConveyorBelts.Scripts
{
    public class CharacterConveyorMovable : MonoBehaviour, IConveyorMovable
    {
        private TopDownController3D _controller;
        private void Awake() => _controller = GetComponent<TopDownController3D>();
        void IConveyorMovable.Move(Vector3 movement) => _controller.AddForce(movement);
    }
}

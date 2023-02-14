using MoreMountains.TopDownEngine;
using UnityEngine;

namespace ConveyorBelts.Scripts
{
    public class CharacterConveyorMovable : ConveyorMovable
    {
        private TopDownController3D _controller;
        private void Awake() => _controller = GetComponent<TopDownController3D>();
        protected override void Move(Vector3 movement) => _controller.AddForce(movement);
    }
}

using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TankControls
{
    public class OrientationRelativeMovement : CharacterMovement
    {
        protected override void SetMovement()
        {
            base.SetMovement();
            _controller.SetMovement(Quaternion.Euler(0, _model.transform.localEulerAngles.y, 0) * _movementVector);
        }
    }
}

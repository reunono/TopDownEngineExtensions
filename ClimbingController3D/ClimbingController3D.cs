using MoreMountains.TopDownEngine;
using UnityEngine;

namespace ClimbingControllerExtension
{
    /// <summary>
    /// An extension of TopDownController3D which gives you the option (activated by default) to have CharacterDash3D climb slopes instead of stopping at obstacles
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/Core/Climbing Controller 3D")]
    public class ClimbingController3D : TopDownController3D
    {
        [Tooltip("if this option is checked, the CharacterDash3D will climb slopes, else the behaviour will be the same as with TopDownController3D")]
        public bool ClimbWhileDashing = true;
        public override void MovePosition(Vector3 newPosition)
        {
            if (ClimbWhileDashing)
                _characterController.Move(newPosition - _transform.position);
            else
                base.MovePosition(newPosition);
        }
    }
}

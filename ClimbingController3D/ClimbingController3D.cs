using System.Net.Http.Headers;
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
        [Header("Dash")]
        
        [Tooltip("if this option is checked, the CharacterDash3D will climb slopes, else the behaviour will be the same as with TopDownController3D")]
        public bool ClimbWhileDashing = true;
        [Tooltip("if both this option and the above option are checked, the character will move downwards while dashing at the speed defined below")]
        public bool MoveDownwardsWhileDashing = true;
        [Tooltip("the speed to move down at while dashing (in meters per second), useful for sticking to the ground")]
        public float DownwardDashingSpeed = 1000;
        
        public override void MovePosition(Vector3 newPosition)
        {
            if (ClimbWhileDashing)
            {
                if (MoveDownwardsWhileDashing)
                    _characterController.Move(newPosition - _transform.position + DownwardDashingSpeed * Time.deltaTime * Vector3.down);
                else
                    _characterController.Move(newPosition - _transform.position);
            }
            else
                base.MovePosition(newPosition);
        }
    }
}

using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TemporaryEffects
{
    public class MultipliedSpeedProjectile : Projectile
    {
        public Multipliers SpeedMultiplier;
    
        public override void Movement()
        {
            _movement = SpeedMultiplier.TotalMultiplier * (Speed / 10) * Time.deltaTime * Direction;
            if (_rigidBody)
                _rigidBody.MovePosition (transform.position + _movement);
            else if (_rigidBody2D)
                _rigidBody2D.MovePosition(transform.position + _movement);
            else
                transform.position += _movement;
            Speed += Acceleration * Time.deltaTime;
        }
    }
}

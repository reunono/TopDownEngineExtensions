using MoreMountains.TopDownEngine;
using UnityEngine;

namespace SpeedMultipliers.Scripts
{
    public class SpeedProjectile : Projectile
    {
        [SerializeField]
        private FloatVariable SpeedMultiplier;

        public override void Movement()
        {
            _movement = Direction * (Speed / 10) * Time.deltaTime * SpeedMultiplier.Value;
            //transform.Translate(_movement,Space.World);
            if (_rigidBody != null)
            {
                _rigidBody.MovePosition (this.transform.position + _movement);
            }
            if (_rigidBody2D != null)
            {
                _rigidBody2D.MovePosition(this.transform.position + _movement);
            }
            // We apply the acceleration to increase the speed
            Speed += Acceleration * Time.deltaTime * SpeedMultiplier.Value;
        }
        
        protected override void OnEnable()
        {
            Size = GetBounds().extents * 2;
            if (LifeTime > 0f)
            {
                Invoke(nameof(Destroy), LifeTime / SpeedMultiplier.Value);	
            }
            Initialization();
            if (InitialInvulnerabilityDuration>0)
            {
                StartCoroutine(InitialInvulnerability());
            }

            if (_health != null)
            {
                _health.OnDeath += OnDeath;
            }
        }
    }
}

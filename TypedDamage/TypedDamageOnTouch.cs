using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions
{
    public class TypedDamageOnTouch : DamageOnTouch
    {
        [Header("Damage type")] public DamageType DamageType;
        protected override void OnCollideWithDamageable(Health health)
        {
            // if what we're colliding with is a TopDownController, we apply a knockback force
            _colliderTopDownController = health.gameObject.MMGetComponentNoAlloc<TopDownController>();
            _colliderRigidBody = health.gameObject.MMGetComponentNoAlloc<Rigidbody>();

            if ((_colliderTopDownController != null) && (DamageCausedKnockbackForce != Vector3.zero) && (!_colliderHealth.Invulnerable) && (!_colliderHealth.ImmuneToKnockback))
            {
                _knockbackForce = DamageCausedKnockbackForce;

                if (_twoD) // if we're in 2D
                {
                    if (DamageCausedKnockbackDirection == KnockbackDirections.BasedOnSpeed)
                    {
                        Vector3 totalVelocity = _colliderTopDownController.Speed + _velocity;
                        _knockbackForce = Vector3.RotateTowards(DamageCausedKnockbackForce, totalVelocity.normalized, 10f, 0f);
                    }
                    if (DamageCausedKnockbackDirection == KnockbackDirections.BasedOnOwnerPosition)
                    {
                        if (Owner == null) { Owner = this.gameObject; }
                        Vector3 relativePosition = _colliderTopDownController.transform.position - Owner.transform.position;
                        _knockbackForce = Vector3.RotateTowards(DamageCausedKnockbackForce, relativePosition.normalized, 10f, 0f);
                    }    
                }
                else // if we're in 3D
                {
                    if (DamageCausedKnockbackDirection == KnockbackDirections.BasedOnSpeed)
                    {
                        Vector3 totalVelocity = _colliderTopDownController.Speed + _velocity;
                        _knockbackForce = DamageCausedKnockbackForce * totalVelocity.magnitude;
                    }
                    if (DamageCausedKnockbackDirection == KnockbackDirections.BasedOnOwnerPosition)
                    {
                        if (Owner == null) { Owner = this.gameObject; }
                        Vector3 relativePosition = _colliderTopDownController.transform.position - Owner.transform.position;
                        _knockbackForce.x = relativePosition.normalized.x * DamageCausedKnockbackForce.x;
                        _knockbackForce.y = DamageCausedKnockbackForce.y;
                        _knockbackForce.z = relativePosition.normalized.z * DamageCausedKnockbackForce.z;
                    } 
                }

                if (DamageCausedKnockbackType == KnockbackStyles.AddForce)
                {
                    _colliderTopDownController.Impact(_knockbackForce.normalized, _knockbackForce.magnitude);
                }
            }

            HitDamageableFeedback?.PlayFeedbacks(this.transform.position);
            
            // we apply the damage to the thing we've collided with
            var typedHealth = _colliderHealth as TypedHealth;
            if (typedHealth != null)
                typedHealth.Damage(DamageCaused, gameObject, InvincibilityDuration, InvincibilityDuration, _damageDirection, DamageType);
            else
                _colliderHealth.Damage(DamageCaused, gameObject, InvincibilityDuration, InvincibilityDuration, _damageDirection);
            if (DamageTakenEveryTime + DamageTakenDamageable > 0)
            {
                SelfDamage(DamageTakenEveryTime + DamageTakenDamageable);
            }
        }
    }
}

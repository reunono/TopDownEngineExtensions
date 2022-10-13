using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TemporaryEffects
{
    public class MultipliedDamageOnTouch : DamageOnTouch
    {
        [MMInspectorGroup("Multiplier", true, 9)]
        [SerializeField] private Multipliers DamageMultipliers;

        protected override void OnCollideWithDamageable(Health health)
        {
            if (!health.CanTakeDamageThisFrame())
            {
                return;
            }
			
            // if what we're colliding with is a TopDownController, we apply a knockback force
            _colliderTopDownController = health.gameObject.MMGetComponentNoAlloc<TopDownController>();

            HitDamageableFeedback?.PlayFeedbacks(transform.position);

            // we apply the damage to the thing we've collided with
            var randomDamage = DamageMultipliers.TotalMultiplier * Random.Range(MinDamageCaused, Mathf.Max(MaxDamageCaused, MinDamageCaused));
			
            ApplyKnockback(randomDamage, TypedDamages);

            if (RepeatDamageOverTime)
            {
                _colliderHealth.DamageOverTime(randomDamage, gameObject, InvincibilityDuration, InvincibilityDuration, _damageDirection, TypedDamages, AmountOfRepeats, DurationBetweenRepeats, DamageOverTimeInterruptible, RepeatedDamageType);	
            }
            else
            {
                _colliderHealth.Damage(randomDamage, gameObject, InvincibilityDuration, InvincibilityDuration, _damageDirection, TypedDamages);	
            }

            // we apply self damage
            if (DamageTakenEveryTime + DamageTakenDamageable > 0 && !_colliderHealth.PreventTakeSelfDamage)
            {
                SelfDamage(DamageTakenEveryTime + DamageTakenDamageable);
            }
        }
    }
}

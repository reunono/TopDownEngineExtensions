using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace StatusSystem
{
    public class MultipliedDamageOnTouch : DamageOnTouch
    {
        private CharacterDamageMultipliers _multipliers;
        protected override void Awake()
        {
            base.Awake();
            // the CharacterDamageMultipliers scriptable object must be in a Resources folder inside your project, like so : Resources/RuntimeSets/CharacterDamageMultipliers
            _multipliers = Resources.Load<CharacterDamageMultipliers>("RuntimeSets/CharacterDamageMultipliers");
        }

        protected override void OnCollideWithDamageable(Health health)
        {
            _collidingHealth = health;

            if (health.CanTakeDamageThisFrame())
            {
                // if what we're colliding with is a TopDownController, we apply a knockback force
                _colliderTopDownController = health.gameObject.MMGetComponentNoAlloc<TopDownController>();
                if (_colliderTopDownController == null)
                {
                    _colliderTopDownController = health.gameObject.GetComponentInParent<TopDownController>();
                }

                HitDamageableFeedback?.PlayFeedbacks(this.transform.position);
                HitDamageableEvent?.Invoke(_colliderHealth);

                // we apply the damage to the thing we've collided with
                float randomDamage = (Owner.TryGetComponent<Character>(out var character) ? _multipliers[character] : 1) *
                    UnityEngine.Random.Range(MinDamageCaused, Mathf.Max(MaxDamageCaused, MinDamageCaused));

                ApplyKnockback(randomDamage, TypedDamages);

                DetermineDamageDirection();

                if (RepeatDamageOverTime)
                {
                    _colliderHealth.DamageOverTime(randomDamage, gameObject, InvincibilityDuration,
                        InvincibilityDuration, _damageDirection, TypedDamages, AmountOfRepeats, DurationBetweenRepeats,
                        DamageOverTimeInterruptible, RepeatedDamageType);
                }
                else
                {
                    _colliderHealth.Damage(randomDamage, gameObject, InvincibilityDuration, InvincibilityDuration,
                        _damageDirection, TypedDamages);
                }
            }

            // we apply self damage
            if (DamageTakenEveryTime + DamageTakenDamageable > 0 && !_colliderHealth.PreventTakeSelfDamage)
            {
                SelfDamage(DamageTakenEveryTime + DamageTakenDamageable);
            }
        }
    }
}

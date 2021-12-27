using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace StatusSystem
{
    public class ApplyStatusAndDamageOnTouch : MultipliedDamageOnTouch
    {
        [Header("Status effects")]
        [Tooltip("The list of status effects that will be applied to colliding characters (provided they have a CharacterStatusEffectListener with the required status effects)")]
        public StatusEffect[] StatusEffects;

        protected Character _character;

        protected override void Colliding(GameObject collider)
        {
            if (!this.isActiveAndEnabled)
            {
                return;
            }

            // if the object we're colliding with is part of our ignore list, we do nothing and exit
            if (_ignoredGameObjects.Contains(collider))
            {
                return;
            }

            // if what we're colliding with isn't part of the target layers, we do nothing and exit
            if (!MMLayers.LayerInLayerMask(collider.layer, TargetLayerMask))
            {
                return;
            }

            // if we're on our first frame, we don't apply damage
            if (Time.time == 0f)
            {
                return;
            }
            
            // we get the character and apply all the status effects in the list
            _character = collider.gameObject.MMGetComponentNoAlloc<Character>();
            if (_character != null)
                foreach (var statusEffect in StatusEffects)
                    StatusEffectEvent.Trigger(statusEffect, _character, StatusEffectEventTypes.Apply);
            
            _colliderHealth = collider.gameObject.MMGetComponentNoAlloc<Health>();

            // if what we're colliding with is damageable
            if (_colliderHealth != null)
            {
                if (_colliderHealth.CurrentHealth > 0)
                {
                    OnCollideWithDamageable(_colliderHealth);
                }
            }

            // if what we're colliding with can't be damaged
            else
            {
                OnCollideWithNonDamageable();
            }
        }
    }
}

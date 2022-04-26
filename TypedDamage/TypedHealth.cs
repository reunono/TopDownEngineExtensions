using System;
using System.Linq;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TypedDamage
{
    [Serializable]
    public class DamageTypeMultiplier
    {
        public DamageType DamageType;
        public float Multiplier = 2;
    }
    public class TypedHealth : Health
    {
        [Header("Damage types")]
        [Tooltip("If this option is checked, we will only take damage from the types in the DamageTypesMultipliers list")]
        public bool OnlyTakeDamageFromListedTypes;
        [Tooltip("We will not take any damage from any type in this list (null type included)")]
        public DamageType[] InvulnerableDamageTypes;
        [Tooltip("This list defines the multipliers for each damage type, types that are not in this list or the invulnerable damage types list will deal normal damage if the 'only take damage from listed types' option is false")]
        public DamageTypeMultiplier[] DamageTypesMultipliers;

        public void Damage(int damage, GameObject instigator, float flickerDuration, float invincibilityDuration, Vector3 damageDirection, DamageType damageType)
        {
            if (!OnlyTakeDamageFromListedTypes && InvulnerableDamageTypes.Any(invulnerableDamageType => damageType == invulnerableDamageType)) return;
            var isListedType = false;
            foreach (var damageTypeMultiplier in DamageTypesMultipliers)
            {
                if (damageTypeMultiplier.DamageType != damageType) continue;
                damage = (int)(damage * damageTypeMultiplier.Multiplier);
                isListedType = true;
                break;
            }
            if (!isListedType && OnlyTakeDamageFromListedTypes) return;
            base.Damage(damage, instigator, flickerDuration, invincibilityDuration, damageDirection);
        }
    }
}

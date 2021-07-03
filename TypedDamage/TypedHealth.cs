using System.Linq;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions
{
    public class TypedHealth : Health
    {
        [Header("Damage types")]
        public float VulnerableDamageTypeMultiplier = 2;
        public DamageType[] VulnerableDamageTypes;
        public DamageType[] InvulnerableDamageTypes;
        
        public void Damage(int damage, GameObject instigator, float flickerDuration, float invincibilityDuration, Vector3 damageDirection, DamageType damageType)
        {
            if (damageType != null)
            {
                if (InvulnerableDamageTypes.Any(invulnerableDamageType => damageType == invulnerableDamageType)) return;
                if (VulnerableDamageTypes.Any(vulnerableDamageType => damageType == vulnerableDamageType))
                    damage = (int) (damage * VulnerableDamageTypeMultiplier);
            }
            base.Damage(damage, instigator, flickerDuration, invincibilityDuration, damageDirection);
        }
    }
}

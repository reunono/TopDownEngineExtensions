using MoreMountains.TopDownEngine;

namespace TopDownEngineExtensions
{
    public class DamageMultiplierMeleeWeapon : MeleeWeapon
    {
        public override void ApplyDamageMultiplier(float multiplier) { _damageOnTouch.DamageCaused = (int)(DamageCaused * multiplier); }
    }
}

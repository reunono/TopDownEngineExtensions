using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

namespace TopDownEngineExtensions
{
    public class DamageMultiplierCharacterHandleWeapon : CharacterHandleWeapon
    {
        private float _damageMultiplier = 1f;

        public float DamageMultiplier
        {
            get => _damageMultiplier;
            set
            {
                _damageMultiplier = value;
                ApplyDamageMultiplier();
            }
        }

        public override void ChangeWeapon(Weapon newWeapon, string weaponID, bool combo = false)
        {
            base.ChangeWeapon(newWeapon, weaponID, combo);
            ApplyDamageMultiplier();
            if (combo || CurrentWeapon == null) return;
            var comboWeapon = CurrentWeapon.gameObject.MMGetComponentNoAlloc<ComboWeapon>();
            if (!comboWeapon) return;
            var weapons = comboWeapon.GetComponents<Weapon>();
            foreach(var weapon in weapons) weapon.ApplyDamageMultiplier(_damageMultiplier);
        }

        private void ApplyDamageMultiplier()
        {
            if (CurrentWeapon == null) return;
            CurrentWeapon.ApplyDamageMultiplier(_damageMultiplier);
        }
    }
}

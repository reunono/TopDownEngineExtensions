using MoreMountains.TopDownEngine;

namespace TopDownEngineExtensions
{
    public class ScriptDrivenHandleWeapon : CharacterHandleWeapon
    {
        protected override void HandleInput() {}
        protected override void InstantiateWeapon(Weapon newWeapon, string weaponID, bool combo = false)
        {
            base.InstantiateWeapon(newWeapon, weaponID, combo);
            if (_weaponAim != null) _weaponAim.AimControl = WeaponAim.AimControls.Script;
        }
    }
}

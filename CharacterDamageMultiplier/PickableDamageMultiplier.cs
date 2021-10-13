using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using UnityEngine;

namespace TopDownEngineExtensions
{
    public class PickableDamageMultiplier : PickableItem
    {
        public float DamageMultiplier = 2f;
        private DamageMultiplierCharacterHandleWeapon _handleWeapon;

        protected override void Pick(GameObject picker)
        {
            _handleWeapon.DamageMultiplier = DamageMultiplier;
        }

        protected override bool CheckIfPickable()
        {
            _handleWeapon = _collidingObject.MMGetComponentNoAlloc<Character>()?.FindAbility<DamageMultiplierCharacterHandleWeapon>();
            return _handleWeapon != null;
        }
    }
}

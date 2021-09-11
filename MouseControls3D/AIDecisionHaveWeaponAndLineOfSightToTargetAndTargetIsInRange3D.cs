using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions
{
    public class AIDecisionHaveWeaponAndLineOfSightToTargetAndTargetIsInRange3D : AIDecisionLineOfSightToTarget3D
    {
        private Character _character;
        private CharacterHandleWeapon _characterHandleWeapon;

        protected override void Awake()
        {
            base.Awake();
            _character = GetComponentInParent<Character>();
            _characterHandleWeapon = _character.FindAbility<CharacterHandleWeapon>();;
        }

        public override bool Decide()
        {
            return _characterHandleWeapon.CurrentWeapon != null && base.Decide() && Vector3.Distance(_brain.Target.position, _character.transform.position) <= _characterHandleWeapon.CurrentWeapon.Range;
        }
    }
}

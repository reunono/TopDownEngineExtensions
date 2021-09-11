using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions
{
    [RequireComponent(typeof(CharacterHandleWeapon))]
    public sealed class AIActionUseWeapon : AIAction
    {
        private CharacterHandleWeapon _characterHandleWeapon;
        private Character _character;

        protected override void Awake()
        {
            base.Awake();
            _characterHandleWeapon = GetComponent<CharacterHandleWeapon>();
            _character = GetComponentInParent<Character>();
        }

        public override void PerformAction()
        {
            _characterHandleWeapon.WeaponAimComponent.SetCurrentAim(_brain.Target.position - _character.transform.position);
            _characterHandleWeapon.CurrentWeapon.WeaponInputStart();
        }

        public override void OnExitState()
        {
            base.OnExitState();
            _characterHandleWeapon.CurrentWeapon.WeaponInputStop();
        }
    }
}
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace GrapplingHook
{
    public class GrapplingHookWeapon : HitscanWeapon
    {
        [MMInspectorGroup("Grappling", true, 28)]
        public float MaxForce = 1000;
        public AnimationCurve GrapplingForce = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
        public Rope Rope;
        private float _distance;

        public override void CaseWeaponStart()
        {
            base.CaseWeaponStart();
            DetermineSpawnPosition();
            DetermineDirection();
            SpawnProjectile(SpawnPosition);
            _distance = (_hitPoint - transform.position).MMSetY(0).magnitude;
            if (_hitObject != null) Rope?.PlayAnimation(_hitPoint);
        }
        
        protected override void HandleDamage()
        {
            if (_hitObject == null) return;
            var direction = (_hitPoint - transform.position).MMSetY(0);
            _controller.Impact(direction, MaxForce * GrapplingForce.Evaluate(1-direction.magnitude / _distance) * Time.deltaTime);
        }

        public override void WeaponUse() { HandleDamage(); }

        public override void WeaponInputStop()
        {
            base.WeaponInputStop();
            _hitObject = null;
            Rope?.StopAnimation();
        }
    }
}

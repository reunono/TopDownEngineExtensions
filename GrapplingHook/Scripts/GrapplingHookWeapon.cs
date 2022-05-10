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
        private WeaponAutoAim3D _autoAim;

        private void Awake()
        {
            _autoAim = GetComponent<WeaponAutoAim3D>();
        }

        public override void CaseWeaponStart()
        {
            base.CaseWeaponStart();
            if (_autoAim == null)
            {
                DetermineSpawnPosition();
                DetermineDirection();
                SpawnProjectile(SpawnPosition);
            }
            else if (_autoAim.Target != null && Physics.Linecast(transform.position, _autoAim.Target.position, out var hit, HitscanTargetLayers))
            {
                _hitObject = hit.collider.gameObject;
                _hitPoint = hit.point;
            }
            _distance = (_hitPoint - transform.position).magnitude;
            if (_hitObject != null) Rope?.PlayAnimation(_hitPoint);
        }

        public override void WeaponUse()
        {
            if (_hitObject == null) return;
            var direction = _hitPoint - transform.position;
            // this next line fixes a bug where using the grappling hook from above makes the character fly away from the grappled object. this is caused by TopDownController3D::Impact making negative y values positive
            if (direction.y < 0) direction.y = 0;
            _controller.Impact(direction, MaxForce * GrapplingForce.Evaluate(1-direction.magnitude / _distance) * Time.deltaTime);
        }

        public override void WeaponInputStop()
        {
            base.WeaponInputStop();
            _hitObject = null;
            Rope?.StopAnimation();
        }
    }
}

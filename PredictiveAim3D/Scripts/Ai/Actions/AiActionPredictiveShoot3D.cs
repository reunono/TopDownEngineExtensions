using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using TopDownEngineExtensions.PredictiveAim3D.Scripts.Utils;
using UnityEngine;

namespace TopDownEngineExtensions.PredictiveAim3D.Scripts.Ai.Actions
{
    public class AiActionPredictiveShoot3D : AIActionShoot3D
    {
        public bool showDebugRays;
        private float _shotForce = 1f;
        private Vector3 _lastProjectilePosition;
        private Vector3 TargetPos => _brain.Target.position;

        private Vector3 BulletStartPos
        {
            get
            {
                if (AimOrigin == AimOrigins.SpawnPosition) return _projectileWeapon.SpawnPosition + ShootOffset;
                return _character.transform.position + ShootOffset;
            }
        }

        private GameObject _projectilePrefab;


        //private Vector3 targetVelocity => m_targetUsesCharController ? _brain.Target.GetComponent<CharacterController>().velocity : _brain.Target.GetComponent<Rigidbody>().velocity; // Supporth either CharController or RB!
        private Vector3 TargetVelocity => _brain.Target.GetComponent<CharacterController>().velocity;

        private bool ShotObeysGravity => _projectilePrefab.GetComponent<Rigidbody>().useGravity;


        public override void Initialization()
        {
            base.Initialization();
            _projectilePrefab = TargetHandleWeaponAbility.CurrentWeapon.GetComponent<MMSimpleObjectPooler>().GameObjectToPool;
            _shotForce = _projectilePrefab.GetComponent<Projectile>().Speed / 10;
        }


        /// <summary>
        ///     Aims at the target if required
        /// </summary>
        protected override void TestAimAtTarget()
        {
            if (!AimAtTarget || _brain.Target == null) return;

            if (TargetHandleWeaponAbility.CurrentWeapon != null)
            {
                if (_weaponAim == null) _weaponAim = TargetHandleWeaponAbility.CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim>();

                if (_weaponAim != null) _weaponAimDirection = Aim();
            }
        }

        private Vector3 Aim()
        {
            var aimVector = AimImpl();
            var aimFailure = !aimVector.HasValue;
            if (aimFailure) aimVector = _character.transform.position * _shotForce;
            //Debug.LogFormat(this, "{0} aimVector:{1}", this, aimVector);
            if (showDebugRays) Debug.DrawRay(BulletStartPos, aimVector.Value, aimFailure ? Color.magenta : Color.red, 4f, false);
            return aimVector.Value;
        }

        private Vector3? AimImpl()
        {
            var aimVector = AimingUtilities.GetLaunchVector(TargetPos, TargetVelocity, BulletStartPos, _shotForce, ShotObeysGravity ? Physics.gravity : null);
            return aimVector.CheckForNaN() ? null : aimVector;
        }
    }
}
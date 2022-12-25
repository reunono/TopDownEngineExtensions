using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using TopDownEngineExtensions.PredictiveAim3D.Scripts.Utils;
using UnityEngine;

namespace TopDownEngineExtensions.PredictiveAim3D.Scripts.Ai.Actions
{
    public class AiActionPredictiveShoot3D : AIActionShoot3D
    {
        public bool showDebugRays;
        private Vector3 _lastProjectilePosition;
        private Vector3 targetPos => _brain.Target.position;
        private Vector3 bulletStartPos
        {
            get
            {
                if (AimOrigin == AimOrigins.SpawnPosition) return _projectileWeapon.SpawnPosition + ShootOffset;
                return _character.transform.position + ShootOffset;
            }
        }
        private GameObject _projectilePrefab;
        
        //private Vector3 targetVelocity => m_targetUsesCharController ? _brain.Target.GetComponent<CharacterController>().velocity : _brain.Target.GetComponent<Rigidbody>().velocity; // Supporth either CharController or RB!
        private Vector3 targetVelocity => _brain.Target.GetComponent<CharacterController>().velocity;
        private bool shotObeysGravity => _projectilePrefab.GetComponent<Rigidbody>().useGravity;
        private float _shotForce = 1f;


        /// <summary>
        ///     Aims at the target if required
        /// </summary>
        protected override void TestAimAtTarget()
        {
            if (!AimAtTarget || _brain.Target == null) return;
            if (_projectilePrefab == null)
            {
                _projectilePrefab = TargetHandleWeaponAbility.CurrentWeapon.GetComponent<MMSimpleObjectPooler>().GameObjectToPool;
            }
            _shotForce = _projectilePrefab.GetComponent<Projectile>().Speed / 10;
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
            if (showDebugRays) Debug.DrawRay(bulletStartPos, aimVector.Value, aimFailure ? Color.magenta : Color.red, 4f, false);
            return aimVector.Value;
        }

        private Vector3? AimImpl()
        {
            var aimVector = AimingUtilities.GetLaunchVector(targetPos, targetVelocity, bulletStartPos, _shotForce, shotObeysGravity ? Physics.gravity : null);
            return aimVector.CheckForNaN() ? null : aimVector;
        }
    }
}
using System;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions
{
    public class AngleBasedAutoAim3D : WeaponAutoAim3D
    {
        [SerializeField]
        [Tooltip("The maximum angle between the target and the direction the character is currently going in for which auto aim will activate")]
        private float MaxAngle = 180;
        private Transform[] _unobstructedTargets;
        private WeaponAim3D _weaponAim3D;

        protected override void Initialization()
        {
            base.Initialization();
            _unobstructedTargets = new Transform[_hits.Length];
            _weaponAim3D = (WeaponAim3D)_weaponAim;
        }

        protected override bool ScanForTargets()
        {
            Target = null;
            var numberOfHits = Physics.OverlapSphereNonAlloc(Origin, ScanRadius, _hits, TargetsMask);
            
            if (numberOfHits == 0) return false;
            var numberOfUnobstructedTargets = 0;
            for (var i = 0; i < numberOfHits; i++)
            {
                _raycastDirection = _hits[i].transform.position - _raycastOrigin;
                var hit = MMDebug.Raycast3D(_raycastOrigin, _raycastDirection, Vector3.Distance(_hits[i].transform.position, _raycastOrigin), ObstacleMask.value, Color.yellow, true);
                if (hit.collider != null) continue;
                _unobstructedTargets[numberOfUnobstructedTargets] = _hits[i].transform;
                numberOfUnobstructedTargets++;
            }

            if (numberOfUnobstructedTargets == 0) return false;
            var smallestAngle = 180f;
            Target = _unobstructedTargets[0];
            for (var i = 0; i < numberOfUnobstructedTargets; i++)
            {
                var angleToTarget = Vector3.Angle(AimDirection(), _unobstructedTargets[i].position - _topDownController3D.transform.position);
                if (angleToTarget > smallestAngle) continue;
                smallestAngle = angleToTarget;
                Target = _unobstructedTargets[i];
            }

            if (smallestAngle < MaxAngle) return true;
            Target = null;
            return false;
            
            Vector3 AimDirection()
            {
                switch (_originalAimControl)
                {
                    case WeaponAim.AimControls.Off:
                        _weaponAim3D.GetOffAim();
                        break;
                    case WeaponAim.AimControls.PrimaryMovement:
                        _weaponAim3D.GetPrimaryMovementAim();
                        break;
                    case WeaponAim.AimControls.SecondaryMovement:
                        _weaponAim3D.GetSecondaryMovementAim();
                        break;
                    case WeaponAim.AimControls.Mouse:
                        _weaponAim3D.GetMouseAim();
                        break;
                    case WeaponAim.AimControls.Script:
                        _weaponAim3D.GetScriptAim();
                        break;
                    case WeaponAim.AimControls.SecondaryThenPrimaryMovement:
                        if (_weapon.Owner.LinkedInputManager.SecondaryMovement.magnitude > _weaponAim3D.MinimumMagnitude)
                            _weaponAim3D.GetSecondaryMovementAim();
                        else
                            _weaponAim3D.GetPrimaryMovementAim();
                        break;
                    case WeaponAim.AimControls.PrimaryThenSecondaryMovement:
                        if (_weapon.Owner.LinkedInputManager.PrimaryMovement.magnitude > _weaponAim3D.MinimumMagnitude)
                            _weaponAim3D.GetPrimaryMovementAim();
                        else
                            _weaponAim3D.GetSecondaryMovementAim();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return _weaponAim3D.CurrentAim;
            }
        }
    }
}
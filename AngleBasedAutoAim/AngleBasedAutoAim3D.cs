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

        protected override void Initialization()
        {
            base.Initialization();
            _unobstructedTargets = new Transform[_hits.Length];
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
                var angleToTarget = Vector3.Angle(_topDownController3D.CurrentDirection, _unobstructedTargets[i].position - _topDownController3D.transform.position);
                if (angleToTarget > smallestAngle) continue;
                smallestAngle = angleToTarget;
                Target = _unobstructedTargets[i];
            }

            if (smallestAngle < MaxAngle) return true;
            Target = null;
            return false;
        }
    }
}

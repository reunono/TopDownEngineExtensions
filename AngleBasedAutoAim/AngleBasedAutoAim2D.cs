using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions
{
    public class AngleBasedAutoAim2D : WeaponAutoAim2D
    {
        private TopDownController2D _topDownController2D;
        protected override void Initialization()
        {
            base.Initialization();
            if (_weapon.Owner != null)
            {
                _topDownController2D = _weapon.Owner.GetComponent<TopDownController2D>();
            }
        }

        protected override bool ScanForTargets()
        {
            if (!_initialized) Initialization();

            Target = null;

            var count = Physics2D.OverlapCircleNonAlloc(_weapon.Owner.transform.position, ScanRadius, _results, TargetsMask);
            if (count == 0) return false;
            var unobstructedTargets = new List<Transform>();
            foreach (var target in _results)
            {
                _boxcastDirection = (Vector2)(target.bounds.center - _raycastOrigin);
                var hit = Physics2D.BoxCast(_raycastOrigin, LineOfFireBoxcastSize, 0f, _boxcastDirection.normalized, _boxcastDirection.magnitude, ObstacleMask);
                if (!hit) unobstructedTargets.Add(target.transform);
            }
            if (unobstructedTargets.Count == 0) return false;
            var smallestAngle = 180f;
            Target = unobstructedTargets[0];
            for (var i = 0; i < unobstructedTargets.Count; i++)
            {
                var angleToTarget = Vector3.Angle(_topDownController2D.CurrentDirection, unobstructedTargets[i].position - _topDownController2D.transform.position);
                if (angleToTarget > smallestAngle) continue;
                smallestAngle = angleToTarget;
                Target = unobstructedTargets[i];
            }

            return true;
        }
    }
}
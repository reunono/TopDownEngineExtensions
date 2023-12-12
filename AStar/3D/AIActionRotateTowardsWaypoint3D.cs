using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions.AStar3D
{
    public class AIActionRotateTowardsWaypoint3D : AIActionRotateTowardsTarget3D
    {
        protected CharacterPathfinder3DAStar _characterPathfinder;

        public override void Initialization()
        {
            base.Initialization();
            _characterPathfinder = this.gameObject.GetComponentInParent<CharacterPathfinder3DAStar>();
        }

        protected override void Rotate()
        {
            if (_brain.Target == null || _characterPathfinder == null)
            {
                return;
            }

            Vector3 direction;
            var waypoints = _characterPathfinder.GetWaypoints();
            if (waypoints == null || waypoints.Length == 0)
            {
                return;
            }
            
            if (waypoints.Length <= 2)
            {
                _targetPosition = _brain.Target.transform.position;
            }
            else
            {
                _targetPosition = waypoints[0];
                // Invert rotation by 180 degrees for Waypoints[0]
                direction = (_targetPosition - this.transform.position).normalized;
                direction = Quaternion.Euler(0, 180, 0) * direction;
                _characterOrientation3D.ForcedRotationDirection = direction;
                return;
            }

            if (LockRotationX)
            {
                _targetPosition.y = this.transform.position.y;
            }

            direction = (_targetPosition - this.transform.position).normalized;
            _characterOrientation3D.ForcedRotationDirection = direction;
        }
    }
}

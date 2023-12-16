using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions.AStar3D
{
    public class AIActionRotateTowardsWaypoint3D : AIActionRotateTowardsTarget3D
    {
        public bool ShowGizmos;
        protected CharacterPathfinder3DAStar _characterPathfinder;
        
        public override void Initialization()
        {
            base.Initialization();
            _characterPathfinder = this.gameObject.GetComponentInParent<CharacterPathfinder3DAStar>();
        }

        protected override void Rotate()
        {
            if (_brain.Target == null || _characterPathfinder == null) return;
            var waypoints = _characterPathfinder.GetWaypoints();
            if (waypoints == null || waypoints.Length == 0) return;
            _targetPosition = waypoints.Length <= 2 ? _brain.Target.transform.position : waypoints[0];
            if (LockRotationX) _targetPosition.y = transform.position.y;
            var direction = (_targetPosition - transform.position).normalized;
            if (waypoints.Length > 2) direction = Quaternion.Euler(0, 180, 0) * direction;
            _characterOrientation3D.ForcedRotationDirection = direction;
        }




        // Draw gizmos in the Scene view
        private void OnDrawGizmos()
        {
            if (!ShowGizmos) return;
            if (_characterPathfinder == null || _characterPathfinder.GetWaypoints() == null ||
                _characterPathfinder.GetWaypoints().Length <= 0) return;
            Gizmos.color = Color.green; // Change the color if needed
            Gizmos.DrawSphere(_characterPathfinder.GetWaypoints()[0], 1f); // Adjust the size (0.5f) as needed
        }
    }
}
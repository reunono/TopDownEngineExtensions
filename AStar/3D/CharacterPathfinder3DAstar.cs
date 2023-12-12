using MoreMountains.TopDownEngine;
using Pathfinding;
using Pathfinding.RVO;
using UnityEngine;


namespace TopDownEngineExtensions.AStar3D 
{
    public class CharacterPathfinder3DAStar : CharacterPathfinder3D
    {
        private RVOController _rvocontroller;
        private Seeker _seeker;
        public bool showDebugMessages;
        public float slowRadius = 3f;
        public float stopRadius = 1f;
        private bool _pathSearched;
        private Vector3 _targetPos;
        private float _slowRadiusSqr;
        private float _maxMovementSpeed;

        // private Vector3 _targetPos => _character.transform.position;
        protected override void Awake()
        {
            base.Awake();
            _seeker = GetComponent<Seeker>();
            _rvocontroller = GetComponent<RVOController>();
            _targetPos = _character.transform.position; // Initialize _targetPos here
            _slowRadiusSqr = slowRadius * slowRadius; // Calculate _slowRadiusSqr
        }
        public Vector3[] GetWaypoints()
        {

            return Waypoints;
        }
        protected override void DeterminePath(Vector3 startingPos, Vector3 targetPos, bool ignoreDelay = false)
        {
            if ((_targetPos - targetPos).sqrMagnitude > 0.001f)
                _pathSearched = false;

            if (!_pathSearched)
            {
                _seeker.StartPath(startingPos, targetPos, OnPathComplete);
            }
            else
            {
                UpdateRVOController();
            }
        }

        private void UpdateRVOController()
        {
            _maxMovementSpeed = _characterMovement.MovementSpeed * _characterMovement.MovementSpeedMultiplier * _characterMovement.ContextSpeedMultiplier;
            _rvocontroller.SetTarget(Waypoints[NextWaypointIndex], _characterMovement.MovementSpeed, _maxMovementSpeed, Vector3.positiveInfinity);
        }

        public void OnPathComplete(Path p)
        {
            if (p.error)
            {
                if (showDebugMessages) Debug.Log("No Valid Path Found");
            }
            else
            {
                Waypoints = p.vectorPath.ToArray();
                _waypoints = Waypoints.Length;

                if (p.vectorPath.Count >= 2) NextWaypointIndex = 1;

                UpdateRVOController();
                _pathSearched = true;
            }
        }

        protected override void MoveController()
        {
            if (Target == null || NextWaypointIndex <= 0)
            {
                _characterMovement.SetMovement(Vector2.zero);
                return;
            }

            var distanceSqr = (transform.position - Target.position).sqrMagnitude;
            float targetSpeed;

            if (distanceSqr <= stopRadius * stopRadius)
                targetSpeed = 0.0f;
            else if (distanceSqr > _slowRadiusSqr)
                targetSpeed = _characterMovement.MovementSpeed;
            else
                targetSpeed = _characterMovement.MovementSpeed * Mathf.Sqrt(distanceSqr) / slowRadius;

            var targetPoint = Waypoints[NextWaypointIndex];
            _rvocontroller.SetTarget(targetPoint, targetSpeed, _characterMovement.MovementSpeed, Vector3.positiveInfinity);
            var delta = _rvocontroller.CalculateMovementDelta(transform.position, Time.deltaTime);

            _direction = delta / (Time.deltaTime * _maxMovementSpeed);
            _newMovement.x = _direction.x;
            _newMovement.y = _direction.z;
            _characterMovement.SetMovement(_newMovement);
        }
    }
}

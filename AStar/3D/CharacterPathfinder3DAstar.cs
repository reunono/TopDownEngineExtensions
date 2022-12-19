using MoreMountains.TopDownEngine;
using Pathfinding;
using Pathfinding.RVO;
using UnityEngine;


namespace TopDownEngineExtensions.AStar.3D {
public class CharacterPathfinder3DAStar : CharacterPathfinder3D
    {
        private RVOController _rvocontroller;
        private Seeker _seeker;
        public bool showDebugMessages = false;
        public float slowRadius = 3f;
        public float stopRadius = 1f;

        protected override void Awake()
        {
            base.Awake();
            _seeker = gameObject.GetComponent<Seeker>();
            _rvocontroller = gameObject.GetComponent<RVOController>();
        }

        /// <summary>
        /// Determines the next path position for the agent. NextPosition will be zero if a path couldn't be found
        /// </summary>
        /// <param name="startingPos"></param>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        /// 
        protected override void DeterminePath(Vector3 startingPos, Vector3 targetPos)
        {
            _seeker.StartPath(startingPos, targetPos, OnPathComplete);
        }

        public void OnPathComplete(Path p)
        {
            if (p.error)
            {
                Debug.Log("No Valid Path Found");
            }
            else
            {
                Waypoints = p.vectorPath.ToArray();
                if (p.vectorPath.Count >= 2)
                {
                    NextWaypointIndex = 1;
                }

                float maxMovementSpeed = _characterMovement.MovementSpeed *
                                         _characterMovement.MovementSpeedMultiplier *
                                         _characterMovement.ContextSpeedMultiplier;
                _rvocontroller.SetTarget(Waypoints[NextWaypointIndex], _characterMovement.MovementSpeed,
                        maxMovementSpeed, Vector3.positiveInfinity);
            }
        }

        protected override void MoveController()
        {
            if ((Target == null) || (NextWaypointIndex <= 0))
            {
                _characterMovement.SetMovement(Vector2.zero);
                return;
            }


            float maxMovementSpeed = _characterMovement.MovementSpeed *
                                     _characterMovement.MovementSpeedMultiplier *
                                     _characterMovement.ContextSpeedMultiplier;

            var distance = Vector3.Distance(this.transform.position, Target.position);
            float targetSpeed;

            if (distance <= stopRadius)
            {
                targetSpeed = 0.0f;
            }
            else if (distance > slowRadius)
            {
                targetSpeed = _characterMovement.MovementSpeed;
            }
            else
            {
                targetSpeed = _characterMovement.MovementSpeed * distance / slowRadius;
            }

            Vector3 targetPoint = Waypoints[NextWaypointIndex];
            _rvocontroller.SetTarget(targetPoint, targetSpeed, _characterMovement.MovementSpeed,
                Vector3.positiveInfinity);
            Vector3 delta = _rvocontroller.CalculateMovementDelta(transform.position, Time.deltaTime);

            _direction = delta / (Time.deltaTime * maxMovementSpeed);
            _newMovement.x = _direction.x;
            _newMovement.y = _direction.z;
            _characterMovement.SetMovement(_newMovement);
        }
    }
}

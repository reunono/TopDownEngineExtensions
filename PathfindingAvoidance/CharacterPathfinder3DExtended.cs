public class CharacterPathfinder3DExtended : CharacterPathfinder3D {
        private NavMeshObstacle _navMeshObstacle;
   
        protected override void Awake() {
            _navMeshObstacle = GetComponent<NavMeshObstacle>();
            base.Awake();
        }
 
        protected override void DeterminePath(Vector3 startingPosition, Vector3 targetPosition) {
            StartCoroutine(DoDeterminePath(startingPosition, targetPosition));
        }
 
        private IEnumerator DoDeterminePath(Vector3 startingPosition, Vector3 targetPosition) {
            _navMeshObstacle.enabled = false;
            yield return null;
            NextWaypointIndex = 0;
 
            _closestNavmeshPosition = targetPosition;
            if (NavMesh.SamplePosition(targetPosition, out _navMeshHit, ClosestPointThreshold, NavMesh.AllAreas))
            {
                _closestNavmeshPosition = _navMeshHit.position;
            }
       
            _pathFound = NavMesh.CalculatePath(startingPosition, _closestNavmeshPosition, NavMesh.AllAreas, AgentPath);
            if (_pathFound)
            {
                _lastValidTargetPosition = _closestNavmeshPosition;
            }
            else
            {
                NavMesh.CalculatePath(startingPosition, _lastValidTargetPosition, NavMesh.AllAreas, AgentPath);
            }
            _navMeshObstacle.enabled = true;
            Waypoints = AgentPath.corners;
            if (AgentPath.corners.Length >= 2)
            {
                NextWaypointIndex = 1;
            }
        }
    }
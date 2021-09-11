using System.Collections;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions
{
    [RequireComponent(typeof(AIBrain))]
    public class MouseControls3D : CharacterPathfindToMouse3D
    {
        public float DoubleClickTime = .15f;
        public LayerMask TargetLayerMask = LayerManager.EnemiesLayerMask;
        private bool _isDoubleClick;
        private CharacterRun _characterRun;
        private Coroutine _doubleClick;
        private AIBrain _brain;
        private const string _aimWeaponAtMovementStateName = "AimWeaponAtMovement";
        private const string _attackStateName = "Attack";

        protected override void Awake()
        {
            base.Awake();
            _brain = GetComponent<AIBrain>();
            var haveWeaponAndLineOfSightToTargetAndTargetIsInRangeDecision = gameObject.AddComponent<AIDecisionHaveWeaponAndLineOfSightToTargetAndTargetIsInRange3D>();
            _brain.States.Add(new AIState
            {
                StateName = _aimWeaponAtMovementStateName,
                Actions = new AIActionsList {gameObject.AddComponent<AIActionAimWeaponAtMovement>()},
                Transitions = new AITransitionsList {new AITransition {Decision = haveWeaponAndLineOfSightToTargetAndTargetIsInRangeDecision, TrueState = _attackStateName}}
            });
            _brain.States.Add(new AIState
            {
                StateName = _attackStateName,
                Actions = new AIActionsList
                {
                    gameObject.AddComponent<AIActionUseWeapon>(),
                    gameObject.AddComponent<AIActionStandStill>()
                },
                Transitions = new AITransitionsList
                {
                    new AITransition {Decision = gameObject.AddComponent<AIDecisionTargetHasHealth>(), FalseState = _aimWeaponAtMovementStateName},
                    new AITransition {Decision = haveWeaponAndLineOfSightToTargetAndTargetIsInRangeDecision, FalseState = _aimWeaponAtMovementStateName}
                }
            });
            foreach (var state in _brain.States) state.SetBrain(_brain);
            _character.CharacterBrain = _brain;
            _characterRun = _character.FindAbility<CharacterRun>();
        }

        private IEnumerator DoubleClick()
        {
            _isDoubleClick = true;
            yield return new WaitForSeconds(DoubleClickTime);
            _isDoubleClick = false;
        }

        protected override void DetectMouse()
        {
            if (Input.GetMouseButtonDown(MouseButtonIndex))
            {
                OnClickFeedbacks?.PlayFeedbacks(Destination.transform.position);
                if (_isDoubleClick)
                {
                    _characterRun.RunStart();
                    StopCoroutine(_doubleClick);
                    _isDoubleClick = false;
                }
                else
                {
                    _characterRun.RunStop();
                    _doubleClick = StartCoroutine(DoubleClick());
                }
            }

            if (!Input.GetMouseButtonDown(MouseButtonIndex) && !Input.GetMouseButton(MouseButtonIndex)) return;
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, TargetLayerMask))
            {
                _brain.Target = hitInfo.transform;
                _characterPathfinder3D.SetNewDestination(_brain.Target);
                return;
            }
            _brain.Target = null;
            if (_brain.CurrentState.StateName != _aimWeaponAtMovementStateName) _brain.TransitionToState(_aimWeaponAtMovementStateName);
            if (!_playerPlane.Raycast(ray, out var distance)) return;
            Destination.transform.position = ray.GetPoint(distance);
            _characterPathfinder3D.SetNewDestination(Destination.transform);
        }
    }
}

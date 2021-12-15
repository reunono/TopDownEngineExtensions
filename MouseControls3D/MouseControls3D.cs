using System.Collections;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions
{
    public class MouseControls3D : CharacterPathfindToMouse3D
    {
        [Tooltip("The interval (in seconds) in which two successive clicks will be interpreted as a double click, which makes the character start running")]
        public float DoubleClickInterval = .15f;
        [Tooltip("The layer mask of objects that will be attacked when clicked (if they have a Health component with non-zero current health)")]
        public LayerMask TargetLayerMask = LayerManager.EnemiesLayerMask;
        private bool _isDoubleClick;
        private CharacterRun _characterRun;
        private Coroutine _doubleClick;
        private AIBrain _brain;
        private AIState _initialState;

        protected override void Awake()
        {
            base.Awake();
            _brain = _character.CharacterBrain;
            _initialState = _brain.States[0];
            _characterRun = _character.FindAbility<CharacterRun>();
        }

        private IEnumerator DoubleClick()
        {
            _isDoubleClick = true;
            yield return new WaitForSeconds(DoubleClickInterval);
            _isDoubleClick = false;
        }

        protected override void DetectMouse()
        {
            if (UIShouldBlockInput && MMGUI.PointOrTouchBlockedByUI()) return;
            
            if (Input.GetMouseButtonDown(MouseButtonIndex))
            {
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
#if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
#endif
            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, TargetLayerMask))
            {
                _brain.Target = hitInfo.transform;
                _characterPathfinder3D.SetNewDestination(_brain.Target);
                return;
            }
            _brain.Target = null;
            if (_brain.CurrentState != _initialState) _brain.TransitionToState(_initialState.StateName);
            if (!_playerPlane.Raycast(ray, out var distance)) return;
            Destination.transform.position = ray.GetPoint(distance);
            _characterPathfinder3D.SetNewDestination(Destination.transform);
            if (Input.GetMouseButtonDown(MouseButtonIndex))
                OnClickFeedbacks?.PlayFeedbacks(Destination.transform.position);
        }
    }
}

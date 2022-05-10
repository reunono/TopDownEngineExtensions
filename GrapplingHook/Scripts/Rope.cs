using System.Collections;
using MoreMountains.Tools;
using UnityEngine;

namespace GrapplingHook
{
    public class Rope : MonoBehaviour
    {
        [SerializeField] private int Resolution, WaveCount, WobbleCount;
        [SerializeField] private float WaveSize, Duration;
        private LineRenderer _line;
        private Coroutine _playAnimation;

        [Header("Testing")] 
        [SerializeField] private Vector3 TestTarget;
        [MMInspectorButton("PlayAnimationTest")] 
        public bool PlayAnimationTestButton;
        [MMInspectorButton("StopAnimation")] 
        public bool StopAnimationTestButton;

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
        }

        private void PlayAnimationTest() { PlayAnimation(TestTarget); }
        
        public void PlayAnimation(Vector3 target)
        {
            StopAnimation();
            _playAnimation = StartCoroutine(AnimateRope(target));
        }
        
        public void StopAnimation()
        {
            _line.positionCount = 0;
            if (_playAnimation != null) StopCoroutine(_playAnimation);
        }

        private IEnumerator AnimateRope(Vector3 targetPosition)
        {
            _line.positionCount = Resolution;
            var percent = 0f;
            while (percent < 1)
            {
                percent += Time.deltaTime / Duration;
                SetPoints(targetPosition, percent);
                yield return null;
            }

            _line.positionCount = 2;
            _line.SetPosition(1, targetPosition);
            while (true)
            {
                _line.SetPosition(0, transform.position);
                yield return null;
            }
        }

        private void SetPoints(Vector3 targetPosition, float percent)
        {
            var position = transform.position;
            var ropeEnd = Vector3.Lerp(position, targetPosition, percent);
            var ropeVector = ropeEnd - position;
            var horizontalProjectionLength = Vector3.ProjectOnPlane(ropeVector, Vector3.up).magnitude;
            var verticalProjectionLength = Vector3.ProjectOnPlane(ropeVector.MMSetZ(0), Vector3.right).magnitude;
            for (var i = 0; i < Resolution; i++)
            {
                var projectionLengthMultiplier = (float)i / Resolution;
                var x =  projectionLengthMultiplier * horizontalProjectionLength;
                var y = projectionLengthMultiplier * verticalProjectionLength * (ropeVector.y < 0 ? -1 : 1);
                var reversePercent = 1 - percent;
                var amplitude = Mathf.Sin(reversePercent * WobbleCount * Mathf.PI) * (1-(float)i/Resolution) * WaveSize;
                var z = Mathf.Sin((float)WaveCount * i / Resolution * 2 * Mathf.PI * reversePercent) * amplitude;
                var pointPosition = MMMaths.RotatePointAroundPivot(position + new Vector3(x, y, z), position, Vector3.down*LookAtAngle(ropeVector));
                _line.SetPosition(i, pointPosition);
            }
            
            float LookAtAngle(Vector3 destination) { return Mathf.Atan2(destination.z, destination.x) * Mathf.Rad2Deg; }
        }
    }
}

using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace ProgressionSystem
{
    [AddComponentMenu("Custom/Character/Progression System/Set Movement Speed On Level Up")]
    public class SetMovementSpeedOnLevelUp : DoSomethingOnLevelUp
    {
        [SerializeField]
        private AnimationCurve LevelMovementSpeedCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
        [SerializeField]
        private float MinMovementSpeed;
        [SerializeField]
        private float MaxMovementSpeed;
        [SerializeField]
        private MMFeedbacks SetMovementSpeedFeedbacks;

        private CharacterMovement _movement;

        private void Awake()
        {
            _movement = GetComponent<CharacterMovement>();
            _movement.MovementSpeed = MinMovementSpeed;
        }

        protected override void OnLevelUp(int level, int maxLevel)
        {
            var oldMovementSpeed = _movement.MovementSpeed;
            _movement.MovementSpeed = MinMovementSpeed+(MaxMovementSpeed-MinMovementSpeed)*LevelMovementSpeedCurve.Evaluate((float)level/maxLevel);
            SetMovementSpeedFeedbacks?.PlayFeedbacks(transform.position, _movement.MovementSpeed - oldMovementSpeed);
        }
    }
}

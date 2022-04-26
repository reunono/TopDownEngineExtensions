using UnityEngine;

namespace SpeedMultipliers.Scripts
{
    public class AnimatorSpeedUpdater : MonoBehaviour
    {
        [SerializeField]
        private FloatVariable SpeedMultiplier;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        
        private void OnEnable()
        {
            SpeedMultiplier.ValueChanged += UpdateAnimatorSpeed;
            UpdateAnimatorSpeed(SpeedMultiplier.Value);
        }

        private void OnDisable()
        {
            SpeedMultiplier.ValueChanged -= UpdateAnimatorSpeed;
            UpdateAnimatorSpeed(1);
        }

        private void UpdateAnimatorSpeed(float speed)
        {
            _animator.speed = speed;
        }
    }
}

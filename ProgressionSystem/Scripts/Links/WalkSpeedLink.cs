using MoreMountains.TopDownEngine;
using ProgressionSystem.Scripts.Variables;
using UnityEngine;

namespace ProgressionSystem.Scripts.Links
{
    public class WalkSpeedLink : MonoBehaviour
    {
        [SerializeField] private FloatVariable WalkSpeed;
        private CharacterMovement _movement;

        private void Awake() { _movement = GetComponent<CharacterMovement>(); }

        private void UpdateWalkSpeed()
        {
            _movement.WalkSpeed = WalkSpeed.Value;
            _movement.ResetSpeed();
        }
        private void OnEnable()
        {
            UpdateWalkSpeed();
            WalkSpeed.Changed += UpdateWalkSpeed;
        }

        private void OnDisable()
        {
            WalkSpeed.Changed -= UpdateWalkSpeed;
        }
    }
}

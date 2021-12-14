using MoreMountains.TopDownEngine;
using UnityEngine;

namespace StatusSystem
{
    [CreateAssetMenu]
    public class TemporarySpeed : TemporaryStatusEffect
    {
        [Tooltip("How much speed to add to the character when this status effect is applied. Also works with negative numbers for a speed reducing effect.")]
        public float AdditionalSpeed = 10f;
        public override void Apply(Character character)
        {
            base.Apply(character);
            ChangeSpeed(character, AdditionalSpeed);
        }

        protected override void Unapply(Character character)
        {
            ChangeSpeed(character, -AdditionalSpeed);
        }

        private void ChangeSpeed(Character character, float speed)
        {
            var movement = character.FindAbility<CharacterMovement>();
            if (movement != null) movement.WalkSpeed += speed;
            
            var run = character.FindAbility<CharacterRun>();
            if (run != null) run.RunSpeed += speed;

            movement.MovementSpeed = character.MovementState.CurrentState == CharacterStates.MovementStates.Running ? run.RunSpeed : movement.WalkSpeed;
        }
    }
}

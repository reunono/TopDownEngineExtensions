using MoreMountains.TopDownEngine;

namespace TankControls
{
    public class CharacterRotation : CharacterAbility
    {
        public float Speed = 5;

        protected override void HandleInput() => _model.transform.Rotate(0.0f, Speed * _horizontalInput, 0.0f);
    }
}

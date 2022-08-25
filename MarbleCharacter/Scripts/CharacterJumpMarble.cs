using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterJumpMarble : CharacterAbility
{
    public float JumpForce = 100;

    protected override void HandleInput()
    {
        if (AbilityAuthorized &&
            _condition.CurrentState == CharacterStates.CharacterConditions.Normal &&
            _controller.Grounded &&
            _inputManager.JumpButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            _controller.AddForce(JumpForce * Vector3.up);
    }
}

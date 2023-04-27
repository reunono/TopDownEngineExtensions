using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

public class ToggleCrouch : CharacterCrouch
{
    private bool _crouch;

    protected override void HandleInput()
    {
        if (_inputManager.CrouchButton.State.CurrentState != MMInput.ButtonStates.ButtonDown) return;
        _crouch = !_crouch;
        if (_crouch) Crouch();
    }

    protected override void CheckExitCrouch()
    {
        if ((!_controller.Grounded || !_crouch && !ForcedCrouch) && _controller.CanGoBackToOriginalSize() && _crouching) ExitCrouch();
    }

    protected override void ExitCrouch()
    {
        base.ExitCrouch();
        _crouch = false;
    }
}

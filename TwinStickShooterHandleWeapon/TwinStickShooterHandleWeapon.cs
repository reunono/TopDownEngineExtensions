using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions
{
    [AddComponentMenu("TopDown Engine/Character/Abilities/Twin Stick Shooter Handle Weapon")]
    public class TwinStickShooterHandleWeapon : CharacterHandleWeapon
    {
        [Header("Secondary Input")]
        [Tooltip("whether or not to shoot when receiving secondary movement input")]
        public bool ShootOnSecondaryInput = true;
        [Tooltip("the minimum magnitude of the secondary input vector needed to start shooting if the above option is checked")]
        public float MinimumMagnitude = 0.6f;
        protected bool _receivedSecondaryInput;
        protected bool _secondaryInputDown;
        protected bool _secondaryInputPressed;
        protected bool _secondaryInputUp;
        protected bool _secondaryInputOff;
        protected bool _receivedSecondaryInputLastFrame;
        
        protected override void HandleInput()
        {
            if (CurrentWeapon is null || !AbilityAuthorized || _condition.CurrentState != CharacterStates.CharacterConditions.Normal) return;
            GetSecondaryInput();
            if 
            (CurrentWeapon.TriggerMode == Weapon.TriggerModes.SemiAuto && _secondaryInputUp ||
             _inputManager.ShootButton.State.CurrentState == MMInput.ButtonStates.ButtonDown ||
             _inputManager.ShootAxis == MMInput.ButtonStates.ButtonDown ||
             CurrentWeapon.TriggerMode == Weapon.TriggerModes.Auto && _secondaryInputDown ||
             ContinuousPress && CurrentWeapon.TriggerMode == Weapon.TriggerModes.Auto && (_inputManager.ShootButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed || _inputManager.ShootAxis == MMInput.ButtonStates.ButtonPressed || _secondaryInputPressed))
                ShootStart();

            if (_inputManager.ReloadButton.State.CurrentState == MMInput.ButtonStates.ButtonDown) Reload();

            if 
            (_inputManager.ShootButton.State.CurrentState == MMInput.ButtonStates.ButtonUp ||
             _inputManager.ShootAxis == MMInput.ButtonStates.ButtonUp ||
             CurrentWeapon.TriggerMode == Weapon.TriggerModes.Auto && _secondaryInputUp)
                ShootStop();

            if (CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBetweenUses && _inputManager.ShootAxis == MMInput.ButtonStates.Off && _inputManager.ShootButton.State.CurrentState == MMInput.ButtonStates.Off && _secondaryInputOff) CurrentWeapon.WeaponInputStop();

            void GetSecondaryInput()
            {
                _receivedSecondaryInput = ShootOnSecondaryInput && _inputManager.SecondaryMovement.magnitude > MinimumMagnitude;
                _secondaryInputDown = _receivedSecondaryInput && !_receivedSecondaryInputLastFrame;
                _secondaryInputPressed = _receivedSecondaryInput && _receivedSecondaryInputLastFrame;
                _secondaryInputUp = !_receivedSecondaryInput && _receivedSecondaryInputLastFrame;
                _secondaryInputOff = !_receivedSecondaryInput && !_receivedSecondaryInputLastFrame;
                _receivedSecondaryInputLastFrame = _receivedSecondaryInput;
            }
        }
    }
}

using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions
{
    [AddComponentMenu("TopDown Engine/Character/Abilities/Character Multiple Handle Weapon")] 
    public class CharacterMultipleHandleWeapon : CharacterHandleWeapon
    {
        [Header("Handle Weapon ID")]
        [Tooltip("this number determines which shoot and reload input this ability takes and what multiple handle weapon ability should equip a weapon. make sure it is unique across all CharacterMultipleHandleWeapon abilities on the current character")]
        public int MultipleHandleWeaponID = 1;
        public override int HandleWeaponID => MultipleHandleWeaponID;
        protected MultipleWeaponsInputManager _multipleWeaponsInputManager;
        protected int _inputIndex;

        protected override void Initialization()
        {
            base.Initialization();
            _multipleWeaponsInputManager = _inputManager as MultipleWeaponsInputManager;
            _inputIndex = MultipleHandleWeaponID - 1;
        }

        public override void SetInputManager(InputManager inputManager)
        {
            base.SetInputManager(inputManager);
            _multipleWeaponsInputManager = _inputManager as MultipleWeaponsInputManager;
        }

        protected override void HandleInput()
        {
            if (!AbilityAuthorized || _condition.CurrentState != CharacterStates.CharacterConditions.Normal) return;
            if (_multipleWeaponsInputManager.ShootButtons[_inputIndex].State.CurrentState == MMInput.ButtonStates.ButtonDown || _multipleWeaponsInputManager.ShootAxes[_inputIndex] == MMInput.ButtonStates.ButtonDown)
            {
                ShootStart();
            }

            if (CurrentWeapon != null)
            {
                if (ContinuousPress && CurrentWeapon.TriggerMode == Weapon.TriggerModes.Auto && _multipleWeaponsInputManager.ShootButtons[_inputIndex].State.CurrentState == MMInput.ButtonStates.ButtonPressed)
                {
                    ShootStart();
                }
                if (ContinuousPress && CurrentWeapon.TriggerMode == Weapon.TriggerModes.Auto && _multipleWeaponsInputManager.ShootAxes[_inputIndex] == MMInput.ButtonStates.ButtonPressed)
                {
                    ShootStart();
                }
            }
            
            if (_multipleWeaponsInputManager.ReloadButtons[_inputIndex].State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
                Reload();
            }

            if (_multipleWeaponsInputManager.ShootButtons[_inputIndex].State.CurrentState == MMInput.ButtonStates.ButtonUp || _multipleWeaponsInputManager.ShootAxes[_inputIndex] == MMInput.ButtonStates.ButtonUp)
            {
                ShootStop();
            }

            if (CurrentWeapon is null) return;
            if (CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBetweenUses && _multipleWeaponsInputManager.ShootAxes[_inputIndex] == MMInput.ButtonStates.Off && _multipleWeaponsInputManager.ShootButtons[_inputIndex].State.CurrentState == MMInput.ButtonStates.Off)
            {
                CurrentWeapon.WeaponInputStop();
            }
        }
    }
}

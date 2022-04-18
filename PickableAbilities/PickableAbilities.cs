using System;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class PickableAbilities : PickableItem
{
    private enum CharacterAbilities
    {
        CharacterRun,
        CharacterDash,
        CharacterJump,
        CharacterMovement,
        CharacterOrientation,
        CharacterCrouch,
        CharacterRotateCamera,
        CharacterSwap,
        CharacterSwitchModel,
        CharacterButtonActivation,
        CharacterTimeControl,
        CharacterInventory
    }
    [SerializeField]
    private CharacterAbilities[] Abilities;
    protected override bool CheckIfPickable()
    {
        _character = _collidingObject.MMGetComponentNoAlloc<Character>();
        return _character != null && _character.CharacterType == Character.CharacterTypes.Player;
    }
    
    protected override void Pick(GameObject picker)
    {
        foreach (var ability in Abilities)
            switch (ability)
            {
                case CharacterAbilities.CharacterRun:
                    _character.FindAbility<CharacterRun>()?.PermitAbility(true);
                    break;
                case CharacterAbilities.CharacterDash:
                    _character.FindAbility<CharacterDash2D>()?.PermitAbility(true);
                    _character.FindAbility<CharacterDash3D>()?.PermitAbility(true);
                    break;
                case CharacterAbilities.CharacterJump:
                    _character.FindAbility<CharacterJump2D>()?.PermitAbility(true);
                    _character.FindAbility<CharacterJump3D>()?.PermitAbility(true);
                    break;
                case CharacterAbilities.CharacterMovement:
                    _character.FindAbility<CharacterMovement>()?.PermitAbility(true);
                    break;
                case CharacterAbilities.CharacterOrientation:
                    _character.FindAbility<CharacterOrientation2D>()?.PermitAbility(true);
                    _character.FindAbility<CharacterOrientation3D>()?.PermitAbility(true);
                    break;
                case CharacterAbilities.CharacterCrouch:
                    _character.FindAbility<CharacterCrouch>()?.PermitAbility(true);
                    break;
                case CharacterAbilities.CharacterRotateCamera:
                    _character.FindAbility<CharacterRotateCamera>()?.PermitAbility(true);
                    break;
                case CharacterAbilities.CharacterSwap:
                    _character.FindAbility<CharacterSwap>()?.PermitAbility(true);
                    break;
                case CharacterAbilities.CharacterSwitchModel:
                    _character.FindAbility<CharacterSwitchModel>()?.PermitAbility(true);
                    break;
                case CharacterAbilities.CharacterButtonActivation:
                    _character.FindAbility<CharacterButtonActivation>()?.PermitAbility(true);
                    break;
                case CharacterAbilities.CharacterTimeControl:
                    _character.FindAbility<CharacterTimeControl>()?.PermitAbility(true);
                    break;
                case CharacterAbilities.CharacterInventory:
                    _character.FindAbility<CharacterInventory>()?.PermitAbility(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
    }
}

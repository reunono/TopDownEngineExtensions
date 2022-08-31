using System;
using MoreMountains.TopDownEngine;

public static class CharacterAbilitiesUtils
{
    public static void PermitAbilities(Character character, CharacterAbilities[] abilities, bool permit = true)
    {
        foreach (var ability in abilities)
            switch (ability)
            {
                case CharacterAbilities.CharacterRun:
                    character.FindAbility<CharacterRun>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterDash:
                    character.FindAbility<CharacterDash2D>()?.PermitAbility(permit);
                    character.FindAbility<CharacterDash3D>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterJump:
                    character.FindAbility<CharacterJump2D>()?.PermitAbility(permit);
                    character.FindAbility<CharacterJump3D>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterMovement:
                    character.FindAbility<CharacterMovement>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterOrientation:
                    character.FindAbility<CharacterOrientation2D>()?.PermitAbility(permit);
                    character.FindAbility<CharacterOrientation3D>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterCrouch:
                    character.FindAbility<CharacterCrouch>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterRotateCamera:
                    character.FindAbility<CharacterRotateCamera>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterSwap:
                    character.FindAbility<CharacterSwap>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterSwitchModel:
                    character.FindAbility<CharacterSwitchModel>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterButtonActivation:
                    character.FindAbility<CharacterButtonActivation>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterTimeControl:
                    character.FindAbility<CharacterTimeControl>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterInventory:
                    character.FindAbility<CharacterInventory>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterStun:
                    character.FindAbility<CharacterStun>()?.PermitAbility(permit);
                    break;
                case CharacterAbilities.CharacterHandleWeapon:
                    character.FindAbility<CharacterHandleWeapon>()?.PermitAbility(permit);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
    }
}

using System;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class BlockAbilitiesWhileAttacking : MonoBehaviour, MMEventListener<MMStateChangeEvent<Weapon.WeaponStates>>
{
    public CharacterAbilities[] AbilitiesToBlock;
    private Character _character;
    private CharacterHandleWeapon[] _characterHandleWeapons;
    private int _weaponsInUse;

    private int WeaponsInUse
    {
        get => _weaponsInUse;
        set
        {
            var newValue = Math.Max(value, 0);
            if (newValue == _weaponsInUse) return;
            var oldValue = _weaponsInUse;
            _weaponsInUse = newValue;
            if (oldValue == 0) CharacterAbilitiesUtils.PermitAbilities(_character, AbilitiesToBlock, false);
            else if (newValue == 0) CharacterAbilitiesUtils.PermitAbilities(_character, AbilitiesToBlock);
        }
    }

    private void Awake()
    {
        _character = GetComponentInParent<Character>();
        _characterHandleWeapons = _character.GetComponentsInChildren<CharacterHandleWeapon>();
    }

    private void OnEnable()
    {
        this.MMEventStartListening();
    }
    
    private void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(MMStateChangeEvent<Weapon.WeaponStates> weaponStateChangeEvent)
    {
        foreach (var characterHandleWeapon in _characterHandleWeapons)
        {
            if (!characterHandleWeapon.CurrentWeapon || weaponStateChangeEvent.Target != characterHandleWeapon.CurrentWeapon.gameObject) continue;
            switch (weaponStateChangeEvent.NewState)
            {
                case Weapon.WeaponStates.WeaponStart:
                    WeaponsInUse++;
                    break;
                case Weapon.WeaponStates.WeaponStop:
                    WeaponsInUse--;
                    break;
            }
        }
    }
}

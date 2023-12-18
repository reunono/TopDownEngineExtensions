using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterWeaponMagazineMemorizer : MonoBehaviour
{
    private CharacterHandleWeapon[] _characterHandleWeapons;
    private readonly Dictionary<string, int> _weaponAmmoLoaded = new Dictionary<string, int>();

    private readonly Dictionary<CharacterHandleWeapon, CharacterHandleWeapon.OnWeaponChangeDelegate>
        _weaponChangeActions = new Dictionary<CharacterHandleWeapon, CharacterHandleWeapon.OnWeaponChangeDelegate>();

    private void Awake()
    {
        _characterHandleWeapons = GetComponentInParent<Character>().GetComponentsInChildren<CharacterHandleWeapon>();
        foreach (var characterHandleWeapon in _characterHandleWeapons)
        {
            _weaponChangeActions[characterHandleWeapon] = () =>
            {
                var currentWeapon = characterHandleWeapon.CurrentWeapon;
                if (!currentWeapon) return;
                currentWeapon.InitializeOnStart = false;
                if (_weaponAmmoLoaded.TryGetValue(currentWeapon.name, out var ammoLoaded))
                    currentWeapon.CurrentAmmoLoaded = ammoLoaded;
            };
        }
    }

    private void OnEnable()
    {
        foreach (var characterHandleWeapon in _characterHandleWeapons)
            characterHandleWeapon.OnWeaponChange += _weaponChangeActions[characterHandleWeapon];
    }

    private void OnDisable()
    {
        foreach (var characterHandleWeapon in _characterHandleWeapons)
            characterHandleWeapon.OnWeaponChange -= _weaponChangeActions[characterHandleWeapon];
    }

    private void Update()
    {
        foreach (var characterHandleWeapon in _characterHandleWeapons)
        {
            var currentWeapon = characterHandleWeapon.CurrentWeapon;
            if (currentWeapon)
                _weaponAmmoLoaded[currentWeapon.name] = currentWeapon.CurrentAmmoLoaded;
        }
    }
}
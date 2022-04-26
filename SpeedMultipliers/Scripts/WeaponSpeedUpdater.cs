using MoreMountains.TopDownEngine;
using UnityEngine;

namespace SpeedMultipliers.Scripts
{
    public class WeaponSpeedUpdater : MonoBehaviour
    {
        private struct InitialWeaponsStats
        {
            public Weapon Weapon;
            public float DelayBeforeUse;
            public float TimeBetweenUses;
            public float BurstTimeBetweenShots;
            public float ReloadTime;
        }
        [SerializeField]
        private FloatVariable SpeedMultiplier;
        private InitialWeaponsStats[] _initialWeaponsStats;

        private void Awake()
        {
            var weapons = GetComponents<Weapon>();
            _initialWeaponsStats = new InitialWeaponsStats[weapons.Length];
            for (var i = 0; i < weapons.Length; i++)
            {
                var weapon = weapons[i];
                _initialWeaponsStats[i].Weapon = weapon;
                _initialWeaponsStats[i].DelayBeforeUse = weapon.DelayBeforeUse;
                _initialWeaponsStats[i].TimeBetweenUses = weapon.TimeBetweenUses;
                _initialWeaponsStats[i].BurstTimeBetweenShots = weapon.BurstTimeBetweenShots;
                _initialWeaponsStats[i].ReloadTime = weapon.ReloadTime;
            }
        }

        private void OnEnable()
        {
            SpeedMultiplier.ValueChanged += UpdateWeaponSpeed;
            UpdateWeaponSpeed(SpeedMultiplier.Value);
        }
        
        private void OnDisable()
        {
            SpeedMultiplier.ValueChanged -= UpdateWeaponSpeed;
            UpdateWeaponSpeed(1);
        }

        private void UpdateWeaponSpeed(float speed)
        {
            foreach (var initialWeaponStats in _initialWeaponsStats)
            {
                initialWeaponStats.Weapon.DelayBeforeUse = initialWeaponStats.DelayBeforeUse / speed;
                initialWeaponStats.Weapon.TimeBetweenUses = initialWeaponStats.TimeBetweenUses / speed;
                initialWeaponStats.Weapon.BurstTimeBetweenShots = initialWeaponStats.BurstTimeBetweenShots / speed;
                initialWeaponStats.Weapon.ReloadTime = initialWeaponStats.ReloadTime / speed;
            }
        }
    }
}

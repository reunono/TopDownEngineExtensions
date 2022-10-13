using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TemporaryEffects
{
    public class WeaponTimeBetweenUsesMultiplier : MonoBehaviour
    {
        public Multipliers Multipliers;
        private Weapon _weapon;
        private float _initialTimeBetweenUses;

        private void Awake()
        {
            _weapon = GetComponent<Weapon>();
            _initialTimeBetweenUses = _weapon.TimeBetweenUses;
        }

        private void OnEnable() => Multipliers.OnChange += UpdateTimeBetweenUses;
        private void OnDisable() => Multipliers.OnChange -= UpdateTimeBetweenUses;
        private void UpdateTimeBetweenUses()
        {
            _weapon.WeaponState.ChangeState(Weapon.WeaponStates.WeaponUse);
            _weapon.TimeBetweenUses = _initialTimeBetweenUses * Multipliers.TotalMultiplier;
        }
    }
}

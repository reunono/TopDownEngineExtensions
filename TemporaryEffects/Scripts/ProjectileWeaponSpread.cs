using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TemporaryEffects
{
    public class ProjectileWeaponSpread : MonoBehaviour
    {
        public ProjectileWeaponSpreadEvents ProjectileWeaponSpreadEvents;
        private ProjectileWeapon _projectileWeapon;
        private Vector3 _initialSpread;

        private void Awake()
        {
            _projectileWeapon = GetComponent<ProjectileWeapon>();
            _initialSpread = _projectileWeapon.Spread;
        }

        private void OnEnable()
        {
            ProjectileWeaponSpreadEvents.OnSetSpread += SetSpread;
            ProjectileWeaponSpreadEvents.OnResetSpread += ResetSpread;
        }

        private void OnDisable()
        {
            ProjectileWeaponSpreadEvents.OnSetSpread -= SetSpread;
            ProjectileWeaponSpreadEvents.OnResetSpread -= ResetSpread;
        }

        private void SetSpread(Vector3 spread) => _projectileWeapon.Spread = spread;
        private void ResetSpread() => _projectileWeapon.Spread = _initialSpread;
    }
}

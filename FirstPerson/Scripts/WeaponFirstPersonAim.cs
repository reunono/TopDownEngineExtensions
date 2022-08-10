using MoreMountains.TopDownEngine;
using UnityEngine;

public class WeaponFirstPersonAim : MonoBehaviour
{
    private WeaponAim3D _weaponAim;
    private Camera _camera;

    private void Awake()
    {
        _weaponAim = GetComponent<WeaponAim3D>();
        _weaponAim.MoveCameraTargetTowardsReticle = false;
        _weaponAim.AimControl = WeaponAim.AimControls.Script;
        _weaponAim.Unrestricted3DAim = true;
        _camera = Camera.main;
    }

    private void Update()
    {
        _weaponAim.SetCurrentAim(_camera.transform.forward);
    }
}

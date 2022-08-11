using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class PerspectiveCharacterHandleWeapon : CharacterHandleWeapon, MMEventListener<PerspectiveChangeEvent>
{
    private Perspectives _perspective;
    private WeaponAim.AimControls _originalAimControl;
    private bool _originalMoveCameraTargetTowardsReticle;
    private bool _originalUnrestricted3DAim;
    private Camera _camera;

    protected override void Awake()
    {
        base.Awake();
        _camera = Camera.main;
        this.MMEventStartListening();
    }

    private void OnDestroy()
    {
        this.MMEventStopListening();
    }

    public override void ChangeWeapon(Weapon newWeapon, string weaponID, bool combo = false)
    {
        base.ChangeWeapon(newWeapon, weaponID, combo);
        if (newWeapon != null && _perspective == Perspectives.FirstPerson)
            ChangeWeaponAimForFirstPerson();
    }

    private void ChangeWeaponAimForFirstPerson()
    {
        if (_weaponAim == null) return;
        var weaponAim = (WeaponAim3D)_weaponAim;
        _originalAimControl = weaponAim.AimControl;
        _originalUnrestricted3DAim = weaponAim.Unrestricted3DAim;
        _originalMoveCameraTargetTowardsReticle = weaponAim.MoveCameraTargetTowardsReticle;
        
        weaponAim.MoveCameraTargetTowardsReticle = false;
        weaponAim.AimControl = WeaponAim.AimControls.Script;
        weaponAim.Unrestricted3DAim = true;
    }

    private void RestoreOriginalWeaponAimSettings()
    {
        if (_weaponAim == null) return;
        var weaponAim = (WeaponAim3D)_weaponAim;
        weaponAim.AimControl = _originalAimControl;
        weaponAim.Unrestricted3DAim = _originalUnrestricted3DAim;
        weaponAim.MoveCameraTargetTowardsReticle = _originalMoveCameraTargetTowardsReticle;
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();
        if (_weaponAim != null && _perspective == Perspectives.FirstPerson)
            _weaponAim.SetCurrentAim(_camera.transform.forward);
    }

    public void OnMMEvent(PerspectiveChangeEvent perspectiveChangeEvent)
    {
        var newPerspective = perspectiveChangeEvent.NewPerspective;
        if (newPerspective == _perspective) return;
        _perspective = newPerspective;
        if (_perspective == Perspectives.FirstPerson) ChangeWeaponAimForFirstPerson();
        else RestoreOriginalWeaponAimSettings();
    }
}

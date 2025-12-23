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
        if (newWeapon != null && _perspective is Perspectives.FirstPerson or Perspectives.ThirdPerson)
            ChangeWeaponAimForFirstOrThirdPerson();
    }

    private void ChangeWeaponAimForFirstOrThirdPerson()
    {
        if (_weaponAim == null) return;
        var weaponAim = (WeaponAim3D)_weaponAim;
        _originalAimControl = weaponAim.AimControl;
        _originalUnrestricted3DAim = weaponAim.Unrestricted3DAim;
        _originalMoveCameraTargetTowardsReticle = weaponAim.MoveCameraTargetTowardsReticle;
        
        weaponAim.MoveCameraTargetTowardsReticle = false;
        weaponAim.AimControl = WeaponAim.AimControls.Script;
        weaponAim.Unrestricted3DAim = true;
        if (weaponAim.ReticleInstance.TryGetComponent<MMUIFollowMouse>(out var follow)) follow.enabled = false;
        weaponAim.ReticleInstance.transform.localPosition = Vector3.zero;
    }

    private void RestoreOriginalWeaponAimSettings()
    {
        if (_weaponAim == null) return;
        var weaponAim = (WeaponAim3D)_weaponAim;
        weaponAim.AimControl = _originalAimControl;
        weaponAim.Unrestricted3DAim = _originalUnrestricted3DAim;
        weaponAim.MoveCameraTargetTowardsReticle = _originalMoveCameraTargetTowardsReticle;
        if (weaponAim.ReticleInstance.TryGetComponent<MMUIFollowMouse>(out var follow)) follow.enabled = true;
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();
        if (_weaponAim != null && _perspective is Perspectives.FirstPerson or Perspectives.ThirdPerson)
            _weaponAim.SetCurrentAim(_camera.transform.forward);
    }

    public void OnMMEvent(PerspectiveChangeEvent perspectiveChangeEvent)
    {
        var newPerspective = perspectiveChangeEvent.NewPerspective;
        if (newPerspective == _perspective) return;
        var oldPerspective = _perspective;
        _perspective = newPerspective;
        if (oldPerspective == Perspectives.TopDown && _perspective is Perspectives.FirstPerson or Perspectives.ThirdPerson) ChangeWeaponAimForFirstOrThirdPerson();
        else if (_perspective == Perspectives.TopDown) RestoreOriginalWeaponAimSettings();
    }
}

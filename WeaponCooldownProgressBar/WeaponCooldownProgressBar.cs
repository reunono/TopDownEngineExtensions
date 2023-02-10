using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class WeaponCooldownProgressBar : MonoBehaviour, MMEventListener<MMStateChangeEvent<Weapon.WeaponStates>>, MMEventListener<MMCameraEvent>
{
    private float _counter;
    private float _cooldown;
    private MMProgressBar _progressBar;
    private CharacterHandleWeapon _player;

    private void Awake()
    {
        _progressBar = GetComponent<MMProgressBar>();
        this.MMEventStartListening<MMStateChangeEvent<Weapon.WeaponStates>>();
        this.MMEventStartListening<MMCameraEvent>();
    }
    
    private void OnDestroy()
    {
        this.MMEventStopListening<MMStateChangeEvent<Weapon.WeaponStates>>();
        this.MMEventStopListening<MMCameraEvent>();
    }

    public void OnMMEvent(MMStateChangeEvent<Weapon.WeaponStates> weaponStateChangeEvent)
    {
        var weapon = _player.CurrentWeapon;
        if (weaponStateChangeEvent.Target != weapon.gameObject ||
            weaponStateChangeEvent.NewState != Weapon.WeaponStates.WeaponUse) return;
        _cooldown = weapon.TimeBetweenUses + (weapon.UseBurstMode ? (weapon.BurstLength-1) * weapon.BurstTimeBetweenShots : 0);
        enabled = true;
    }

    public void OnMMEvent(MMCameraEvent cameraEvent)
    {
        if (cameraEvent.EventType != MMCameraEventTypes.SetTargetCharacter) return;
        _player = cameraEvent.TargetCharacter.FindAbility<CharacterHandleWeapon>();
        enabled = _player.CurrentWeapon;
    }

    private void Update()
    {
        _counter += Time.deltaTime;
        _progressBar.SetBar01(Mathf.Lerp(0, 1, _counter / _cooldown));
        if (_counter < _cooldown) return;
        _counter = 0;
        enabled = false;
    }
}

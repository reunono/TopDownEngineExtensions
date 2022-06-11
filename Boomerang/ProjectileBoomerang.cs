using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class ProjectileBoomerang : MonoBehaviour
{
    private Projectile _projectile;
    private DamageOnTouch _damageOnTouch;
    private Health _health;
    private bool _is3DProjectile;

    private void Awake()
    {
        _projectile = GetComponent<Projectile>();
        _damageOnTouch = GetComponent<DamageOnTouch>();
        _health = GetComponent<Health>();
        _is3DProjectile = GetComponent<Collider>() != null;
    }

    private void Update()
    {
        if (_projectile.Speed > 0 && _projectile.Acceleration < 0) return;
        if (_projectile.Acceleration < 0) _projectile.Acceleration *= -1;
        var ownerDirection = _damageOnTouch.Owner.transform.position - transform.position;
        ownerDirection = _is3DProjectile ? ownerDirection.MMSetY(0) : ownerDirection.MMSetZ(0);
        if (ownerDirection.sqrMagnitude < 1) _health.Kill();
        _projectile.Direction = ownerDirection.normalized;
    }

    private void OnEnable()
    {
        // we need the projectile to stop before it returns to the owner
        // so we make sure it decelerates
        if (_projectile.Acceleration == 0) _projectile.Acceleration = -100;
        else if (_projectile.Acceleration > 0) _projectile.Acceleration *= -1;
    }
}

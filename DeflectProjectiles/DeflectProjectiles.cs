using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using UnityEngine;

public class DeflectProjectiles : MonoBehaviour, MMEventListener<MMDamageTakenEvent>
{
    public void OnMMEvent(MMDamageTakenEvent damageTakenEvent)
    {
        if (damageTakenEvent.Instigator == damageTakenEvent.AffectedHealth.gameObject ||
            !damageTakenEvent.Instigator.TryGetComponent<Projectile>(out var projectile)) return;
        LayerMask layerMask = projectile.TryGetComponent<DamageOnTouch>(out var damage) ? damage.TargetLayerMask : -1;
        const float raycastDistance = 10f;
        var direction = projectile.Direction;
        var position = projectile.transform.position - direction.normalized;
        Vector3 newDirection;
        var _3D = projectile.TryGetComponent<Collider>(out _);
        if (_3D) newDirection = Physics.Raycast(position, direction, out var hit, raycastDistance, layerMask, QueryTriggerInteraction.Collide) ? Vector3.Reflect(direction,  hit.normal) : -direction;
        else
        {
            var hit = Physics2D.Raycast(position, direction, raycastDistance, layerMask);
            newDirection = hit.collider ? Vector2.Reflect(direction, hit.normal) : -direction;
        }
        projectile.SetDirection(newDirection, Quaternion.LookRotation(newDirection, _3D ? Vector3.up : Vector3.forward));
    }
    private void OnEnable() => this.MMEventStartListening();
    private void OnDisable() => this.MMEventStopListening();
}

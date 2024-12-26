using System.Collections;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class DamageOnTouchWithMissFeedback : DamageOnTouch
{
    [MMInspectorGroup("Feedbacks", true, 18)]
    public MMF_Player MissFeedback;
    private bool _hit;
    private Collider _collider;
    private Collider2D _collider2D;

    protected override void GrabComponents()
    {
        base.GrabComponents();
        _collider = GetComponent<Collider>();
        _collider2D = GetComponent<Collider2D>();
    }

    public override void InitializeFeedbacks()
    {
        base.InitializeFeedbacks();
        MissFeedback?.Initialization();
    }

    protected override void OnAnyCollision(GameObject other)
    {
        base.OnAnyCollision(other);
        _hit = true;
    }

    private void OnAttackStop()
    {
        if (!_hit) MissFeedback?.PlayFeedbacks(transform.position);
        _hit = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(DetectAttackStop());

        IEnumerator DetectAttackStop()
        {
            yield return null;
            var colliderEnabled = ColliderEnabled();
            while (true)
            {
                yield return null;
                var colliderWasEnabled = colliderEnabled;
                colliderEnabled = ColliderEnabled();
                if (colliderWasEnabled && !colliderEnabled) OnAttackStop();
            }
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (ColliderEnabled()) OnAttackStop();
        StopAllCoroutines();
    }
    private bool ColliderEnabled() => _collider && _collider.enabled || _collider2D && _collider2D.enabled;
}

using MoreMountains.Tools;
using UnityEngine;

public static class TargetDetector
{
    public static Collider GetTarget3D(Vector3 position, Vector3 direction, float maxDistance, float maxAngle, int numberOfRays, LayerMask target, LayerMask obstacle)
    {
        var targetObstacleMask = target | obstacle;
        RaycastHit hitInfo;
        float step;
        float start;
        if (numberOfRays % 2 == 1)
        {
            TryGetTarget(0);
            numberOfRays--;
            step = maxAngle / numberOfRays;
            start = step;
        }
        else
        {
            step = maxAngle / (numberOfRays + 1);
            start = step / 2;
        }

        for (var i = 0; i < numberOfRays/2; i++)
        {
            var angle = start + i * step;
            if (TryGetTarget(-angle)) return hitInfo.collider;
            if (TryGetTarget(angle)) return hitInfo.collider;
        }

        return null;
        bool TryGetTarget(float angle) => Physics.Raycast(position, Quaternion.Euler(0, angle, 0) * direction, out hitInfo, maxDistance, targetObstacleMask) && !obstacle.MMContains(hitInfo.collider.gameObject);
    }

    public static Collider2D GetTarget2D(Vector3 position, Vector3 direction, float maxDistance, float maxAngle, int numberOfRays, LayerMask target, LayerMask obstacle)
    {
        var targetObstacleMask = target | obstacle;
        RaycastHit2D hitInfo;
        float step;
        float start;
        if (numberOfRays % 2 == 1)
        {
            TryGetTarget(0);
            numberOfRays--;
            step = maxAngle / numberOfRays;
            start = step;
        }
        else
        {
            step = maxAngle / (numberOfRays + 1);
            start = step / 2;
        }

        for (var i = 0; i < numberOfRays/2; i++)
        {
            var angle = start + i * step;
            if (TryGetTarget(-angle)) return hitInfo.collider;
            if (TryGetTarget(angle)) return hitInfo.collider;
        }

        return null;
        bool TryGetTarget(float angle)
        {
            hitInfo = Physics2D.Raycast(position, Quaternion.Euler(0, 0, angle) * direction, maxDistance, targetObstacleMask);
            return hitInfo.collider && !obstacle.MMContains(hitInfo.collider.gameObject);
        }
    }
}

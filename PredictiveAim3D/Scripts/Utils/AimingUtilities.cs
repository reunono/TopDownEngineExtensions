using UnityEngine;

namespace TopDownEngineExtensions.PredictiveAim3D.Scripts.Utils
{
    public static class AimingUtilities
    {
        public static bool CheckForNaN(this Vector3 v)
        {
            return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPos">Current position of the target.</param>
        /// <param name="targetVelocity">Current velocity of the target.</param>
        /// <param name="bulletPos">Initial position of the projectile when fired.</param>
        /// <param name="bulletSpeed">The speed that the shot will fire at once launched.</param>
        /// <param name="gravity">If supplied, gravity is accounted for with a slightly more expensive equation.</param>
        /// <returns></returns>
        public static Vector3 GetLaunchVector(Vector3 targetPos, Vector3 targetVelocity, Vector3 bulletPos, float bulletSpeed, Vector3? gravity = null)
        {
            // t = [ -2*D*St*cos(theta) ± Sqrt[ (2*D*St*cos(theta))^2 + 4*(Sb^2 - St^2)*D^2 ] ] / (2*(Sb^2 - St^2))
            var distanceSqr = Vector3.SqrMagnitude(targetPos - bulletPos);
            var distance = Mathf.Sqrt(distanceSqr);
            var targetSpeedSqr = targetVelocity.sqrMagnitude;
            var targetSpeed = Mathf.Sqrt(targetSpeedSqr);

            var cosTheta = Vector3.Dot(Vector3.Normalize(bulletPos - targetPos), Vector3.Normalize(targetVelocity));

            var speedSqrDiff = (bulletSpeed * bulletSpeed) - targetSpeedSqr;
            var travelTimeP1 = -2f * distance * targetSpeed * cosTheta;
            var travelTimeP2 = Mathf.Sqrt((travelTimeP1 * travelTimeP1) + (4f * speedSqrDiff * distanceSqr));
            var travelTimeP3 = 2f * speedSqrDiff;

            // +/- for 2 intersections.  Choose lower
            var travelTimePlus = (travelTimeP1 + travelTimeP2) / travelTimeP3;
            var travelTimeMinus = (travelTimeP1 - travelTimeP2) / travelTimeP3;
            var travelTime = Mathf.Min(travelTimePlus, travelTimeMinus);
            if (0 >= travelTime)
            {
                travelTime = Mathf.Max(travelTimePlus, travelTimeMinus);
            }

            var launchVector = gravity.HasValue
                ? targetVelocity - gravity.Value * (0.5f * travelTime) + (targetPos - bulletPos) / travelTime
                : targetVelocity + (targetPos - bulletPos) / travelTime;

            return launchVector;
        }
    }
}
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace ProgressionSystem
{
    [FeedbackPath("UI/Level Up Floating Text")]
    public class LevelUpFloatingTextFeedback : MMFeedbackFloatingText
    {
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active) return;
            var intensityMultiplier = Timing.ConstantIntensity ? 1f : feedbacksIntensity;
            switch (PositionMode)
            {
                case PositionModes.FeedbackPosition:
                    _playPosition = transform.position;
                    break;
                case PositionModes.PlayPosition:
                    _playPosition = position;
                    break;
                case PositionModes.TargetTransform:
                    _playPosition = TargetTransform.position;
                    break;
            }
            _value = UseIntensityAsValue ? "LEVEL "+feedbacksIntensity+"!" : Value;
            MMFloatingTextSpawnEvent.Trigger(Channel, _playPosition, _value, Direction, Intensity * intensityMultiplier, ForceLifetime, Lifetime, ForceColor, AnimateColorGradient, Timing.TimescaleMode == TimescaleModes.Unscaled);
        }
    }
}

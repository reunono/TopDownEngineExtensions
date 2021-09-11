using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

namespace TopDownEngineExtensions
{
    public class AIDecisionTargetHasHealth : AIDecision
    {
        public override bool Decide()
        {
            var targetHealth = _brain.Target.gameObject.MMGetComponentNoAlloc<Health>();
            if (targetHealth == null)
            {
                _brain.Target = null;
                return false;
            }

            if (targetHealth.MasterHealth != null)
            {
                if (targetHealth.MasterHealth.CurrentHealth > 0) return true;
            }
            else if (targetHealth.CurrentHealth > 0) return true;
            _brain.Target = null;
            return false;
        }
    }
}

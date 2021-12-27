using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace StatusSystem
{
    public class StatusEffectTester : MonoBehaviour
    {
        public StatusEffect StatusEffect;
        public Character Character;
        
        [MMInspectorButton("ApplyStatusEffectTest")]
        public bool ApplyStatusEffectTestButton;
        
        [MMInspectorButton("StatusEffectStartFeedbacksTest")]
        public bool StatusEffectStartFeedbacksTestButton;
        
        [MMInspectorButton("StatusEffectStopFeedbacksTest")]
        public bool StatusEffectStopFeedbacksTestButton;
        
        private void ApplyStatusEffectTest()
        {
            StatusEffectEvent.Trigger(StatusEffect, Character, StatusEffectEventTypes.Apply);
        }
        
        private void StatusEffectStartFeedbacksTest()
        {
            StatusEffectEvent.Trigger(StatusEffect, Character, StatusEffectEventTypes.Start);
        }
        
        private void StatusEffectStopFeedbacksTest()
        {
            StatusEffectEvent.Trigger(StatusEffect, Character, StatusEffectEventTypes.Stop);
        }
    }
}

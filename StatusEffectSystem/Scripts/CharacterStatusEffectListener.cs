using System;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace StatusSystem
{
    public class CharacterStatusEffectListener : MonoBehaviour, MMEventListener<StatusEffectEvent>
    {
        private Character _character;

        [Serializable]
        public struct StatusEffectFeedbacks
        {
            public StatusEffect StatusEffect;
            public MMFeedbacks StatusEffectStartFeedbacks;
            public MMFeedbacks StatusEffectStopFeedbacks;

            public void OnApply(StatusEffect statusEffect, Character character)
            {
                if (statusEffect != StatusEffect) return;
                StatusEffect.Apply(character);
            }

            public void OnStart(StatusEffect statusEffect)
            {
                if (statusEffect != StatusEffect) return;
                StatusEffectStartFeedbacks?.PlayFeedbacks();
            }
            
            public void OnStop(StatusEffect statusEffect)
            {
                if (statusEffect != StatusEffect) return;
                StatusEffectStopFeedbacks?.PlayFeedbacks();
            }
        }

        public StatusEffectFeedbacks[] StatusEffectsAndFeedbacks;

        private void Awake()
        {
            _character = GetComponentInParent<Character>();
        }

        public void OnMMEvent(StatusEffectEvent statusEffectEvent)
        {
            if (statusEffectEvent.Character != _character) return;
            switch (statusEffectEvent.Type)
            {
                case StatusEffectEventTypes.Apply:
                    foreach (var statusEffectsFeedback in StatusEffectsAndFeedbacks)
                        statusEffectsFeedback.OnApply(statusEffectEvent.StatusEffect, _character);
                    break;
                case StatusEffectEventTypes.Start:
                    foreach (var statusEffectsFeedback in StatusEffectsAndFeedbacks)
                        statusEffectsFeedback.OnStart(statusEffectEvent.StatusEffect);
                    break;
                case StatusEffectEventTypes.Stop:
                    foreach (var statusEffectsFeedback in StatusEffectsAndFeedbacks)
                        statusEffectsFeedback.OnStop(statusEffectEvent.StatusEffect);
                    break;
            }
        }

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }
    }
}

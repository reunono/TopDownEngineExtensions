using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace StatusSystem
{
    public enum StatusEffectEventTypes
    {
        Apply,
        Unapply,
        Start,
        Stop
    }
    
    public struct StatusEffectEvent
    {
        public StatusEffectEventTypes Type;
        public StatusEffect StatusEffect;
        public Character Character;

        static StatusEffectEvent e;
        public static void Trigger(StatusEffect statusEffect, Character character, StatusEffectEventTypes type)
        {
            e.StatusEffect = statusEffect;
            e.Character = character;
            e.Type = type;
            MMEventManager.TriggerEvent(e);
        }
    }
    
    public abstract class StatusEffect : ScriptableObject, MMEventListener<StatusEffectEvent>
    {
        public abstract void Apply(Character character);
        protected abstract void Unapply(Character character);

        public void OnMMEvent(StatusEffectEvent statusEffectEvent)
        {
            if (statusEffectEvent.StatusEffect == this &&
                statusEffectEvent.Type == StatusEffectEventTypes.Unapply)
                Unapply(statusEffectEvent.Character);
        }
        
        protected virtual void OnEnable()
        {
            this.MMEventStartListening();
        }
        
        protected virtual void OnDisable()
        {
            this.MMEventStopListening();
        }
    }
}

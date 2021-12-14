using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace StatusSystem
{
    public enum StatusEffectEventTypes
    {
        Apply,
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
    
    public abstract class StatusEffect : ScriptableObject
    {
        public abstract void Apply(Character character);
    }
}

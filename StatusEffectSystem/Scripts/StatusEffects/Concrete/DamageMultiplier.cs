using MoreMountains.TopDownEngine;
using UnityEngine;

namespace StatusSystem
{
    [CreateAssetMenu(fileName = "DamageMultiplier", menuName = "Status System/Status Effects/Simple/Damage Multiplier")]
    public class DamageMultiplier : StatusEffect
    {
        public float Multiplier = 2f;
        
        private CharacterDamageMultipliers _multipliers;
        protected override void OnEnable()
        {
            base.OnEnable();
            _multipliers = Resources.Load<CharacterDamageMultipliers>("RuntimeSets/CharacterDamageMultipliers");
        }

        public override void Apply(Character character)
        {
            StatusEffectEvent.Trigger(this, character, StatusEffectEventTypes.Start);
            _multipliers[character] *= Multiplier;
        }

        protected override void Unapply(Character character)
        {
            StatusEffectEvent.Trigger(this, character, StatusEffectEventTypes.Stop);
            _multipliers[character] /= Multiplier;
        }
    }
}

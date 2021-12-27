using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace StatusSystem
{
    public abstract class TemporaryStatusEffect : StatusEffect
    {
        [Tooltip("The duration (in seconds) of this status effect")]
        public float Duration = 3f;
        private readonly Dictionary<Character, bool> _isStatusEffectActive = new Dictionary<Character, bool>();
        private readonly Dictionary<Character, Coroutine> _coroutines = new Dictionary<Character, Coroutine>();
        public override void Apply(Character character)
        {
            _isStatusEffectActive.TryGetValue(character, out var statusEffectActive);
            if (statusEffectActive)
            {
                character.StopCoroutine(_coroutines[character]);
                Unapply(character);
            }
            else
                StatusEffectEvent.Trigger(this, character, StatusEffectEventTypes.Start);
            _isStatusEffectActive[character] = true;
            _coroutines[character] = character.StartCoroutine(WaitForDurationThenUnapply(character));
        }

        private IEnumerator WaitForDurationThenUnapply(Character character)
        {
            yield return new WaitForSeconds(Duration);
            StatusEffectEvent.Trigger(this, character, StatusEffectEventTypes.Stop);
            _isStatusEffectActive[character] = false;
            Unapply(character);
        }
    }
}

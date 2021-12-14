using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace StatusSystem.Scripts
{
    [CreateAssetMenu]
    public class DamageOverTime : TemporaryStatusEffect
    {
        [Tooltip("How much damage (or healing in case the value is negative) will be done each interval")]
        public int DamagePerInterval = 3;
        [Tooltip("How often the effect “Ticks” on the character. Each time it ticks, it does damage or healing.")]
        public float Interval = .5f;
        private Dictionary<Character, Coroutine> _coroutines = new Dictionary<Character, Coroutine>();

        public override void Apply(Character character)
        {
            base.Apply(character);
            _coroutines[character] = character.StartCoroutine(ApplyDamageEveryTick(character));
        }

        private IEnumerator ApplyDamageEveryTick(Character character)
        {
            var health = character._health;
            while (true)
            {
                yield return new WaitForSeconds(Interval);
                if (DamagePerInterval > 0)
                    health.Damage(DamagePerInterval, character.gameObject, 0f, 0f, Vector3.zero);
                else
                    health.GetHealth(-DamagePerInterval, character.gameObject);
            }
        }

        protected override void Unapply(Character character)
        {
            character.StopCoroutine(_coroutines[character]);
        }
    }
}

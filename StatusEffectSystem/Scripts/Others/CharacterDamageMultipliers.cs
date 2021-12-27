using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace StatusSystem
{
    [CreateAssetMenu(fileName = "CharacterDamageMultipliers", menuName = "Status System/Runtime Sets/Character Damage Multipliers")]
    public class CharacterDamageMultipliers : ScriptableObject
    {
        private Dictionary<Character, float> _characterDamageMultipliers;

        public float this [Character character]
        {
            get => !_characterDamageMultipliers.ContainsKey(character) ? 1f : _characterDamageMultipliers[character];
            set => _characterDamageMultipliers[character] = value;
        }
        
        private void OnEnable()
        {
            _characterDamageMultipliers = new Dictionary<Character, float>();
        }
    }
}

using MoreMountains.TopDownEngine;
using UnityEngine;

namespace StatusSystem.Scripts
{
    [CreateAssetMenu]
    public class Freeze : TemporaryStatusEffect
    {
        public override void Apply(Character character)
        {
            base.Apply(character);
            character.Freeze();
        }

        protected override void Unapply(Character character)
        {
            character.UnFreeze();
        }
    }
}

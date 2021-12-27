using MoreMountains.TopDownEngine;
using UnityEngine;

namespace StatusSystem
{
    [CreateAssetMenu(fileName = "Freeze", menuName = "Status System/Status Effects/Temporary/Freeze")]
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

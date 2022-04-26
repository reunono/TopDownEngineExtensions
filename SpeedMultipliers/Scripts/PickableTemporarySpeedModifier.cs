using System.Collections;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace SpeedMultipliers.Scripts
{
    public class PickableTemporarySpeedModifier : PickableItem
    {
        [SerializeField]
        private FloatVariable SpeedMultiplier;
        [SerializeField]
        public float NewSpeed = .3f;
        [SerializeField]
        private float Duration = 10;
        private float _initialSpeed;

        protected override void Pick(GameObject picker)
        {
            _character.StartCoroutine(TemporarilyChangeSpeed());

            IEnumerator TemporarilyChangeSpeed()
            {
                _initialSpeed = SpeedMultiplier.Value;
                SpeedMultiplier.Value = NewSpeed;
                yield return MMCoroutine.WaitFor(Duration);
                SpeedMultiplier.Value = _initialSpeed;
            }
        }
    }
}

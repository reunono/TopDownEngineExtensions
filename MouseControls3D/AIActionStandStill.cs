using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions
{
    public class AIActionStandStill : AIAction
    {
        private CharacterMovement _characterMovement;
        protected override void Awake()
        {
            base.Awake();
            _characterMovement = GetComponentInParent<Character>().FindAbility<CharacterMovement>();
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            _characterMovement.SetMovement(Vector2.zero);
            _characterMovement.MovementForbidden = true;
        }

        public override void OnExitState()
        {
            base.OnExitState();
            _characterMovement.MovementForbidden = false;
        }

        public override void PerformAction() { }
    }
}

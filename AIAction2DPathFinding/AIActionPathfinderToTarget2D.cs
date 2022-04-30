using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Requires a CharacterMovement ability. Makes the character move up to the specified MinimumDistance in the direction of the target. 
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionPathfinderToTarget2D")]
    //[RequireComponent(typeof(CharacterMovement))]
    //[RequireComponent(typeof(CharacterPathfinder3D))]
    public class AIActionPathfinderToTarget2D : AIAction
    {
        protected CharacterMovement _characterMovement;
        protected CharacterPathfinder2D _characterPathfinder2D;

        /// <summary>
        /// On init we grab our CharacterMovement ability
        /// </summary>
        public override void Initialization()
        {
            _characterMovement = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterMovement>();
            _characterPathfinder2D = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterPathfinder2D>();
            if (_characterPathfinder2D == null)
            {
                Debug.LogWarning(this.name + " : the AIActionPathfinderToTarget3D AI Action requires the CharacterPathfinder3D ability");
            }
        }

        /// <summary>
        /// On PerformAction we move
        /// </summary>
        public override void PerformAction()
        {
            Move();
        }

        /// <summary>
        /// Moves the character towards the target if needed
        /// </summary>
        protected virtual void Move()
        {
            if (_brain.Target == null)
            {
                _characterPathfinder2D.SetNewDestination(null);
                return;
            }
            else
            {
                _characterPathfinder2D.SetNewDestination(_brain.Target.transform);
            }
        }

        /// <summary>
        /// On exit state we stop our movement
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();
            
            _characterPathfinder2D?.SetNewDestination(null);
            _characterMovement?.SetHorizontalMovement(0f);
            _characterMovement?.SetVerticalMovement(0f);
        }
    }
}

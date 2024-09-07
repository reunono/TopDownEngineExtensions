using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// Add this to seamlessly transition from jump to jetpack at the top of a jump
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/JumpToGlide")]
    [RequireComponent(typeof(CharacterJump))]
    [RequireComponent(typeof(CharacterGlideFromJump))]
    public class JumpToGlide : CharacterAbility
    {
        /// This method is only used to display a helpbox text
        /// at the beginning of the ability's inspector
        public override string HelpBoxText()
        {
            return
                "This component implements a transition from jump to glide using the jump button. Requires the <b>CharacterGlide</b> component and uses the jump state. Sounds trigger at the apex of the jump [Start] and will continue [In Progress] until you hit the ground [Stop].";
        }

        //protected CharacterJetpack _characterJetpack;
        protected CharacterGlideFromJump _characterGlideFromJump;
        protected bool _hitApex = false;
        protected bool _abilityStart = false;

        /// <summary>
        /// Here you should initialize our parameters
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            //_characterJetpack = transform.GetComponent<CharacterJetpack>();
            _characterGlideFromJump = transform.GetComponent<CharacterGlideFromJump>();
        }

        /// <summary>
        /// Every frame, we check if we've hit the high point or are grounded
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();

            if (_controller.State.IsJumping && _controller.Speed.y < 0)
            {
                _hitApex = true;
            }

            if (_controller.State.IsGrounded && _hitApex)
            {
                _hitApex = false;
                PlayAbilityStopFeedbacks();
            }
        }

        /// <summary>
        /// Called at the start of the ability's cycle, this is where you'll check for input
        /// </summary>
        protected override void HandleInput()
        {
            if (_hitApex)
            {
                if (_inputManager.JumpButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed)
                {
                    _characterGlideFromJump.GlideStart();

                    if (!_abilityStart)
                    {
                        _abilityStart = true;
                        PlayAbilityStartFeedbacks();
                    }
                }

                if (_inputManager.JumpButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
                {
                    _characterGlideFromJump.GlideStop();
                    _abilityStart = false;
                }
            }
        }
    }
}
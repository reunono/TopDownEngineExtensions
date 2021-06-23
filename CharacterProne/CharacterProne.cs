using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions
{
	/// <summary>
	/// This ability allows the character to "prone" when pressing the prone button, which resizes the collider
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Prone")]
    public class CharacterProne : CharacterAbility
    {
	    /// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component handles prone and prone moving behaviours. Here you can determine the prone move speed, and whether or not the collider should resize when prone (to prone move into tunnels for example). If it should, please setup its new size here."; }

        /// if this is true, the character is in ForcedProne mode. A ProneZone or an AI script can do that.
        [MMReadOnly]
		[Tooltip("if this is true, the character is in ForcedProne mode. A ProneZone or an AI script can do that.")]
		public bool ForcedProne = false;

		[Header("Prone Move")]

		/// if this is set to false, the character won't be able to prone move, just to prone
		[Tooltip("if this is set to false, the character won't be able to prone move, just to prone")]
		public bool ProneMoveAuthorized = true;
		/// the speed of the character when it's prone
		[Tooltip("the speed of the character when it's prone")]
		public float ProneMoveSpeed = 2f;

		[Space(10)]	
		[Header("Prone")]

		/// if this is true, the collider will be resized when proned
		[Tooltip("if this is true, the collider will be resized when proned")]
		public bool ResizeColliderWhenProned = false;
		/// the size to apply to the collider when proned (if ResizeColliderWhenProned is true, otherwise this will be ignored)
		[Tooltip("the size to apply to the collider when proned (if ResizeColliderWhenProned is true, otherwise this will be ignored)")]
		public float PronedColliderHeight = .75f;

		[Space(10)]	
		[Header("Offset")]

		/// a list of objects to offset when prone
		[Tooltip("a list of objects to offset when prone")]
		public List<GameObject> ObjectsToOffset;
		/// the offset to apply to objects when prone
		[Tooltip("the offset to apply to objects when prone")]
		public Vector3 OffsetProne;
		/// the offset to apply to objects when prone AND moving
		[Tooltip("the offset to apply to objects when prone AND moving")]
		public Vector3 OffsetProneMove;
		/// the speed at which to offset objects
		[Tooltip("the speed at which to offset objects")]
		public float OffsetSpeed = 5f;

        /// whether or not the character is in a tunnel right now and can't get up
        [MMReadOnly]
        [Tooltip("whether or not the character is in a tunnel right now and can't get up")]
		public bool InATunnel;

		protected List<Vector3> _objectsToOffsetOriginalPositions;
		protected const string _proneAnimationParameterName = "Prone";
		protected const string _proneMovingAnimationParameterName = "ProneMoving";
		protected int _proneAnimationParameter;
		protected int _proneMovingAnimationParameter;
        protected bool _prone = false;
        protected CharacterRun _characterRun;
        protected ExtendedInputManager _extendedInputManager;

        /// <summary>
        /// On Start(), we set our tunnel flag to false
        /// </summary>
        protected override void Initialization()
		{
			base.Initialization();
			_extendedInputManager = _inputManager as ExtendedInputManager;
			InATunnel = false;
			_characterRun = _character.FindAbility<CharacterRun>();

			// we store our objects to offset's initial positions
			if (ObjectsToOffset.Count > 0)
			{
				_objectsToOffsetOriginalPositions = new List<Vector3> ();
				foreach(GameObject go in ObjectsToOffset)
				{
                    if (go != null)
                    {
                        _objectsToOffsetOriginalPositions.Add(go.transform.localPosition);
                    }					
				}
			}
		}

		/// <summary>
		/// Every frame, we check if we're proned and if we still should be
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();
            HandleForcedProne();
            DetermineState ();
			CheckExitProne();
			OffsetObjects ();
		}

        /// <summary>
        /// If we're in forced prone state, we prone
        /// </summary>
        protected virtual void HandleForcedProne()
        {
            if (ForcedProne && (_movement.CurrentState != CharacterStates.MovementStates.Prone) && (_movement.CurrentState != CharacterStates.MovementStates.ProneMoving))
            {
                Prone();
            }
        }

        /// <summary>
        /// Starts a forced prone
        /// </summary>
        public virtual void StartForcedProne()
        {
            ForcedProne = true;
            _prone = true;
        }

        /// <summary>
        /// Stops a forced prone
        /// </summary>
        public virtual void StopForcedProne()
        {
            ForcedProne = false;
            _prone = false;
        }

		/// <summary>
		/// If we're pressing down, we check if we can prone or prone move, and change states accordingly
		/// </summary>
		protected virtual void Prone()
		{
			if (!AbilityAuthorized// if the ability is not permitted
			    || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)// or if we're not in our normal stance
			    || (!_controller.Grounded))// or if we're grounded
				// we do nothing and exit
			{
				return;
			}				

			// if this is the first time we're here, we trigger our sounds
			if ((_movement.CurrentState != CharacterStates.MovementStates.Prone) && (_movement.CurrentState != CharacterStates.MovementStates.ProneMoving))
			{
				// we play the prone start sound 
				PlayAbilityStartSfx();
				PlayAbilityUsedSfx();
			}

			if (_movement.CurrentState == CharacterStates.MovementStates.Running)
			{
				_characterRun.RunStop();
			}

			_prone = true;

			// we set the character's state to Prone and if it's also moving we set it to ProneMoving
			_movement.ChangeState(CharacterStates.MovementStates.Prone);
			if ( (Mathf.Abs(_horizontalInput) > 0) && (ProneMoveAuthorized) )
			{
				_movement.ChangeState(CharacterStates.MovementStates.ProneMoving);
			}

			// we resize our collider to match the new shape of our character (it's usually smaller when proned)
			if (ResizeColliderWhenProned)
			{
				_controller.ResizeColliderHeight(PronedColliderHeight);		
			}

			// we change our character's speed
			if (_characterMovement != null)
			{
				_characterMovement.MovementSpeed = ProneMoveSpeed;
			}

			// we prevent movement if we can't prone move
			if (!ProneMoveAuthorized)
			{
				_characterMovement.MovementSpeed = 0f;
			}
		}

		protected virtual void OffsetObjects ()
		{
			// we move all the objects we want to move
			if (ObjectsToOffset.Count > 0)
			{
				for (int i = 0; i < ObjectsToOffset.Count; i++)
				{
					Vector3 newOffset = Vector3.zero;
					if (_movement.CurrentState == CharacterStates.MovementStates.Prone)
					{
						newOffset = OffsetProne;
					}
					if (_movement.CurrentState == CharacterStates.MovementStates.ProneMoving)
					{
						newOffset = OffsetProneMove;
					}
                    if (ObjectsToOffset[i] != null)
                    {
                        ObjectsToOffset[i].transform.localPosition = Vector3.Lerp(ObjectsToOffset[i].transform.localPosition, _objectsToOffsetOriginalPositions[i] + newOffset, Time.deltaTime * OffsetSpeed);
                    }					
				}
			}
		}

		/// <summary>
		/// Runs every frame to check if we should switch from prone to prone moving or the other way around
		/// </summary>
		protected virtual void DetermineState()
		{
			if ((_movement.CurrentState == CharacterStates.MovementStates.Prone) || (_movement.CurrentState == CharacterStates.MovementStates.ProneMoving))
			{
				if ( (_controller.CurrentMovement.magnitude > 0) && (ProneMoveAuthorized) )
				{
					_movement.ChangeState(CharacterStates.MovementStates.ProneMoving);
				}
				else
				{
					_movement.ChangeState(CharacterStates.MovementStates.Prone);
				}
			}
		}

		/// <summary>
		/// Every frame, we check to see if we should exit the Prone (or ProneMoving) state
		/// </summary>
		protected virtual void CheckExitProne()
		{				
			// if we're currently grounded
			if ( (_movement.CurrentState == CharacterStates.MovementStates.Prone)
				|| (_movement.CurrentState == CharacterStates.MovementStates.ProneMoving)
				|| _prone)
			{	
                if (_inputManager == null)
                {
                    if (!ForcedProne)
                    {
                        ExitProne();
                    }
                    return;
                }

				// but we're not pressing down anymore, or we're not grounded anymore
				if ( (!_controller.Grounded) 
				     || ((_movement.CurrentState != CharacterStates.MovementStates.Prone) 
				         && (_movement.CurrentState != CharacterStates.MovementStates.ProneMoving)
				         && (_extendedInputManager.ProneButton.State.CurrentState == MMInput.ButtonStates.Off) && (!ForcedProne))
				     || ((_extendedInputManager.ProneButton.State.CurrentState == MMInput.ButtonStates.Off) && (!ForcedProne)))
				{
					// we cast a raycast above to see if we have room enough to go back to normal size
					InATunnel = !_controller.CanGoBackToOriginalSize();

					// if the character is not in a tunnel, we can go back to normal size
					if (!InATunnel)
					{
                        ExitProne();
                    }
				}
			}
		}

        /// <summary>
        /// Returns the character to normal stance
        /// </summary>
        protected virtual void ExitProne()
        {
	        _prone = false;
	        
            // we return to normal walking speed
            if (_characterMovement != null)
            {
                _characterMovement.ResetSpeed();
            }

            // we play our exit sound
            StopAbilityUsedSfx();
            PlayAbilityStopSfx();

            // we go back to Idle state and reset our collider's size
            if ((_movement.CurrentState == CharacterStates.MovementStates.ProneMoving) ||
                (_movement.CurrentState == CharacterStates.MovementStates.Prone))
            {
	            _movement.ChangeState(CharacterStates.MovementStates.Idle);    
            }
            
            _controller.ResetColliderSize();
        }

        protected override void HandleInput()
        {
	        base.HandleInput();
            if (_extendedInputManager.ProneButton.State.CurrentState == MMInput.ButtonStates.ButtonDown) Prone();
        }

        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter (_proneAnimationParameterName, AnimatorControllerParameterType.Bool, out _proneAnimationParameter);
            RegisterAnimatorParameter (_proneMovingAnimationParameterName, AnimatorControllerParameterType.Bool, out _proneMovingAnimationParameter);
        }
        public override void UpdateAnimator()
        {
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _proneAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Prone), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _proneMovingAnimationParameter,(_movement.CurrentState == CharacterStates.MovementStates.ProneMoving), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
        }
    }
}

using UnityEngine;
using MoreMountains.TopDownEngine;

namespace TopDownEngineExtensions
{	
	/// <summary>
	/// Add this component to a character and it'll be able to run only when the models faces the direction of movement.
	/// Animator parameters : Running
	/// </summary>
	[AddComponentMenu("TopDown Engine/Character/Abilities/Character Run When Face Direction 3D")] 
	public class CharacterRunWhenFaceDirection3D : CharacterRun
	{
		[Header("Permitted Angle")]
		[SerializeField]
		[Tooltip("The maximum angle allowed at which the character can run.")]
		private float AngleThreshold = 45f;

		/// <summary>
		/// At the beginning of each cycle, we check if we've pressed or released the run button
		/// </summary>
		protected override void HandleInput()
		{
			base.HandleInput();
			if(_movement.CurrentState == CharacterStates.MovementStates.Running && !isFacingDirection())
            {
				RunStop();
            }
		}

		/// <summary>
		/// Causes the character to start running.
		/// </summary>
		public override void RunStart()
		{
			if (!isFacingDirection()) return;
			base.RunStart();
		}


		/// <summary>
		/// Checks if the character model is facing the movement direction.
		/// </summary>
		private bool isFacingDirection()
        {
			CharacterOrientation3D orientation = _character.FindAbility<CharacterOrientation3D>();

			Vector3 modelDirection = orientation.ModelDirection.normalized;
			Vector3 movementDirection = orientation.CurrentDirection.normalized;

			float dotProduct = Vector3.Dot(modelDirection, movementDirection);

			float cosThreshold = Mathf.Cos(AngleThreshold * Mathf.Deg2Rad);

			return dotProduct >= cosThreshold;
		}
	}
}
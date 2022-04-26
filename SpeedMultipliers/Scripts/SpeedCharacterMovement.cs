using MoreMountains.TopDownEngine;
using UnityEngine;

namespace SpeedMultipliers.Scripts
{
    public class SpeedCharacterMovement : CharacterMovement
    {
        [SerializeField]
        private FloatVariable SpeedMultiplier;
        
        protected override void SetMovement()
		{
            _movementVector = Vector3.zero;
			_currentInput = Vector2.zero;

			_currentInput.x = _horizontalMovement;
			_currentInput.y = _verticalMovement;
            
            _normalizedInput = _currentInput.normalized;

            float interpolationSpeed = 1f;
            
			if ((Acceleration == 0) || (Deceleration == 0))
			{
				_lerpedInput = AnalogInput ? _currentInput : _normalizedInput;
			}
			else
			{
				if (_normalizedInput.magnitude == 0)
				{
					_acceleration = Mathf.Lerp(_acceleration, 0f, Deceleration * Time.deltaTime);
                    _lerpedInput = Vector2.Lerp(_lerpedInput, _lerpedInput * _acceleration, Time.deltaTime * Deceleration);
                    interpolationSpeed = Deceleration;
				}
				else
				{
                    _acceleration = Mathf.Lerp(_acceleration, 1f, Acceleration * Time.deltaTime);
                    _lerpedInput = AnalogInput ? Vector2.ClampMagnitude (_currentInput, _acceleration) : Vector2.ClampMagnitude(_normalizedInput, _acceleration);
                    interpolationSpeed = Acceleration;
                }
			}		
			
			_movementVector.x = _lerpedInput.x;
            _movementVector.y = 0f;
			_movementVector.z = _lerpedInput.y;

			var speedMultiplier = SpeedMultiplier != null ? SpeedMultiplier.Value : 1;
            if (InterpolateMovementSpeed)
            {
                _movementSpeed = Mathf.Lerp(_movementSpeed, MovementSpeed * ContextSpeedMultiplier * MovementSpeedMultiplier * speedMultiplier, interpolationSpeed * Time.deltaTime);
            }
            else
            {
                _movementSpeed = MovementSpeed * MovementSpeedMultiplier * ContextSpeedMultiplier * speedMultiplier;
            }

			_movementVector *= _movementSpeed;

			if (_movementVector.magnitude > MovementSpeed * ContextSpeedMultiplier * MovementSpeedMultiplier * speedMultiplier)
			{
				_movementVector = Vector3.ClampMagnitude(_movementVector, MovementSpeed);
			}

            if ((_currentInput.magnitude <= IdleThreshold) && (_controller.CurrentMovement.magnitude < IdleThreshold))
            {
                _movementVector = Vector3.zero;
            }
            
			_controller.SetMovement (_movementVector);
		}
    }
}

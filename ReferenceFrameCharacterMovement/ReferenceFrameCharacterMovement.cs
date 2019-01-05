using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;


/// <summary>
/// Specialized variant of the TopDown character movement that corrects for a reference frame camera
/// </summary>
public class ReferenceFrameCharacterMovement : CharacterMovement
{
    /// Control Style
    public Camera MovementReferenceFrame = null;


    /// <summary>
    /// Moves the controller
    /// </summary>
    protected override void SetMovement()
    {
        _movementVector = Vector3.zero;
        _currentInput = Vector2.zero;


        _currentInput.x = _horizontalMovement;
        _currentInput.y = _verticalMovement;


        if (MovementReferenceFrame != null)
        {
            Vector3 input = new Vector3(_currentInput.x, 0, _currentInput.y);
            Vector3 worldSpaceInput = (Quaternion.LookRotation(-Vector3.up, MovementReferenceFrame.transform.forward) * Quaternion.Euler(Vector3.right * -90f)) * input;
            var resulting = transform.InverseTransformVector(worldSpaceInput);
            _currentInput = new Vector2(resulting.x, resulting.z);
        }

        _normalizedInput = _currentInput.normalized;

        if ((Acceleration == 0) || (Deceleration == 0))
        {
            _lerpedInput = _currentInput;
        }
        else
        {
            if (_normalizedInput.magnitude == 0)
            {
                _acceleration = Mathf.Lerp(_acceleration, 0f, Deceleration * Time.deltaTime);
                _lerpedInput = Vector2.Lerp(_lerpedInput, _lerpedInput * _acceleration, Time.deltaTime * Deceleration);
            }
            else
            {
                _acceleration = Mathf.Lerp(_acceleration, 1f, Acceleration * Time.deltaTime);
                _lerpedInput = Vector2.ClampMagnitude(_normalizedInput, _acceleration);
            }
        }

        _movementVector.x = _lerpedInput.x;
        _movementVector.y = 0f;
        _movementVector.z = _lerpedInput.y;

        _movementVector *= MovementSpeed * MovementSpeedMultiplier;
        if (_movementVector.magnitude > MovementSpeed)
        {
            _movementVector = Vector3.ClampMagnitude(_movementVector, MovementSpeed);
        }

        if ((_currentInput.magnitude <= IdleThreshold) && (_controller.CurrentMovement.magnitude < IdleThreshold))
        {
            _movementVector = Vector3.zero;
        }

        _controller.SetMovement(_movementVector);

    }
}
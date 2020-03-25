using MoreMountains.TopDownEngine;
using UnityEngine;
using MoreMountains.Tools;

/// Add this ability to a Character to have it move on a grid in 8 Directions.
/// This will require a GridManager be present in your scene
/// DO NOT use that component and a CharacterMovement component on the same character.
/// </summary>
[RequireComponent(typeof(AdvancedTopDownController))]
[AddComponentMenu("TopDown Engine/Character/Abilities/Advanced Character Grid Movement")]
public class AdvancedCharacterGridMovement : CharacterAbility
{
    /// the possible directions on the grid
    public enum GridDirections { None, Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight }

    [Header("Movement")]
    /// the maximum speed of the character
    public float MaximumSpeed = 8;
    /// the acceleration of the character
    public float Acceleration = 5;
    /// the current speed at which the character is going
    [MMReadOnly]
    public float CurrentSpeed;
    /// a multiplier to apply to the maximum speed
    [MMReadOnly]
    public float MaximumSpeedMultiplier = 1f;
    /// a multiplier to apply to the acceleration, letting you modify it safely from outside
    [MMReadOnly]
    public float AccelerationMultiplier = 1f;

    [Header("Input Settings")]
    /// whether or not input should be buffered (to anticipate the next turn)
    public bool UseInputBuffer = true;
    /// the size of the input buffer (in grid units)
    public int BufferSize = 2;
    /// whether or not the agent can perform fast direction changes such as U turns
    public bool FastDirectionChanges = true;
    /// the speed threshold after which the character is not considered idle anymore
    public float IdleThreshold = 0.05f;

    public bool DiaginalMovement = true;

    public bool PushBlocks = true;

    [Header("Grid")]
    /// the offset to apply when detecting obstacles
    public Vector3 ObstacleDetectionOffset = new Vector3(0f, 0.5f, 0f);
    /// the position of the object on the grid
    public Vector3 CurrentGridPosition { get; protected set; }
    /// the position the object will be at when it reaches its next perfect tile
    public Vector3 TargetGridPosition { get; protected set; }

    [MMReadOnly]
    /// this is true everytime a character is at the exact position of a tile
    public bool PerfectTile;

    protected GridDirections _inputDirection;
    protected GridDirections _currentDirection = GridDirections.Up;
    protected GridDirections _bufferedDirection;
    protected bool _movementInterruptionBuffered = false;
    protected bool _perfectTile = false;
    protected Vector3 _inputMovement;
    protected Vector3 _prevEndPosition;
    protected Vector3 _endWorldPosition;
    protected bool _movingToNextGridUnit = false;
    protected bool _stopBuffered = false;
    protected int _lastBufferInGridUnits;
    protected bool _agentMoving;
    protected GridDirections _newDirection;
    protected bool _leftPressed = false;
    protected bool _rightPressed = false;
    protected bool _upPressed = false;
    protected bool _downPressed = false;
    protected bool _upLeftPressed = false;
    protected bool _upRightPressed = false;
    protected bool _downLeftPressed = false;
    protected bool _downRightPressed = false;
    protected float _lastPressLeft = 0;
    protected float _lastPressRight = 0;
    protected float _lastPressUp = 0;
    protected float _lastPressDown = 0;
    protected float _lastPressUpLeft = 0;
    protected float _lastPressUpRight = 0;
    protected float _lastPressDownLeft = 0;
    protected float _lastPressDownRight = 0;
    protected float _horizontalMovement;
    protected float _verticalMovement;
    protected Vector3 _lastOccupiedCell;

    protected const string _speedAnimationParameterName = "Speed";
    protected const string _walkingAnimationParameterName = "Walking";
    protected const string _idleAnimationParameterName = "Idle";
    protected int _speedAnimationParameter;
    protected int _walkingAnimationParameter;
    protected int _idleAnimationParameter;
    protected bool _firstPositionRegistered = false;
    protected AdvancedTopDownController _AdvancedController;
    /// <summary>
    /// On Initialization, we set our movement speed to WalkSpeed.
    /// </summary>
    protected override void Initialization()
    {
        base.Initialization();
        _AdvancedController = GetComponent<AdvancedTopDownController>();
        if (_controller.gameObject.MMGetComponentNoAlloc<TopDownController2D>() != null)
        {
            _controller.FreeMovement = false;
        }
        _bufferedDirection = GridDirections.None;
    }

    protected virtual void RegisterFirstPosition()
    {
        if (!_firstPositionRegistered)
        {
            _endWorldPosition = this.transform.position;
            _lastOccupiedCell = GridManager.Instance.ComputeGridPosition(_endWorldPosition);
            GridManager.Instance.OccupyCell(GridManager.Instance.ComputeGridPosition(this.transform.position));
            GridManager.Instance.SetLastPosition(this.gameObject, GridManager.Instance.ComputeGridPosition(this.transform.position));
            GridManager.Instance.SetNextPosition(this.gameObject, GridManager.Instance.ComputeGridPosition(this.transform.position));
            _firstPositionRegistered = true;
        }
    }

    /// <summary>
    /// The second of the 3 passes you can have in your ability. Think of it as Update()
    /// </summary>
    public override void ProcessAbility()
    {
        base.ProcessAbility();

        RegisterFirstPosition();

        _controller.DetectObstacles(GridManager.Instance.GridUnitSize, ObstacleDetectionOffset);
        if (DiaginalMovement)
        {
            _AdvancedController.DetectObstacles(GridManager.Instance.GridUnitSize, ObstacleDetectionOffset);
        }
        if (PushBlocks)
        {
            _AdvancedController.DetectPushables(GridManager.Instance.GridUnitSize, ObstacleDetectionOffset);
        }

        DetermineInputDirection();
        ApplyAcceleration();
        HandleMovement();
        HandleState();
    }

    /// <summary>
    /// Grabs horizontal and vertical input and stores them
    /// </summary>
    protected override void HandleInput()
    {
        _horizontalMovement = _horizontalInput;
        _verticalMovement = _verticalInput;
    }

    /// <summary>
    /// Based on press times, determines the input direction
    /// </summary>
    protected virtual void DetermineInputDirection()
    {
        if ((Mathf.Abs(_horizontalMovement) <= IdleThreshold) && (Mathf.Abs(_verticalMovement) <= IdleThreshold))
        {
            Stop(_newDirection);
            _newDirection = GridDirections.None;
            _inputMovement = Vector3.zero;
            _inputDirection = _newDirection;
            return;
        }

        //todo: This is not used in CharacterGridMovement added the new movements to it but commented it out.
        //float lastPressed = Mathf.Max(_lastPressLeft, _lastPressRight, _lastPressUp, _lastPressDown, _lastPressUpLeft, _lastPressUpRight, _lastPressDownLeft, _lastPressDownRight);

        int movementAngle = Mathf.RoundToInt((Mathf.Atan2(_horizontalMovement, _verticalMovement) * Mathf.Rad2Deg) / 45) * 45;

        if (movementAngle == 0) { _newDirection = GridDirections.Up; }
        if (movementAngle == 45) { _newDirection = GridDirections.UpRight; }
        if (movementAngle == 90) { _newDirection = GridDirections.Right; }
        if (movementAngle == 135) { _newDirection = GridDirections.DownRight; }
        if (movementAngle == 180) { _newDirection = GridDirections.Down; }
        if (movementAngle == -135) { _newDirection = GridDirections.DownLeft; }
        if (movementAngle == -90) { _newDirection = GridDirections.Left; }
        if (movementAngle == -45) { _newDirection = GridDirections.UpLeft; }

        _inputMovement = ConvertDirectionToVector3(_newDirection);
        _inputDirection = _newDirection;
    }

    /// <summary>
    /// Stops the character and has it face the specified direction
    /// </summary>
    /// <param name="direction"></param>
    public virtual void Stop(GridDirections direction)
    {
        if (direction == GridDirections.None)
        {
            return;
        }
        _bufferedDirection = direction;
        _stopBuffered = true;
    }

    /// <summary>
    /// Modifies the current speed based on the acceleration
    /// </summary>
    protected virtual void ApplyAcceleration()
    {
        if ((_currentDirection != GridDirections.None) && (CurrentSpeed < MaximumSpeed * MaximumSpeedMultiplier))
        {
            CurrentSpeed = CurrentSpeed + Acceleration * AccelerationMultiplier * Time.deltaTime;
            CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0f, MaximumSpeed * MaximumSpeedMultiplier);
        }
    }

    /// <summary>
    /// Moves the character on the grid
    /// </summary>
    protected virtual void HandleMovement()
    {
        _perfectTile = false;
        PerfectTile = false;
        ProcessBuffer();

        // if we're performing a U turn, we change direction if allowed
        if (FastDirectionChanges && _agentMoving && !_stopBuffered && _movingToNextGridUnit)
        {
            if (_bufferedDirection != GridDirections.None && _bufferedDirection == GetInverseDirection(_currentDirection))
            {
                _endWorldPosition = _endWorldPosition + ConvertDirectionToVector3(_bufferedDirection) * GridManager.Instance.GridUnitSize;
                _currentDirection = _bufferedDirection;
            }
        }

        // if we're not in between grid cells
        if (!_movingToNextGridUnit)
        {
            PerfectTile = true;

            // if we have a stop buffered
            if (_movementInterruptionBuffered)
            {
                _perfectTile = true;
                _movementInterruptionBuffered = false;
                return;
            }

            // if we don't have a direction anymore
            if (_bufferedDirection == GridDirections.None)
            {
                _currentDirection = GridDirections.None;
                _bufferedDirection = GridDirections.None;
                _agentMoving = false;
                CurrentSpeed = 0;

                GridManager.Instance.SetLastPosition(this.gameObject, GridManager.Instance.ComputeGridPosition(_endWorldPosition));
                GridManager.Instance.SetNextPosition(this.gameObject, GridManager.Instance.ComputeGridPosition(_endWorldPosition));

                return;
            }

            // we check if we can move in the selected direction
            if (((_currentDirection == GridDirections.Left) && (_controller.DetectedObstacleLeft != null))
                || ((_currentDirection == GridDirections.Right) && (_controller.DetectedObstacleRight != null))
                || ((_currentDirection == GridDirections.Up) && (_controller.DetectedObstacleUp != null))
                || ((_currentDirection == GridDirections.Down) && (_controller.DetectedObstacleDown != null))
                || ((_currentDirection == GridDirections.UpLeft) && (_controller.DetectedObstacleUp != null || _controller.DetectedObstacleLeft != null || _AdvancedController.DetectedObstacleUpLeft != null))
                || ((_currentDirection == GridDirections.UpRight) && (_controller.DetectedObstacleUp != null || _controller.DetectedObstacleRight != null || _AdvancedController.DetectedObstacleUpRight != null))
                || ((_currentDirection == GridDirections.DownLeft) && (_controller.DetectedObstacleDown != null || _controller.DetectedObstacleLeft != null || _AdvancedController.DetectedObstacleDownLeft != null))
                || ((_currentDirection == GridDirections.DownRight) && (_controller.DetectedObstacleDown != null || _controller.DetectedObstacleRight != null || _AdvancedController.DetectedObstacleDownRight != null)))
            {
                _currentDirection = _bufferedDirection;

                GridManager.Instance.SetLastPosition(this.gameObject, GridManager.Instance.ComputeGridPosition(_endWorldPosition));
                GridManager.Instance.SetNextPosition(this.gameObject, GridManager.Instance.ComputeGridPosition(_endWorldPosition));

                return;
            }

            // we check if we can move in the selected direction
            if (((_bufferedDirection == GridDirections.Left) && !(_controller.DetectedObstacleLeft != null))
                || ((_bufferedDirection == GridDirections.Right) && !(_controller.DetectedObstacleRight != null))
                || ((_bufferedDirection == GridDirections.Up) && !(_controller.DetectedObstacleUp != null))
                || ((_bufferedDirection == GridDirections.Down) && !(_controller.DetectedObstacleDown != null))
                || ((_bufferedDirection == GridDirections.UpLeft) && !(_controller.DetectedObstacleUp != null || _controller.DetectedObstacleLeft != null || _AdvancedController.DetectedObstacleUpLeft != null))
                || ((_bufferedDirection == GridDirections.UpRight) && !(_controller.DetectedObstacleUp != null || _controller.DetectedObstacleRight != null || _AdvancedController.DetectedObstacleUpRight != null))
                || ((_bufferedDirection == GridDirections.DownLeft) && !(_controller.DetectedObstacleDown != null || _controller.DetectedObstacleLeft != null || _AdvancedController.DetectedObstacleDownLeft != null))
                || ((_bufferedDirection == GridDirections.DownRight) && !(_controller.DetectedObstacleDown != null || _controller.DetectedObstacleRight != null || _AdvancedController.DetectedObstacleDownRight != null)))
            {
                _currentDirection = _bufferedDirection;
            }

            // we compute and move towards our new destination
            _movingToNextGridUnit = true;
            DetermineEndPosition();

            // we make sure the target cell is free
            TargetGridPosition = GridManager.Instance.ComputeGridPosition(_endWorldPosition);
            if (GridManager.Instance.CellIsOccupied(TargetGridPosition))
            {
                _movingToNextGridUnit = false;
                _currentDirection = GridDirections.None;
                _bufferedDirection = GridDirections.None;
                _agentMoving = false;
                CurrentSpeed = 0;
            }
            else
            {
                GridManager.Instance.FreeCell(_lastOccupiedCell);
                GridManager.Instance.SetLastPosition(this.gameObject, _lastOccupiedCell);
                GridManager.Instance.SetNextPosition(this.gameObject, TargetGridPosition);

                _lastOccupiedCell = TargetGridPosition;
            }
        }

        // computes our new grid position
        TargetGridPosition = GridManager.Instance.ComputeGridPosition(_endWorldPosition);

        // moves the controller to the next position
        Vector3 newPosition = Vector3.MoveTowards(transform.position, _endWorldPosition, Time.deltaTime * CurrentSpeed);
        _controller.CurrentDirection = _endWorldPosition - this.transform.position;
        _controller.MovePosition(newPosition);
    }

    /// <summary>
    /// Processes buffered input
    /// </summary>
    protected virtual void ProcessBuffer()
    {
        // if we have a direction in input, it becomes our new buffered direction
        if ((_inputDirection != GridDirections.None) && !_stopBuffered)
        {
            _bufferedDirection = _inputDirection;
            _lastBufferInGridUnits = BufferSize;
        }

        // if we're not moving and get an input, we start moving
        if (!_agentMoving && _inputDirection != GridDirections.None)
        {
            _currentDirection = _inputDirection;
            _agentMoving = true;
        }

        // if we've reached our next tile, we're not moving anymore
        if (_movingToNextGridUnit && (transform.position == _endWorldPosition))
        {
            _movingToNextGridUnit = false;
            CurrentGridPosition = GridManager.Instance.ComputeGridPosition(_endWorldPosition);
        }

        // we handle the buffer. If we have a buffered direction, are on a perfect tile, and don't have an input
        if ((_bufferedDirection != GridDirections.None) && !_movingToNextGridUnit && (_inputDirection == GridDirections.None) && UseInputBuffer)
        {
            // we reduce the buffer counter
            _lastBufferInGridUnits--;
            // if our buffer is expired, we revert to our current direction
            if ((_lastBufferInGridUnits < 0) && (_bufferedDirection != _currentDirection))
            {
                _bufferedDirection = _currentDirection;
            }
        }

        // if we have a stop planned and are not moving, we stop
        if ((_stopBuffered) && !_movingToNextGridUnit)
        {
            _bufferedDirection = GridDirections.None;
            _stopBuffered = false;
        }
    }

    /// <summary>
    /// Determines the end position based on the current direction
    /// </summary>
    protected virtual void DetermineEndPosition()
    {
        if (_currentDirection != GridDirections.None) { _prevEndPosition = _endWorldPosition; }
        if (_currentDirection == GridDirections.Left) { _endWorldPosition = transform.position + ConvertDirectionToVector3(GridDirections.Left) * GridManager.Instance.GridUnitSize; }
        if (_currentDirection == GridDirections.Right) { _endWorldPosition = transform.position + ConvertDirectionToVector3(GridDirections.Right) * GridManager.Instance.GridUnitSize; }
        if (_currentDirection == GridDirections.Up) { _endWorldPosition = transform.position + ConvertDirectionToVector3(GridDirections.Up) * GridManager.Instance.GridUnitSize; }
        if (_currentDirection == GridDirections.Down) { _endWorldPosition = transform.position + ConvertDirectionToVector3(GridDirections.Down) * GridManager.Instance.GridUnitSize; }
        if (_currentDirection == GridDirections.UpLeft) { _endWorldPosition = transform.position + ConvertDirectionToVector3(GridDirections.UpLeft) * GridManager.Instance.GridUnitSize; }
        if (_currentDirection == GridDirections.UpRight) { _endWorldPosition = transform.position + ConvertDirectionToVector3(GridDirections.UpRight) * GridManager.Instance.GridUnitSize; }
        if (_currentDirection == GridDirections.DownLeft) { _endWorldPosition = transform.position + ConvertDirectionToVector3(GridDirections.DownLeft) * GridManager.Instance.GridUnitSize; }
        if (_currentDirection == GridDirections.DownRight) { _endWorldPosition = transform.position + ConvertDirectionToVector3(GridDirections.DownRight) * GridManager.Instance.GridUnitSize; }
    }

    /// <summary>
    /// Converts a GridDirection to a Vector3 based on the current D
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    protected virtual Vector3 ConvertDirectionToVector3(GridDirections direction)
    {
        if (direction != GridDirections.None)
        {
            if (direction == GridDirections.Left) return Vector3.left;
            if (direction == GridDirections.Right) return Vector3.right;
            if (_character.CharacterDimension == Character.CharacterDimensions.Type2D)
            {
                if (direction == GridDirections.Up) return Vector3.up;
                if (direction == GridDirections.Down) return Vector3.down;
                if (direction == GridDirections.UpLeft) return Vector3Directions.UpLeft;
                if (direction == GridDirections.UpRight) return Vector3Directions.UpRight;
                if (direction == GridDirections.DownLeft) return Vector3Directions.DownLeft;
                if (direction == GridDirections.DownRight) return Vector3Directions.DownRight;
            }
            else
            {
                if (direction == GridDirections.Up) return Vector3.forward;
                if (direction == GridDirections.Down) return Vector3.back;
                if (direction == GridDirections.UpLeft) return Vector3Directions.ForwardLeft;
                if (direction == GridDirections.UpRight) return Vector3Directions.ForwardRight;
                if (direction == GridDirections.DownLeft) return Vector3Directions.BackLeft;
                if (direction == GridDirections.DownRight) return Vector3Directions.BackRight;
            }
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Returns the opposite direction of a GridDirection
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    protected virtual GridDirections GetInverseDirection(GridDirections direction)
    {
        if (direction != GridDirections.None)
        {
            if (direction == GridDirections.Left) return GridDirections.Right;
            if (direction == GridDirections.Right) return GridDirections.Left;
            if (direction == GridDirections.Up) return GridDirections.Down;
            if (direction == GridDirections.Down) return GridDirections.Up;
            if (direction == GridDirections.UpLeft) return GridDirections.DownRight;
            if (direction == GridDirections.UpRight) return GridDirections.DownLeft;
            if (direction == GridDirections.DownLeft) return GridDirections.UpRight;
            if (direction == GridDirections.DownRight) return GridDirections.UpLeft;

        }
        return GridDirections.None;
    }

    protected virtual void HandleState()
    {
        if (_movingToNextGridUnit)
        {
            if (_movement.CurrentState != CharacterStates.MovementStates.Walking)
            {
                _movement.ChangeState(CharacterStates.MovementStates.Walking);
                PlayAbilityStartFeedbacks();
            }
        }
        else
        {
            if (_movement.CurrentState != CharacterStates.MovementStates.Idle)
            {
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
                if (_startFeedbackIsPlaying)
                {
                    StopStartFeedbacks();
                    PlayAbilityStopFeedbacks();
                }
            }
        }
    }

    /// <summary>
    /// On death we free our last occupied cell
    /// </summary>
    protected override void OnDeath()
    {
        base.OnDeath();
        GridManager.Instance.FreeCell(_lastOccupiedCell);

    }

    /// <summary>
    /// Adds required animator parameters to the animator parameters list if they exist
    /// </summary>
    protected override void InitializeAnimatorParameters()
    {
        RegisterAnimatorParameter(_speedAnimationParameterName, AnimatorControllerParameterType.Float, out _speedAnimationParameter);
        RegisterAnimatorParameter(_walkingAnimationParameterName, AnimatorControllerParameterType.Bool, out _walkingAnimationParameter);
        RegisterAnimatorParameter(_idleAnimationParameterName, AnimatorControllerParameterType.Bool, out _idleAnimationParameter);
    }

    /// <summary>
    /// Sends the current speed and the current value of the Walking state to the animator
    /// </summary>
    public override void UpdateAnimator()
    {
        MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _speedAnimationParameter, CurrentSpeed, _character._animatorParameters);
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _walkingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Walking), _character._animatorParameters);
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _idleAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Idle), _character._animatorParameters);
    }
}


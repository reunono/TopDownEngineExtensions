using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Collections.Generic;
using UnityEngine;

namespace PhluxApps.TopDownEngine
{
    /// <summary>
    /// Add this ability to a Character to have it move on a grid. This will require a GridManager
    /// be present in your scene This adds the ability to move diagonally to the
    /// CharacterGridMovement ability. DO NOT use this component and a CharacterMovement component
    /// on the same character. DO NOT use this component and a CharacterGridMovement component on
    /// the same character.
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/Abilities/Advanced Character Grid Movement")]
    public class AdvancedCharacterGridMovement : CharacterGridMovement
    {
        public enum GridDiaginalDirections
        { None, Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight }

        [Header("Advanced Movement")]

        /// Allow diaginal movement
        [Tooltip("allow diaginal movement")]
        public bool DiaginalMovement = false;

        /// Ignore collisions with cardinal obstacles when moving diaginally
        [Tooltip("Ignore collisions with cardinal obstacles when moving diaginally")]
        public bool IgnoreCardinalObstacles = false;

        /// the movement state when ignoring cardinal obstacles
        [Tooltip("the movement state when ignoring cardinal obstacles")]
        public CharacterStates.MovementStates IgnoreCardinalObstacleMovementState = CharacterStates.MovementStates.Jumping;

        [Header("Test")]
        [MMInspectorButton("UpLeft")]
        public bool UpLeftButton;

        [MMInspectorButton("UpRight")]
        public bool UpRightButton;

        [MMInspectorButton("DownLeft")]
        public bool DownLeftButton;

        [MMInspectorButton("DownRight")]
        public bool DownRightButton;

        [MMInspectorButton("UpLeftOneCell")]
        public bool UpLeftOneCellButton;

        [MMInspectorButton("UpRightOneCell")]
        public bool UpRightOneCellButton;

        [MMInspectorButton("DownLeftOneCell")]
        public bool DownLeftOneCellButton;

        [MMInspectorButton("DownRightOneCell")]
        public bool DownRightOneCellButton;

        // the obstacle up and left to this controller (only updated if DetectObstacles is called)
        public GameObject DetectedObstacleUpLeft { get; set; }

        // the obstacle up and right to this controller (only updated if DetectObstacles is called)
        public GameObject DetectedObstacleUpRight { get; set; }

        // the obstacle down and left to this controller (only updated if DetectObstacles is called)
        public GameObject DetectedObstacleDownLeft { get; set; }

        // the obstacle down and right to this controller (only updated if DetectObstacles is called)
        public GameObject DetectedObstacleDownRight { get; set; }

        // collision detection
        protected Collider2D _collider2D;

        protected List<Collider2D> _ignoreed2DCardinalObstacles = new();
        protected Collider _collider;
        protected List<Collider> _ignoreed3DCardinalObstacles = new();

        protected RaycastHit2D _raycastUpLeft;
        protected RaycastHit2D _raycastUpRight;
        protected RaycastHit2D _raycastDownLeft;
        protected RaycastHit2D _raycastDownRight;

        protected RaycastHit _cardinalRaycast;

        protected GridDiaginalDirections _inputDiaginalDirection;
        protected GridDiaginalDirections _currentDiaginalDirection = GridDiaginalDirections.Up;
        protected GridDiaginalDirections _bufferedDiaginalDirection;
        protected GridDiaginalDirections _newDiaginalDirection;
        protected GridDiaginalDirections _fallbackDiaginalDirection;

        protected bool _hasDiagonalMovement = false;
        protected bool _isIgnoringCardinalObstacles = false;

        protected bool _upLeftPressedLastFrame = false;
        protected bool _upRightPressedLastFrame = false;
        protected bool _downLeftPressedLastFrame = false;
        protected bool _downRightPressedLastFrame = false;

        /// <summary>
        /// Moves the character one cell up and left
        /// </summary>
        public virtual void UpLeftOneCell()
        {
            StartCoroutine(OneCell(Vector2.up + Vector2.left));
        }

        /// <summary>
        /// Moves the character one cell up and right
        /// </summary>
        public virtual void UpRightOneCell()
        {
            StartCoroutine(OneCell(Vector2.up + Vector2.right));
        }

        /// <summary>
        /// Moves the character one cell down and left
        /// </summary>
        public virtual void DownLeftOneCell()
        {
            StartCoroutine(OneCell(Vector2.down + Vector2.left));
        }

        /// <summary>
        /// Moves the character one cell down and right
        /// </summary>
        public virtual void DownRightOneCell()
        {
            StartCoroutine(OneCell(Vector2.down + Vector2.left));
        }

        /// <summary>
        /// Sets script movement to left
        /// </summary>
        public virtual void UpLeft()
        { SetMovement(Vector2.up + Vector2.left); }

        /// <summary>
        /// Sets script movement to right
        /// </summary>
        public virtual void UpRight()
        { SetMovement(Vector2.up + Vector2.right); }

        /// <summary>
        /// Sets script movement to left
        /// </summary>
        public virtual void DownLeft()
        { SetMovement(Vector2.down + Vector2.left); }

        /// <summary>
        /// Sets script movement to right
        /// </summary>
        public virtual void DownRight()
        { SetMovement(Vector2.down + Vector2.left); }

        /// <summary>
        /// On Initialization, we set our movement speed to WalkSpeed.
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

            _newDiaginalDirection = GridDiaginalDirections.None;
            _currentDirection = GridDirections.None;
            if (DimensionMode == DimensionModes.TwoD)
            {
                _collider2D = _controller.gameObject.MMGetComponentNoAlloc<Collider2D>();
            }
            else
            {
                _collider = _controller.gameObject.MMGetComponentNoAlloc<Collider>();
            }
        }

        /// <summary>
        /// The second of the 3 passes you can have in your ability. Think of it as Update()
        /// </summary>
        public override void ProcessAbility()
        {
            if ((_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen)
                || (_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Stunned))
            {
                return;
            }

            RegisterFirstPosition();

            DetectObstacles(GridManager.Instance.GridUnitSize, ObstacleDetectionOffset);
            DetermineInputDirection();
            ApplyAcceleration();
            HandleMovement();
            HandleState();
        }

        /// <summary>
        /// Based on press times, determines the input direction
        /// </summary>
        protected override void DetermineInputDirection()
        {
            // if we're not pressing any direction, we stop
            if (Mathf.Abs(_horizontalMovement) <= IdleThreshold && Mathf.Abs(_verticalMovement) <= IdleThreshold)
            {
                Stop(_newDiaginalDirection);
                _newDiaginalDirection = GridDiaginalDirections.None;
                _fallbackDiaginalDirection = GridDiaginalDirections.None;
                _inputMovement = Vector3.zero;
            }

            _hasDiagonalMovement = DiaginalMovement && _verticalMovement != 0f && _horizontalMovement != 0f;

            // if we're pressing a direction for the first time, it becomes our new direction
            if (!_hasDiagonalMovement)
            {
                if ((_horizontalMovement < 0f) && !_leftPressedLastFrame) { _newDiaginalDirection = GridDiaginalDirections.Left; _inputMovement = Vector3.left; }
                if ((_horizontalMovement > 0f) && !_rightPressedLastFrame) { _newDiaginalDirection = GridDiaginalDirections.Right; _inputMovement = Vector3.right; }
                if ((_verticalMovement < 0f) && !_downPressedLastFrame) { _newDiaginalDirection = GridDiaginalDirections.Down; _inputMovement = Vector3.down; }
                if ((_verticalMovement > 0f) && !_upPressedLastFrame) { _newDiaginalDirection = GridDiaginalDirections.Up; _inputMovement = Vector3.up; }
            }
            else
            {
                if ((_verticalMovement < 0f) && (_horizontalMovement < 0f) && !_downLeftPressedLastFrame) { _newDiaginalDirection = GridDiaginalDirections.DownLeft; _inputMovement = Vector3.down + Vector3.left; }
                if ((_verticalMovement < 0f) && (_horizontalMovement > 0f) && !_downRightPressedLastFrame) { _newDiaginalDirection = GridDiaginalDirections.DownRight; _inputMovement = Vector3.down + Vector3.right; }
                if ((_verticalMovement > 0f) && (_horizontalMovement < 0f) && !_upLeftPressedLastFrame) { _newDiaginalDirection = GridDiaginalDirections.UpLeft; _inputMovement = Vector3.up + Vector3.left; }
                if ((_verticalMovement > 0f) && (_horizontalMovement > 0f) && !_upRightPressedLastFrame) { _newDiaginalDirection = GridDiaginalDirections.UpRight; _inputMovement = Vector3.up + Vector3.right; }
            }

            // if we were pressing a direction, and have just released it, we'll look for an other direction
            if (((_horizontalMovement == 0f) && (_leftPressedLastFrame || _rightPressedLastFrame)) ||
               ((_verticalMovement == 0f) && (_downPressedLastFrame || _upPressedLastFrame)) ||
               (_verticalMovement == 0f && _horizontalMovement == 0f && (_downLeftPressedLastFrame || _downRightPressedLastFrame || _upLeftPressedLastFrame || _upRightPressedLastFrame)))
            { _newDiaginalDirection = GridDiaginalDirections.None; }

            // if at this point we have no direction, we take any pressed one
            if (_newDiaginalDirection == GridDiaginalDirections.None)
            {
                if (!_hasDiagonalMovement)
                {
                    if (_horizontalMovement < 0f) { _newDiaginalDirection = GridDiaginalDirections.Left; _inputMovement = Vector3.left; }
                    if (_horizontalMovement > 0f) { _newDiaginalDirection = GridDiaginalDirections.Right; _inputMovement = Vector3.right; }
                    if (_verticalMovement < 0f) { _newDiaginalDirection = GridDiaginalDirections.Down; _inputMovement = Vector3.down; }
                    if (_verticalMovement > 0f) { _newDiaginalDirection = GridDiaginalDirections.Up; _inputMovement = Vector3.up; }
                }
                else
                {
                    if ((_verticalMovement < 0f) && (_horizontalMovement < 0f)) { _newDiaginalDirection = GridDiaginalDirections.DownLeft; _inputMovement = Vector3.down + Vector3.left; }
                    if ((_verticalMovement < 0f) && (_horizontalMovement > 0f)) { _newDiaginalDirection = GridDiaginalDirections.DownRight; _inputMovement = Vector3.down + Vector3.right; }
                    if ((_verticalMovement > 0f) && (_horizontalMovement < 0f)) { _newDiaginalDirection = GridDiaginalDirections.UpLeft; _inputMovement = Vector3.up + Vector3.left; }
                    if ((_verticalMovement > 0f) && (_horizontalMovement > 0f)) { _newDiaginalDirection = GridDiaginalDirections.UpRight; _inputMovement = Vector3.up + Vector3.right; }
                }
            }

            if (_hasDiagonalMovement && (_leftPressedLastFrame || _rightPressedLastFrame || _downPressedLastFrame || _upPressedLastFrame))
            {
                if (_leftPressedLastFrame) { _fallbackDiaginalDirection = (_newDiaginalDirection == GridDiaginalDirections.DownLeft) ? GridDiaginalDirections.Down : GridDiaginalDirections.Up; }
                if (_rightPressedLastFrame) { _fallbackDiaginalDirection = (_newDiaginalDirection == GridDiaginalDirections.DownRight) ? GridDiaginalDirections.Down : GridDiaginalDirections.Up; }
                if (_downPressedLastFrame) { _fallbackDiaginalDirection = (_newDiaginalDirection == GridDiaginalDirections.DownLeft) ? GridDiaginalDirections.Left : GridDiaginalDirections.Right; }
                if (_upPressedLastFrame) { _fallbackDiaginalDirection = (_newDiaginalDirection == GridDiaginalDirections.UpLeft) ? GridDiaginalDirections.Left : GridDiaginalDirections.Right; }
            }

            _inputDiaginalDirection = _newDiaginalDirection;

            // we store our presses for next frame
            _leftPressedLastFrame = !_hasDiagonalMovement && (_horizontalMovement < 0f);
            _rightPressedLastFrame = !_hasDiagonalMovement && (_horizontalMovement > 0f);
            _downPressedLastFrame = !_hasDiagonalMovement && (_verticalMovement < 0f);
            _upPressedLastFrame = !_hasDiagonalMovement && (_verticalMovement > 0f);
            if (DiaginalMovement)
            {
                _downLeftPressedLastFrame = (_verticalMovement < 0f) && (_horizontalMovement < 0f);
                _downRightPressedLastFrame = (_verticalMovement < 0f) && (_horizontalMovement > 0f);
                _upLeftPressedLastFrame = (_verticalMovement > 0f) && (_horizontalMovement < 0f);
                _upRightPressedLastFrame = (_verticalMovement > 0f) && (_horizontalMovement > 0f);
            }
        }

        /// <summary>
        /// Stops the character and has it face the specified direction
        /// </summary>
        /// <param name="direction"></param>
        public virtual void Stop(GridDiaginalDirections direction)
        {
            if (direction == GridDiaginalDirections.None)
            {
                return;
            }
            _bufferedDiaginalDirection = direction;
            _stopBuffered = true;
        }

        /// <summary>
        /// Modifies the current speed based on the acceleration
        /// </summary>
		protected override void ApplyAcceleration()
        {
            if ((_currentDiaginalDirection != GridDiaginalDirections.None) && (CurrentSpeed < MaximumSpeed * MaximumSpeedMultiplier))
            {
                CurrentSpeed = CurrentSpeed + Acceleration * AccelerationMultiplier * Time.deltaTime;
                CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0f, MaximumSpeed * MaximumSpeedMultiplier);
            }
        }

        /// <summary>
        /// Moves the character on the grid
        /// </summary>
        protected override void HandleMovement()
        {
            _perfectTile = false;
            PerfectTile = false;
            ProcessBuffer();

            // if we're not in between grid cells
            if (!_movingToNextGridUnit)
            {
                PerfectTile = true;

                if (_isIgnoringCardinalObstacles)
                {
                    if (DimensionMode == DimensionModes.TwoD)
                    {
                        foreach (Collider2D collider in _ignoreed2DCardinalObstacles)
                        {
                            Physics2D.IgnoreCollision(collider, _collider2D, false);
                        }
                        _ignoreed2DCardinalObstacles.Clear();
                    }
                    else
                    {
                        foreach (Collider collider in _ignoreed3DCardinalObstacles)
                        {
                            Physics.IgnoreCollision(collider, _collider, false);
                        }
                        _ignoreed3DCardinalObstacles.Clear();
                    }
                    _isIgnoringCardinalObstacles = false;
                }

                // if we have a stop buffered
                if (_movementInterruptionBuffered)
                {
                    _perfectTile = true;
                    _movementInterruptionBuffered = false;
                    return;
                }

                // if we don't have a direction anymore
                if (_bufferedDiaginalDirection == GridDiaginalDirections.None)
                {
                    _currentDiaginalDirection = GridDiaginalDirections.None;
                    _bufferedDiaginalDirection = GridDiaginalDirections.None;
                    _agentMoving = false;
                    CurrentSpeed = 0;

                    GridManager.Instance.SetLastPosition(this.gameObject, GridManager.Instance.WorldToCellCoordinates(_endWorldPosition));
                    GridManager.Instance.SetNextPosition(this.gameObject, GridManager.Instance.WorldToCellCoordinates(_endWorldPosition));

                    return;
                }

                // if there is a diagonal movement but have diagonal obstacles we switch to a
                // cardinal direction that doesn't have an obstacle
                if (_hasDiagonalMovement)
                {
                    if ((_currentDiaginalDirection == GridDiaginalDirections.UpLeft) && ((DetectedObstacleUpLeft != null && (_controller.DetectedObstacleUp == null || _controller.DetectedObstacleLeft == null)) || (!IgnoreCardinalObstacles && (_controller.DetectedObstacleUp != null || _controller.DetectedObstacleLeft != null))))
                    {
                        _currentDiaginalDirection = (_controller.DetectedObstacleUp == null && _controller.DetectedObstacleLeft == null)
                            ? (_fallbackDiaginalDirection != GridDiaginalDirections.None) ? _fallbackDiaginalDirection : (MMMaths.RollADice(2) == 1) ? GridDiaginalDirections.Left : GridDiaginalDirections.Up
                            : (_controller.DetectedObstacleUp == null) ? GridDiaginalDirections.Up : GridDiaginalDirections.Left;
                    }
                    else if ((_currentDiaginalDirection == GridDiaginalDirections.UpRight) && ((DetectedObstacleUpRight != null && (_controller.DetectedObstacleUp == null || _controller.DetectedObstacleRight == null)) || (!IgnoreCardinalObstacles && (_controller.DetectedObstacleUp != null || _controller.DetectedObstacleRight != null))))
                    {
                        _currentDiaginalDirection = _controller.DetectedObstacleUp == null && _controller.DetectedObstacleRight == null
                            ? (_fallbackDiaginalDirection != GridDiaginalDirections.None) ? _fallbackDiaginalDirection : (MMMaths.RollADice(2) == 1) ? GridDiaginalDirections.Right : GridDiaginalDirections.Up
                            : (_controller.DetectedObstacleUp == null) ? GridDiaginalDirections.Up : GridDiaginalDirections.Right;
                    }
                    else if ((_currentDiaginalDirection == GridDiaginalDirections.DownLeft) && ((DetectedObstacleDownLeft != null && (_controller.DetectedObstacleDown == null || _controller.DetectedObstacleLeft == null)) || (!IgnoreCardinalObstacles && (_controller.DetectedObstacleDown != null || _controller.DetectedObstacleLeft != null))))
                    {
                        _currentDiaginalDirection = (_controller.DetectedObstacleUp == null && _controller.DetectedObstacleRight == null)
                            ? (_fallbackDiaginalDirection != GridDiaginalDirections.None) ? _fallbackDiaginalDirection : (MMMaths.RollADice(2) == 1) ? GridDiaginalDirections.Right : GridDiaginalDirections.Up
                            : (_controller.DetectedObstacleUp == null) ? GridDiaginalDirections.Up : GridDiaginalDirections.Right;
                    }
                    else if ((_currentDiaginalDirection == GridDiaginalDirections.DownLeft) && ((DetectedObstacleDownLeft != null && (_controller.DetectedObstacleDown == null || _controller.DetectedObstacleLeft == null)) || (!IgnoreCardinalObstacles && (_controller.DetectedObstacleDown != null || _controller.DetectedObstacleLeft != null))))
                    {
                        _currentDiaginalDirection = (_controller.DetectedObstacleDown == null && _controller.DetectedObstacleLeft == null)
                            ? (_fallbackDiaginalDirection != GridDiaginalDirections.None) ? _fallbackDiaginalDirection : (MMMaths.RollADice(2) == 1) ? GridDiaginalDirections.Left : GridDiaginalDirections.Down
                            : (_controller.DetectedObstacleDown == null) ? GridDiaginalDirections.Down : GridDiaginalDirections.Left;
                    }
                    else if ((_currentDiaginalDirection == GridDiaginalDirections.DownRight) && ((DetectedObstacleDownRight != null && (_controller.DetectedObstacleDown == null || _controller.DetectedObstacleRight == null)) || (!IgnoreCardinalObstacles && (_controller.DetectedObstacleDown != null || _controller.DetectedObstacleRight != null))))
                    {
                        _currentDiaginalDirection = _controller.DetectedObstacleDown == null && _controller.DetectedObstacleRight == null
                            ? (_fallbackDiaginalDirection != GridDiaginalDirections.None) ? _fallbackDiaginalDirection : (MMMaths.RollADice(2) == 1) ? GridDiaginalDirections.Right : GridDiaginalDirections.Down
                            : (_controller.DetectedObstacleDown == null) ? GridDiaginalDirections.Down : GridDiaginalDirections.Right;
                    }
                }

                // we check if we can move in the selected direction
                if (((_currentDiaginalDirection == GridDiaginalDirections.Left) && (_controller.DetectedObstacleLeft != null))
                    || ((_currentDiaginalDirection == GridDiaginalDirections.Right) && (_controller.DetectedObstacleRight != null))
                    || ((_currentDiaginalDirection == GridDiaginalDirections.Up) && (_controller.DetectedObstacleUp != null))
                    || ((_currentDiaginalDirection == GridDiaginalDirections.Down) && (_controller.DetectedObstacleDown != null))
                    || ((_currentDiaginalDirection == GridDiaginalDirections.UpLeft) && ((DetectedObstacleUpLeft != null) || ((DetectedObstacleUpLeft == null) && !IgnoreCardinalObstacles && ((_controller.DetectedObstacleUp != null) || (_controller.DetectedObstacleLeft != null)))))
                    || ((_currentDiaginalDirection == GridDiaginalDirections.UpRight) && ((DetectedObstacleUpRight != null) || ((DetectedObstacleUpRight == null) && !IgnoreCardinalObstacles && ((_controller.DetectedObstacleUp != null) || (_controller.DetectedObstacleRight != null)))))
                    || ((_currentDiaginalDirection == GridDiaginalDirections.DownLeft) && ((DetectedObstacleDownLeft != null) || ((DetectedObstacleDownLeft == null) && !IgnoreCardinalObstacles && ((_controller.DetectedObstacleDown != null) || (_controller.DetectedObstacleLeft != null)))))
                    || ((_currentDiaginalDirection == GridDiaginalDirections.DownRight) && ((DetectedObstacleDownRight != null) || ((DetectedObstacleDownRight == null) && !IgnoreCardinalObstacles && ((_controller.DetectedObstacleDown != null) || (_controller.DetectedObstacleRight != null))))))
                {
                    _currentDiaginalDirection = _bufferedDiaginalDirection;

                    GridManager.Instance.SetLastPosition(this.gameObject, GridManager.Instance.WorldToCellCoordinates(_endWorldPosition));
                    GridManager.Instance.SetNextPosition(this.gameObject, GridManager.Instance.WorldToCellCoordinates(_endWorldPosition));

                    return;
                }

                // we check if we can move in the selected direction
                if (((_bufferedDiaginalDirection == GridDiaginalDirections.Left) && (_controller.DetectedObstacleLeft == null))
                    || ((_bufferedDiaginalDirection == GridDiaginalDirections.Right) && (_controller.DetectedObstacleRight == null))
                    || ((_bufferedDiaginalDirection == GridDiaginalDirections.Up) && (_controller.DetectedObstacleUp == null))
                    || ((_bufferedDiaginalDirection == GridDiaginalDirections.Down) && (_controller.DetectedObstacleDown == null))
                    || ((_bufferedDiaginalDirection == GridDiaginalDirections.UpLeft) && (DetectedObstacleUpLeft == null) && (IgnoreCardinalObstacles || ((_controller.DetectedObstacleUp == null) && (_controller.DetectedObstacleLeft == null))))
                    || ((_bufferedDiaginalDirection == GridDiaginalDirections.UpRight) && (DetectedObstacleUpRight == null) && (IgnoreCardinalObstacles || ((_controller.DetectedObstacleUp == null) && (_controller.DetectedObstacleRight == null))))
                    || ((_bufferedDiaginalDirection == GridDiaginalDirections.DownLeft) && (DetectedObstacleDownLeft == null) && (IgnoreCardinalObstacles || ((_controller.DetectedObstacleDown == null) && (_controller.DetectedObstacleLeft == null))))
                    || ((_bufferedDiaginalDirection == GridDiaginalDirections.DownRight) && (DetectedObstacleDownRight == null) && (IgnoreCardinalObstacles || ((_controller.DetectedObstacleDown == null) && (_controller.DetectedObstacleRight == null)))))
                {
                    _currentDiaginalDirection = _bufferedDiaginalDirection;
                }

                // we compute and move towards our new destination
                _movingToNextGridUnit = true;
                DetermineEndPosition();

                // we make sure the target cell is free
                if (GridManager.Instance.CellIsOccupied(TargetGridPosition))
                {
                    _movingToNextGridUnit = false;
                    _currentDiaginalDirection = GridDiaginalDirections.None;
                    _bufferedDiaginalDirection = GridDiaginalDirections.None;
                    _agentMoving = false;
                    CurrentSpeed = 0;
                }
                else
                {
                    GridManager.Instance.FreeCell(_lastOccupiedCellCoordinates);
                    GridManager.Instance.SetLastPosition(this.gameObject, _lastOccupiedCellCoordinates);
                    GridManager.Instance.SetNextPosition(this.gameObject, TargetGridPosition);
                    GridManager.Instance.OccupyCell(TargetGridPosition);
                    CurrentCellCoordinates = TargetGridPosition;
                    _lastOccupiedCellCoordinates = TargetGridPosition;

                    // we ignore collisions with obstacles if needed
                    if (IgnoreCardinalObstacles && _hasDiagonalMovement)
                    {
                        if (DimensionMode == DimensionModes.TwoD)
                        {
                            if (((_currentDiaginalDirection == GridDiaginalDirections.UpLeft) || (_currentDiaginalDirection == GridDiaginalDirections.UpRight)) && (_controller.DetectedObstacleUp != null))
                            {
                                Collider2D obstacleCollider = _controller.DetectedObstacleUp.GetComponent<Collider2D>();
                                Physics2D.IgnoreCollision(obstacleCollider, _collider2D, true);
                                _ignoreed2DCardinalObstacles.Add(obstacleCollider);
                                _isIgnoringCardinalObstacles = true;
                            }
                            if (((_currentDiaginalDirection == GridDiaginalDirections.DownLeft) || (_currentDiaginalDirection == GridDiaginalDirections.DownRight)) && (_controller.DetectedObstacleDown != null))
                            {
                                Collider2D obstacleCollider = _controller.DetectedObstacleDown.GetComponent<Collider2D>();
                                Physics2D.IgnoreCollision(obstacleCollider, _collider2D, true);
                                _ignoreed2DCardinalObstacles.Add(obstacleCollider);
                                _isIgnoringCardinalObstacles = true;
                            }
                            if (((_currentDiaginalDirection == GridDiaginalDirections.UpLeft) || (_currentDiaginalDirection == GridDiaginalDirections.DownLeft)) && (_controller.DetectedObstacleLeft != null))
                            {
                                Collider2D obstacleCollider = _controller.DetectedObstacleLeft.GetComponent<Collider2D>();
                                Physics2D.IgnoreCollision(obstacleCollider, _collider2D, true);
                                _ignoreed2DCardinalObstacles.Add(obstacleCollider);
                                _isIgnoringCardinalObstacles = true;
                            }
                            if (((_currentDiaginalDirection == GridDiaginalDirections.DownRight) || (_currentDiaginalDirection == GridDiaginalDirections.UpRight)) && (_controller.DetectedObstacleRight != null))
                            {
                                Collider2D obstacleCollider = _controller.DetectedObstacleRight.GetComponent<Collider2D>();
                                Physics2D.IgnoreCollision(obstacleCollider, _collider2D, true);
                                _ignoreed2DCardinalObstacles.Add(obstacleCollider);
                                _isIgnoringCardinalObstacles = true;
                            }
                        }
                        else
                        {
                            if (((_currentDiaginalDirection == GridDiaginalDirections.UpLeft) || (_currentDiaginalDirection == GridDiaginalDirections.UpRight)) && (_controller.DetectedObstacleUp != null))
                            {
                                Collider obstacleCollider = _controller.DetectedObstacleUp.GetComponent<Collider>();
                                Physics.IgnoreCollision(obstacleCollider, _collider, true);
                                _ignoreed3DCardinalObstacles.Add(obstacleCollider);
                                _isIgnoringCardinalObstacles = true;
                            }
                            if (((_currentDiaginalDirection == GridDiaginalDirections.DownLeft) || (_currentDiaginalDirection == GridDiaginalDirections.DownRight)) && (_controller.DetectedObstacleDown != null))
                            {
                                Collider obstacleCollider = _controller.DetectedObstacleDown.GetComponent<Collider>();
                                Physics.IgnoreCollision(obstacleCollider, _collider, true);
                                _ignoreed3DCardinalObstacles.Add(obstacleCollider);
                                _isIgnoringCardinalObstacles = true;
                            }
                            if (((_currentDiaginalDirection == GridDiaginalDirections.UpLeft) || (_currentDiaginalDirection == GridDiaginalDirections.DownRight)) && (_controller.DetectedObstacleLeft != null))
                            {
                                Collider obstacleCollider = _controller.DetectedObstacleLeft.GetComponent<Collider>();
                                Physics.IgnoreCollision(obstacleCollider, _collider, true);
                                _ignoreed3DCardinalObstacles.Add(obstacleCollider);
                                _isIgnoringCardinalObstacles = true;
                            }
                            if (((_currentDiaginalDirection == GridDiaginalDirections.DownLeft) || (_currentDiaginalDirection == GridDiaginalDirections.UpRight)) && (_controller.DetectedObstacleRight != null))
                            {
                                Collider obstacleCollider = _controller.DetectedObstacleRight.GetComponent<Collider>();
                                Physics.IgnoreCollision(obstacleCollider, _collider, true);
                                _ignoreed3DCardinalObstacles.Add(obstacleCollider);
                                _isIgnoringCardinalObstacles = true;
                            }
                        }
                    }
                }
            }

            // computes our new grid position
            TargetGridPosition = GridManager.Instance.WorldToCellCoordinates(_endWorldPosition);

            // moves the controller to the next position
            Vector3 newPosition = Vector3.MoveTowards(transform.position, _endWorldPosition, Time.deltaTime * CurrentSpeed);

            _lastCurrentDirection = _endWorldPosition - this.transform.position;
            _lastCurrentDirection = _lastCurrentDirection.MMRound();
            if (_lastCurrentDirection != Vector3.zero)
            {
                _controller.CurrentDirection = _lastCurrentDirection;
            }
            _controller.MovePosition(newPosition);
        }

        /// <summary>
        /// Processes buffered input
        /// </summary>
		protected override void ProcessBuffer()
        {
            // if we have a direction in input, it becomes our new buffered direction
            if ((_inputDiaginalDirection != GridDiaginalDirections.None) && !_stopBuffered)
            {
                _bufferedDiaginalDirection = _inputDiaginalDirection;
                _lastBufferInGridUnits = BufferSize;
            }

            // if we're not moving and get an input, we start moving
            if (!_agentMoving && _inputDiaginalDirection != GridDiaginalDirections.None)
            {
                _currentDiaginalDirection = _inputDiaginalDirection;
                _agentMoving = true;
            }

            // if we've reached our next tile, we're not moving anymore
            if (_movingToNextGridUnit && (transform.position == _endWorldPosition))
            {
                _movingToNextGridUnit = false;
                CurrentGridPosition = GridManager.Instance.WorldToCellCoordinates(_endWorldPosition);
            }

            // we handle the buffer. If we have a buffered direction, are on a perfect tile, and
            // don't have an input
            if ((_bufferedDiaginalDirection != GridDiaginalDirections.None) && !_movingToNextGridUnit && (_inputDiaginalDirection == GridDiaginalDirections.None) && UseInputBuffer)
            {
                // we reduce the buffer counter
                _lastBufferInGridUnits--;
                // if our buffer is expired, we revert to our current direction
                if ((_lastBufferInGridUnits < 0) && (_bufferedDiaginalDirection != _currentDiaginalDirection))
                {
                    _bufferedDiaginalDirection = _currentDiaginalDirection;
                }
            }

            // if we have a stop planned and are not moving, we stop
            if ((_stopBuffered) && !_movingToNextGridUnit)
            {
                _bufferedDiaginalDirection = GridDiaginalDirections.None;
                _stopBuffered = false;
            }
        }

        /// <summary>
        /// Determines the end position based on the current direction
        /// </summary>
		protected override void DetermineEndPosition()
        {
            TargetGridPosition = CurrentCellCoordinates + ConvertDirectionToVector3Int(_currentDiaginalDirection);
            _endWorldPosition = GridManager.Instance.CellToWorldCoordinates(TargetGridPosition);
            // we maintain our z(2D) or y (3D)
            _endWorldPosition = DimensionClamp(_endWorldPosition);
        }

        protected virtual Vector3Int ConvertDirectionToVector3Int(GridDiaginalDirections direction)
        {
            if (direction != GridDiaginalDirections.None)
            {
                if (direction == GridDiaginalDirections.Left) return Vector3Int.left;
                if (direction == GridDiaginalDirections.Right) return Vector3Int.right;

                if (DimensionMode == DimensionModes.TwoD)
                {
                    if (direction == GridDiaginalDirections.Up) return Vector3Int.up;
                    if (direction == GridDiaginalDirections.Down) return Vector3Int.down;
                    if (direction == GridDiaginalDirections.UpLeft) return Vector3Int.up + Vector3Int.left;
                    if (direction == GridDiaginalDirections.UpRight) return Vector3Int.up + Vector3Int.right;
                    if (direction == GridDiaginalDirections.DownLeft) return Vector3Int.down + Vector3Int.left;
                    if (direction == GridDiaginalDirections.DownRight) return Vector3Int.down + Vector3Int.right;
                }
                else
                {
                    if (direction == GridDiaginalDirections.Up) return Vector3Int.RoundToInt(Vector3.forward);
                    if (direction == GridDiaginalDirections.Down) return Vector3Int.RoundToInt(Vector3.back);
                    if (direction == GridDiaginalDirections.UpLeft) return Vector3Int.RoundToInt(Vector3.forward + Vector3.left);
                    if (direction == GridDiaginalDirections.UpRight) return Vector3Int.RoundToInt(Vector3.forward + Vector3.right);
                    if (direction == GridDiaginalDirections.DownLeft) return Vector3Int.RoundToInt(Vector3.back + Vector3.left);
                    if (direction == GridDiaginalDirections.DownRight) return Vector3Int.RoundToInt(Vector3.back + Vector3.right);
                }
            }
            return Vector3Int.zero;
        }

        /// <summary>
        /// Returns the opposite direction of a GridDirection
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected virtual GridDiaginalDirections GetInverseDirection(GridDiaginalDirections direction)
        {
            if (direction != GridDiaginalDirections.None)
            {
                if (direction == GridDiaginalDirections.Left) return GridDiaginalDirections.Right;
                if (direction == GridDiaginalDirections.Right) return GridDiaginalDirections.Left;
                if (direction == GridDiaginalDirections.Up) return GridDiaginalDirections.Down;
                if (direction == GridDiaginalDirections.Down) return GridDiaginalDirections.Up;
                if (direction == GridDiaginalDirections.UpLeft) return GridDiaginalDirections.DownRight;
                if (direction == GridDiaginalDirections.UpRight) return GridDiaginalDirections.DownLeft;
                if (direction == GridDiaginalDirections.DownLeft) return GridDiaginalDirections.UpRight;
                if (direction == GridDiaginalDirections.DownRight) return GridDiaginalDirections.UpLeft;
            }
            return GridDiaginalDirections.None;
        }

        protected override void HandleState()
        {
            if (_movingToNextGridUnit)
            {
                if (_isIgnoringCardinalObstacles && _movement.CurrentState != IgnoreCardinalObstacleMovementState)
                {
                    _movement.ChangeState(IgnoreCardinalObstacleMovementState);
                    PlayAbilityStartFeedbacks();
                }
                else if (_movement.CurrentState != CharacterStates.MovementStates.Walking)
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
        /// Performs a cardinal collision check and stores collision objects informations
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="offset"></param>
        public void DetectObstacles(float distance, Vector3 offset)
        {
            if (!_controller.PerformCardinalObstacleRaycastDetection)
            {
                return;
            }

            _controller.DetectObstacles(distance, offset);

            if (!DiaginalMovement)
                return;

            if (DimensionMode == DimensionModes.TwoD)
            {
                _raycastUpRight = MMDebug.RayCast(this.transform.position + offset, Vector3.up + Vector3.right, distance, _controller2D.ObstaclesLayerMask, Color.yellow, true);
                if (_raycastUpRight.collider != null) { DetectedObstacleUpRight = _raycastUpRight.collider.gameObject; _controller2D.CollidingWithCardinalObstacle = true; } else { DetectedObstacleUpRight = null; }
                _raycastUpLeft = MMDebug.RayCast(this.transform.position + offset, Vector3.up + Vector3.left, distance, _controller2D.ObstaclesLayerMask, Color.yellow, true);
                if (_raycastUpLeft.collider != null) { DetectedObstacleUpLeft = _raycastUpLeft.collider.gameObject; _controller2D.CollidingWithCardinalObstacle = true; } else { DetectedObstacleUpLeft = null; }
                _raycastDownRight = MMDebug.RayCast(this.transform.position + offset, Vector3.down + Vector3.right, distance, _controller2D.ObstaclesLayerMask, Color.yellow, true);
                if (_raycastDownRight.collider != null) { DetectedObstacleDownRight = _raycastDownRight.collider.gameObject; _controller2D.CollidingWithCardinalObstacle = true; } else { DetectedObstacleDownRight = null; }
                _raycastDownLeft = MMDebug.RayCast(this.transform.position + offset, Vector3.down + Vector3.left, distance, _controller2D.ObstaclesLayerMask, Color.yellow, true);
                if (_raycastDownLeft.collider != null) { DetectedObstacleDownLeft = _raycastDownLeft.collider.gameObject; _controller2D.CollidingWithCardinalObstacle = true; } else { DetectedObstacleDownLeft = null; }
            }
            else
            {
                _cardinalRaycast = MMDebug.Raycast3D(this.transform.position + offset, Vector3.forward + Vector3.right, distance, _controller3D.ObstaclesLayerMask, Color.yellow, true);
                if (_cardinalRaycast.collider != null) { DetectedObstacleUpRight = _cardinalRaycast.collider.gameObject; _controller3D.CollidingWithCardinalObstacle = true; } else { DetectedObstacleUpRight = null; }
                _cardinalRaycast = MMDebug.Raycast3D(this.transform.position + offset, Vector3.forward + Vector3.left, distance, _controller3D.ObstaclesLayerMask, Color.yellow, true);
                if (_cardinalRaycast.collider != null) { DetectedObstacleUpLeft = _cardinalRaycast.collider.gameObject; _controller3D.CollidingWithCardinalObstacle = true; } else { DetectedObstacleUpLeft = null; }
                _cardinalRaycast = MMDebug.Raycast3D(this.transform.position + offset, Vector3.back + Vector3.right, distance, _controller3D.ObstaclesLayerMask, Color.yellow, true);
                if (_cardinalRaycast.collider != null) { DetectedObstacleDownRight = _cardinalRaycast.collider.gameObject; _controller3D.CollidingWithCardinalObstacle = true; } else { DetectedObstacleDownRight = null; }
                _cardinalRaycast = MMDebug.Raycast3D(this.transform.position + offset, Vector3.back + Vector3.left, distance, _controller3D.ObstaclesLayerMask, Color.yellow, true);
                if (_cardinalRaycast.collider != null) { DetectedObstacleDownLeft = _cardinalRaycast.collider.gameObject; _controller3D.CollidingWithCardinalObstacle = true; } else { DetectedObstacleDownLeft = null; }
            }
        }
    }
}
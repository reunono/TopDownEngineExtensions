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
        public enum GridDiagonalDirections
        { None, Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight }

        [Header("Advanced Movement")]

        /// Allow diagonal movement
        [Tooltip("allow diagonal movement")]
        public bool DiagonalMovement = false;

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

        protected GridDiagonalDirections _inputDiagonalDirection;
        protected GridDiagonalDirections _currentDiagonalDirection = GridDiagonalDirections.Up;
        protected GridDiagonalDirections _bufferedDiagonalDirection;
        protected GridDiagonalDirections _newDiagonalDirection;
        protected GridDiagonalDirections _fallbackDiagonalDirection;

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

            _newDiagonalDirection = GridDiagonalDirections.None;
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
                Stop(_newDiagonalDirection);
                _newDiagonalDirection = GridDiagonalDirections.None;
                _fallbackDiagonalDirection = GridDiagonalDirections.None;
                _inputMovement = Vector3.zero;
            }

            _hasDiagonalMovement = DiagonalMovement && _verticalMovement != 0f && _horizontalMovement != 0f;

            // if we're pressing a direction for the first time, it becomes our new direction
            if (!_hasDiagonalMovement)
            {
                if ((_horizontalMovement < 0f) && !_leftPressedLastFrame) { _newDiagonalDirection = GridDiagonalDirections.Left; _inputMovement = Vector3.left; }
                if ((_horizontalMovement > 0f) && !_rightPressedLastFrame) { _newDiagonalDirection = GridDiagonalDirections.Right; _inputMovement = Vector3.right; }
                if ((_verticalMovement < 0f) && !_downPressedLastFrame) { _newDiagonalDirection = GridDiagonalDirections.Down; _inputMovement = Vector3.down; }
                if ((_verticalMovement > 0f) && !_upPressedLastFrame) { _newDiagonalDirection = GridDiagonalDirections.Up; _inputMovement = Vector3.up; }
            }
            else
            {
                if ((_verticalMovement < 0f) && (_horizontalMovement < 0f) && !_downLeftPressedLastFrame) { _newDiagonalDirection = GridDiagonalDirections.DownLeft; _inputMovement = Vector3.down + Vector3.left; }
                if ((_verticalMovement < 0f) && (_horizontalMovement > 0f) && !_downRightPressedLastFrame) { _newDiagonalDirection = GridDiagonalDirections.DownRight; _inputMovement = Vector3.down + Vector3.right; }
                if ((_verticalMovement > 0f) && (_horizontalMovement < 0f) && !_upLeftPressedLastFrame) { _newDiagonalDirection = GridDiagonalDirections.UpLeft; _inputMovement = Vector3.up + Vector3.left; }
                if ((_verticalMovement > 0f) && (_horizontalMovement > 0f) && !_upRightPressedLastFrame) { _newDiagonalDirection = GridDiagonalDirections.UpRight; _inputMovement = Vector3.up + Vector3.right; }
            }

            // if we were pressing a direction, and have just released it, we'll look for an other direction
            if (((_horizontalMovement == 0f) && (_leftPressedLastFrame || _rightPressedLastFrame)) ||
               ((_verticalMovement == 0f) && (_downPressedLastFrame || _upPressedLastFrame)) ||
               (_verticalMovement == 0f && _horizontalMovement == 0f && (_downLeftPressedLastFrame || _downRightPressedLastFrame || _upLeftPressedLastFrame || _upRightPressedLastFrame)))
            { _newDiagonalDirection = GridDiagonalDirections.None; }

            // if at this point we have no direction, we take any pressed one
            if (_newDiagonalDirection == GridDiagonalDirections.None)
            {
                if (!_hasDiagonalMovement)
                {
                    if (_horizontalMovement < 0f) { _newDiagonalDirection = GridDiagonalDirections.Left; _inputMovement = Vector3.left; }
                    if (_horizontalMovement > 0f) { _newDiagonalDirection = GridDiagonalDirections.Right; _inputMovement = Vector3.right; }
                    if (_verticalMovement < 0f) { _newDiagonalDirection = GridDiagonalDirections.Down; _inputMovement = Vector3.down; }
                    if (_verticalMovement > 0f) { _newDiagonalDirection = GridDiagonalDirections.Up; _inputMovement = Vector3.up; }
                }
                else
                {
                    if ((_verticalMovement < 0f) && (_horizontalMovement < 0f)) { _newDiagonalDirection = GridDiagonalDirections.DownLeft; _inputMovement = Vector3.down + Vector3.left; }
                    if ((_verticalMovement < 0f) && (_horizontalMovement > 0f)) { _newDiagonalDirection = GridDiagonalDirections.DownRight; _inputMovement = Vector3.down + Vector3.right; }
                    if ((_verticalMovement > 0f) && (_horizontalMovement < 0f)) { _newDiagonalDirection = GridDiagonalDirections.UpLeft; _inputMovement = Vector3.up + Vector3.left; }
                    if ((_verticalMovement > 0f) && (_horizontalMovement > 0f)) { _newDiagonalDirection = GridDiagonalDirections.UpRight; _inputMovement = Vector3.up + Vector3.right; }
                }
            }

            if (_hasDiagonalMovement && (_leftPressedLastFrame || _rightPressedLastFrame || _downPressedLastFrame || _upPressedLastFrame))
            {
                if (_leftPressedLastFrame) { _fallbackDiagonalDirection = (_newDiagonalDirection == GridDiagonalDirections.DownLeft) ? GridDiagonalDirections.Down : GridDiagonalDirections.Up; }
                if (_rightPressedLastFrame) { _fallbackDiagonalDirection = (_newDiagonalDirection == GridDiagonalDirections.DownRight) ? GridDiagonalDirections.Down : GridDiagonalDirections.Up; }
                if (_downPressedLastFrame) { _fallbackDiagonalDirection = (_newDiagonalDirection == GridDiagonalDirections.DownLeft) ? GridDiagonalDirections.Left : GridDiagonalDirections.Right; }
                if (_upPressedLastFrame) { _fallbackDiagonalDirection = (_newDiagonalDirection == GridDiagonalDirections.UpLeft) ? GridDiagonalDirections.Left : GridDiagonalDirections.Right; }
            }

            _inputDiagonalDirection = _newDiagonalDirection;

            // we store our presses for next frame
            _leftPressedLastFrame = !_hasDiagonalMovement && (_horizontalMovement < 0f);
            _rightPressedLastFrame = !_hasDiagonalMovement && (_horizontalMovement > 0f);
            _downPressedLastFrame = !_hasDiagonalMovement && (_verticalMovement < 0f);
            _upPressedLastFrame = !_hasDiagonalMovement && (_verticalMovement > 0f);
            if (DiagonalMovement)
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
        public virtual void Stop(GridDiagonalDirections direction)
        {
            if (direction == GridDiagonalDirections.None)
            {
                return;
            }
            _bufferedDiagonalDirection = direction;
            _stopBuffered = true;
        }

        /// <summary>
        /// Modifies the current speed based on the acceleration
        /// </summary>
		protected override void ApplyAcceleration()
        {
            if ((_currentDiagonalDirection != GridDiagonalDirections.None) && (CurrentSpeed < MaximumSpeed * MaximumSpeedMultiplier))
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
                if (_bufferedDiagonalDirection == GridDiagonalDirections.None)
                {
                    _currentDiagonalDirection = GridDiagonalDirections.None;
                    _bufferedDiagonalDirection = GridDiagonalDirections.None;
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
                    if ((_currentDiagonalDirection == GridDiagonalDirections.UpLeft) && ((DetectedObstacleUpLeft != null && (_controller.DetectedObstacleUp == null || _controller.DetectedObstacleLeft == null)) || (!IgnoreCardinalObstacles && (_controller.DetectedObstacleUp != null || _controller.DetectedObstacleLeft != null))))
                    {
                        _currentDiagonalDirection = (_controller.DetectedObstacleUp == null && _controller.DetectedObstacleLeft == null)
                            ? (_fallbackDiagonalDirection != GridDiagonalDirections.None) ? _fallbackDiagonalDirection : (MMMaths.RollADice(2) == 1) ? GridDiagonalDirections.Left : GridDiagonalDirections.Up
                            : (_controller.DetectedObstacleUp == null) ? GridDiagonalDirections.Up : GridDiagonalDirections.Left;
                    }
                    else if ((_currentDiagonalDirection == GridDiagonalDirections.UpRight) && ((DetectedObstacleUpRight != null && (_controller.DetectedObstacleUp == null || _controller.DetectedObstacleRight == null)) || (!IgnoreCardinalObstacles && (_controller.DetectedObstacleUp != null || _controller.DetectedObstacleRight != null))))
                    {
                        _currentDiagonalDirection = _controller.DetectedObstacleUp == null && _controller.DetectedObstacleRight == null
                            ? (_fallbackDiagonalDirection != GridDiagonalDirections.None) ? _fallbackDiagonalDirection : (MMMaths.RollADice(2) == 1) ? GridDiagonalDirections.Right : GridDiagonalDirections.Up
                            : (_controller.DetectedObstacleUp == null) ? GridDiagonalDirections.Up : GridDiagonalDirections.Right;
                    }
                    else if ((_currentDiagonalDirection == GridDiagonalDirections.DownLeft) && ((DetectedObstacleDownLeft != null && (_controller.DetectedObstacleDown == null || _controller.DetectedObstacleLeft == null)) || (!IgnoreCardinalObstacles && (_controller.DetectedObstacleDown != null || _controller.DetectedObstacleLeft != null))))
                    {
                        _currentDiagonalDirection = (_controller.DetectedObstacleUp == null && _controller.DetectedObstacleRight == null)
                            ? (_fallbackDiagonalDirection != GridDiagonalDirections.None) ? _fallbackDiagonalDirection : (MMMaths.RollADice(2) == 1) ? GridDiagonalDirections.Right : GridDiagonalDirections.Up
                            : (_controller.DetectedObstacleUp == null) ? GridDiagonalDirections.Up : GridDiagonalDirections.Right;
                    }
                    else if ((_currentDiagonalDirection == GridDiagonalDirections.DownLeft) && ((DetectedObstacleDownLeft != null && (_controller.DetectedObstacleDown == null || _controller.DetectedObstacleLeft == null)) || (!IgnoreCardinalObstacles && (_controller.DetectedObstacleDown != null || _controller.DetectedObstacleLeft != null))))
                    {
                        _currentDiagonalDirection = (_controller.DetectedObstacleDown == null && _controller.DetectedObstacleLeft == null)
                            ? (_fallbackDiagonalDirection != GridDiagonalDirections.None) ? _fallbackDiagonalDirection : (MMMaths.RollADice(2) == 1) ? GridDiagonalDirections.Left : GridDiagonalDirections.Down
                            : (_controller.DetectedObstacleDown == null) ? GridDiagonalDirections.Down : GridDiagonalDirections.Left;
                    }
                    else if ((_currentDiagonalDirection == GridDiagonalDirections.DownRight) && ((DetectedObstacleDownRight != null && (_controller.DetectedObstacleDown == null || _controller.DetectedObstacleRight == null)) || (!IgnoreCardinalObstacles && (_controller.DetectedObstacleDown != null || _controller.DetectedObstacleRight != null))))
                    {
                        _currentDiagonalDirection = _controller.DetectedObstacleDown == null && _controller.DetectedObstacleRight == null
                            ? (_fallbackDiagonalDirection != GridDiagonalDirections.None) ? _fallbackDiagonalDirection : (MMMaths.RollADice(2) == 1) ? GridDiagonalDirections.Right : GridDiagonalDirections.Down
                            : (_controller.DetectedObstacleDown == null) ? GridDiagonalDirections.Down : GridDiagonalDirections.Right;
                    }
                }

                // we check if we can move in the selected direction
                if (((_currentDiagonalDirection == GridDiagonalDirections.Left) && (_controller.DetectedObstacleLeft != null))
                    || ((_currentDiagonalDirection == GridDiagonalDirections.Right) && (_controller.DetectedObstacleRight != null))
                    || ((_currentDiagonalDirection == GridDiagonalDirections.Up) && (_controller.DetectedObstacleUp != null))
                    || ((_currentDiagonalDirection == GridDiagonalDirections.Down) && (_controller.DetectedObstacleDown != null))
                    || ((_currentDiagonalDirection == GridDiagonalDirections.UpLeft) && ((DetectedObstacleUpLeft != null) || ((DetectedObstacleUpLeft == null) && !IgnoreCardinalObstacles && ((_controller.DetectedObstacleUp != null) || (_controller.DetectedObstacleLeft != null)))))
                    || ((_currentDiagonalDirection == GridDiagonalDirections.UpRight) && ((DetectedObstacleUpRight != null) || ((DetectedObstacleUpRight == null) && !IgnoreCardinalObstacles && ((_controller.DetectedObstacleUp != null) || (_controller.DetectedObstacleRight != null)))))
                    || ((_currentDiagonalDirection == GridDiagonalDirections.DownLeft) && ((DetectedObstacleDownLeft != null) || ((DetectedObstacleDownLeft == null) && !IgnoreCardinalObstacles && ((_controller.DetectedObstacleDown != null) || (_controller.DetectedObstacleLeft != null)))))
                    || ((_currentDiagonalDirection == GridDiagonalDirections.DownRight) && ((DetectedObstacleDownRight != null) || ((DetectedObstacleDownRight == null) && !IgnoreCardinalObstacles && ((_controller.DetectedObstacleDown != null) || (_controller.DetectedObstacleRight != null))))))
                {
                    _currentDiagonalDirection = _bufferedDiagonalDirection;

                    GridManager.Instance.SetLastPosition(this.gameObject, GridManager.Instance.WorldToCellCoordinates(_endWorldPosition));
                    GridManager.Instance.SetNextPosition(this.gameObject, GridManager.Instance.WorldToCellCoordinates(_endWorldPosition));

                    return;
                }

                // we check if we can move in the selected direction
                if (((_bufferedDiagonalDirection == GridDiagonalDirections.Left) && (_controller.DetectedObstacleLeft == null))
                    || ((_bufferedDiagonalDirection == GridDiagonalDirections.Right) && (_controller.DetectedObstacleRight == null))
                    || ((_bufferedDiagonalDirection == GridDiagonalDirections.Up) && (_controller.DetectedObstacleUp == null))
                    || ((_bufferedDiagonalDirection == GridDiagonalDirections.Down) && (_controller.DetectedObstacleDown == null))
                    || ((_bufferedDiagonalDirection == GridDiagonalDirections.UpLeft) && (DetectedObstacleUpLeft == null) && (IgnoreCardinalObstacles || ((_controller.DetectedObstacleUp == null) && (_controller.DetectedObstacleLeft == null))))
                    || ((_bufferedDiagonalDirection == GridDiagonalDirections.UpRight) && (DetectedObstacleUpRight == null) && (IgnoreCardinalObstacles || ((_controller.DetectedObstacleUp == null) && (_controller.DetectedObstacleRight == null))))
                    || ((_bufferedDiagonalDirection == GridDiagonalDirections.DownLeft) && (DetectedObstacleDownLeft == null) && (IgnoreCardinalObstacles || ((_controller.DetectedObstacleDown == null) && (_controller.DetectedObstacleLeft == null))))
                    || ((_bufferedDiagonalDirection == GridDiagonalDirections.DownRight) && (DetectedObstacleDownRight == null) && (IgnoreCardinalObstacles || ((_controller.DetectedObstacleDown == null) && (_controller.DetectedObstacleRight == null)))))
                {
                    _currentDiagonalDirection = _bufferedDiagonalDirection;
                }

                // we compute and move towards our new destination
                _movingToNextGridUnit = true;
                DetermineEndPosition();

                // we make sure the target cell is free
                if (GridManager.Instance.CellIsOccupied(TargetGridPosition))
                {
                    _movingToNextGridUnit = false;
                    _currentDiagonalDirection = GridDiagonalDirections.None;
                    _bufferedDiagonalDirection = GridDiagonalDirections.None;
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
                            if (((_currentDiagonalDirection == GridDiagonalDirections.UpLeft) || (_currentDiagonalDirection == GridDiagonalDirections.UpRight)) && (_controller.DetectedObstacleUp != null))
                            {
                                Collider2D obstacleCollider = _controller.DetectedObstacleUp.GetComponent<Collider2D>();
                                Physics2D.IgnoreCollision(obstacleCollider, _collider2D, true);
                                _ignoreed2DCardinalObstacles.Add(obstacleCollider);
                                _isIgnoringCardinalObstacles = true;
                            }
                            if (((_currentDiagonalDirection == GridDiagonalDirections.DownLeft) || (_currentDiagonalDirection == GridDiagonalDirections.DownRight)) && (_controller.DetectedObstacleDown != null))
                            {
                                Collider2D obstacleCollider = _controller.DetectedObstacleDown.GetComponent<Collider2D>();
                                Physics2D.IgnoreCollision(obstacleCollider, _collider2D, true);
                                _ignoreed2DCardinalObstacles.Add(obstacleCollider);
                                _isIgnoringCardinalObstacles = true;
                            }
                            if (((_currentDiagonalDirection == GridDiagonalDirections.UpLeft) || (_currentDiagonalDirection == GridDiagonalDirections.DownLeft)) && (_controller.DetectedObstacleLeft != null))
                            {
                                Collider2D obstacleCollider = _controller.DetectedObstacleLeft.GetComponent<Collider2D>();
                                Physics2D.IgnoreCollision(obstacleCollider, _collider2D, true);
                                _ignoreed2DCardinalObstacles.Add(obstacleCollider);
                                _isIgnoringCardinalObstacles = true;
                            }
                            if (((_currentDiagonalDirection == GridDiagonalDirections.DownRight) || (_currentDiagonalDirection == GridDiagonalDirections.UpRight)) && (_controller.DetectedObstacleRight != null))
                            {
                                Collider2D obstacleCollider = _controller.DetectedObstacleRight.GetComponent<Collider2D>();
                                Physics2D.IgnoreCollision(obstacleCollider, _collider2D, true);
                                _ignoreed2DCardinalObstacles.Add(obstacleCollider);
                                _isIgnoringCardinalObstacles = true;
                            }
                        }
                        else
                        {
                            if (((_currentDiagonalDirection == GridDiagonalDirections.UpLeft) || (_currentDiagonalDirection == GridDiagonalDirections.UpRight)) && (_controller.DetectedObstacleUp != null))
                            {
                                Collider obstacleCollider = _controller.DetectedObstacleUp.GetComponent<Collider>();
                                Physics.IgnoreCollision(obstacleCollider, _collider, true);
                                _ignoreed3DCardinalObstacles.Add(obstacleCollider);
                                _isIgnoringCardinalObstacles = true;
                            }
                            if (((_currentDiagonalDirection == GridDiagonalDirections.DownLeft) || (_currentDiagonalDirection == GridDiagonalDirections.DownRight)) && (_controller.DetectedObstacleDown != null))
                            {
                                Collider obstacleCollider = _controller.DetectedObstacleDown.GetComponent<Collider>();
                                Physics.IgnoreCollision(obstacleCollider, _collider, true);
                                _ignoreed3DCardinalObstacles.Add(obstacleCollider);
                                _isIgnoringCardinalObstacles = true;
                            }
                            if (((_currentDiagonalDirection == GridDiagonalDirections.UpLeft) || (_currentDiagonalDirection == GridDiagonalDirections.DownRight)) && (_controller.DetectedObstacleLeft != null))
                            {
                                Collider obstacleCollider = _controller.DetectedObstacleLeft.GetComponent<Collider>();
                                Physics.IgnoreCollision(obstacleCollider, _collider, true);
                                _ignoreed3DCardinalObstacles.Add(obstacleCollider);
                                _isIgnoringCardinalObstacles = true;
                            }
                            if (((_currentDiagonalDirection == GridDiagonalDirections.DownLeft) || (_currentDiagonalDirection == GridDiagonalDirections.UpRight)) && (_controller.DetectedObstacleRight != null))
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
            if ((_inputDiagonalDirection != GridDiagonalDirections.None) && !_stopBuffered)
            {
                _bufferedDiagonalDirection = _inputDiagonalDirection;
                _lastBufferInGridUnits = BufferSize;
            }

            // if we're not moving and get an input, we start moving
            if (!_agentMoving && _inputDiagonalDirection != GridDiagonalDirections.None)
            {
                _currentDiagonalDirection = _inputDiagonalDirection;
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
            if ((_bufferedDiagonalDirection != GridDiagonalDirections.None) && !_movingToNextGridUnit && (_inputDiagonalDirection == GridDiagonalDirections.None) && UseInputBuffer)
            {
                // we reduce the buffer counter
                _lastBufferInGridUnits--;
                // if our buffer is expired, we revert to our current direction
                if ((_lastBufferInGridUnits < 0) && (_bufferedDiagonalDirection != _currentDiagonalDirection))
                {
                    _bufferedDiagonalDirection = _currentDiagonalDirection;
                }
            }

            // if we have a stop planned and are not moving, we stop
            if ((_stopBuffered) && !_movingToNextGridUnit)
            {
                _bufferedDiagonalDirection = GridDiagonalDirections.None;
                _stopBuffered = false;
            }
        }

        /// <summary>
        /// Determines the end position based on the current direction
        /// </summary>
		protected override void DetermineEndPosition()
        {
            TargetGridPosition = CurrentCellCoordinates + ConvertDirectionToVector3Int(_currentDiagonalDirection);
            _endWorldPosition = GridManager.Instance.CellToWorldCoordinates(TargetGridPosition);
            // we maintain our z(2D) or y (3D)
            _endWorldPosition = DimensionClamp(_endWorldPosition);
        }

        protected virtual Vector3Int ConvertDirectionToVector3Int(GridDiagonalDirections direction)
        {
            if (direction != GridDiagonalDirections.None)
            {
                if (direction == GridDiagonalDirections.Left) return Vector3Int.left;
                if (direction == GridDiagonalDirections.Right) return Vector3Int.right;

                if (DimensionMode == DimensionModes.TwoD)
                {
                    if (direction == GridDiagonalDirections.Up) return Vector3Int.up;
                    if (direction == GridDiagonalDirections.Down) return Vector3Int.down;
                    if (direction == GridDiagonalDirections.UpLeft) return Vector3Int.up + Vector3Int.left;
                    if (direction == GridDiagonalDirections.UpRight) return Vector3Int.up + Vector3Int.right;
                    if (direction == GridDiagonalDirections.DownLeft) return Vector3Int.down + Vector3Int.left;
                    if (direction == GridDiagonalDirections.DownRight) return Vector3Int.down + Vector3Int.right;
                }
                else
                {
                    if (direction == GridDiagonalDirections.Up) return Vector3Int.RoundToInt(Vector3.forward);
                    if (direction == GridDiagonalDirections.Down) return Vector3Int.RoundToInt(Vector3.back);
                    if (direction == GridDiagonalDirections.UpLeft) return Vector3Int.RoundToInt(Vector3.forward + Vector3.left);
                    if (direction == GridDiagonalDirections.UpRight) return Vector3Int.RoundToInt(Vector3.forward + Vector3.right);
                    if (direction == GridDiagonalDirections.DownLeft) return Vector3Int.RoundToInt(Vector3.back + Vector3.left);
                    if (direction == GridDiagonalDirections.DownRight) return Vector3Int.RoundToInt(Vector3.back + Vector3.right);
                }
            }
            return Vector3Int.zero;
        }

        /// <summary>
        /// Returns the opposite direction of a GridDirection
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected virtual GridDiagonalDirections GetInverseDirection(GridDiagonalDirections direction)
        {
            if (direction != GridDiagonalDirections.None)
            {
                if (direction == GridDiagonalDirections.Left) return GridDiagonalDirections.Right;
                if (direction == GridDiagonalDirections.Right) return GridDiagonalDirections.Left;
                if (direction == GridDiagonalDirections.Up) return GridDiagonalDirections.Down;
                if (direction == GridDiagonalDirections.Down) return GridDiagonalDirections.Up;
                if (direction == GridDiagonalDirections.UpLeft) return GridDiagonalDirections.DownRight;
                if (direction == GridDiagonalDirections.UpRight) return GridDiagonalDirections.DownLeft;
                if (direction == GridDiagonalDirections.DownLeft) return GridDiagonalDirections.UpRight;
                if (direction == GridDiagonalDirections.DownRight) return GridDiagonalDirections.UpLeft;
            }
            return GridDiagonalDirections.None;
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

            if (!DiagonalMovement)
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
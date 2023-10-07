using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Linq;
using UnityEngine;

namespace PhluxApps.TopDownEngine
{
    /// <summary>
    /// Requires a CharacterGridMovement ability. Makes the character move randomly in the grid,
    /// until it finds an obstacle in its path, in which case it'll pick a new direction at random
    /// Supports both 2D and 3D grids
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/AI/Actions/AIAdvancedActionMoveRandomlyGrid")]
    public class AIAdvancedActionMoveRandomlyGrid : AIActionMoveRandomlyGrid
    {
        protected int _directions = 4;

        /// <summary>
        /// On start we grab our character movement component and pick a random direction
        /// </summary>
        public override void Initialization()
        {
            if (!ShouldInitialize) return;
            base.Initialization();
            AdvancedCharacterGridMovement acgm = gameObject.GetComponentInParent<Character>()?.FindAbility<AdvancedCharacterGridMovement>();
            if (acgm != null && acgm.DiaginalMovement)
            {
                _directions = 8;
                _raycastDirections2D = _raycastDirections2D.Concat(new[] { Vector2.up + Vector2.left, Vector2.up + Vector2.right, Vector2.down + Vector2.left, Vector2.down + Vector2.right }).ToArray();
                _raycastDirections3D = _raycastDirections3D.Concat(new[] { Vector3.forward + Vector3.left, Vector3.forward + Vector3.right, Vector3.back + Vector3.left, Vector3.back + Vector3.right }).ToArray();

                PickNewDirection();
            }
        }

        /// <summary>
        /// Tests and picks a new direction to move towards
        /// </summary>
        protected override void PickNewDirection()
        {
            int retries = 0;
            switch (Mode)
            {
                case Modes.ThreeD:
                    while (retries < 10)
                    {
                        retries++;
                        int random = MMMaths.RollADice(_directions) - 1;
                        _temp3DVector = _raycastDirections3D[random];

                        if (Avoid180)
                        {
                            if ((_temp3DVector.x == -_direction.x) && (Mathf.Abs(_temp3DVector.x) > 0))
                            {
                                continue;
                            }
                            if ((_temp3DVector.y == -_direction.y) && (Mathf.Abs(_temp3DVector.y) > 0))
                            {
                                continue;
                            }
                        }

                        _hit = MMDebug.Raycast3D(_collider.bounds.center, _temp3DVector, ObstaclesDetectionDistance, ObstacleLayerMask, Color.gray);
                        if (_hit.collider == null)
                        {
                            _direction = _temp3DVector;
                            _direction.y = _temp3DVector.z;

                            return;
                        }
                    }
                    break;

                case Modes.TwoD:
                    while (retries < 10)
                    {
                        retries++;

                        int random = MMMaths.RollADice(_directions) - 1;
                        _temp2DVector = _raycastDirections2D[random];

                        if (Avoid180)
                        {
                            if ((_temp2DVector.x == -_direction.x) && (Mathf.Abs(_temp2DVector.x) > 0))
                            {
                                continue;
                            }
                            if ((_temp2DVector.y == -_direction.y) && (Mathf.Abs(_temp2DVector.y) > 0))
                            {
                                continue;
                            }
                        }

                        _hit2D = MMDebug.RayCast(_collider2D.bounds.center, _temp2DVector, ObstaclesDetectionDistance, ObstacleLayerMask, Color.gray);
                        if (_hit2D.collider == null)
                        {
                            _direction = _temp2DVector;

                            return;
                        }
                    }
                    break;
            }
        }
    }
}
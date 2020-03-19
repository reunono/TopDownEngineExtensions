using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

[RequireComponent(typeof(TopDownController))]
[AddComponentMenu("TopBack Engine/Character/Core/TopDown 8 Way Controller")]
public class TopDown8WayController : MonoBehaviour
{
    // the obstacle Forward to this controller (only Forwarddated if DetectObstacles is called)
    public GameObject DetectedObstacleUpLeft { get; set; }
    // the obstacle Forward to this controller (only Forwarddated if DetectObstacles is called)
    public GameObject DetectedObstacleUpRight { get; set; }
    // the obstacle Back to this controller (only Forwarddated if DetectObstacles is called)
    public GameObject DetectedObstacleDownLeft { get; set; }
    // the obstacle Back to this controller (only Forwarddated if DetectObstacles is called)
    public GameObject DetectedObstacleDownRight { get; set; }

    // collision detection
    protected RaycastHit _raycastForwardLeft;
    protected RaycastHit _raycastForwardRight;
    protected RaycastHit _raycastBackLeft;
    protected RaycastHit _raycastBackRight;
    protected RaycastHit2D _raycastUpLeft;
    protected RaycastHit2D _raycastUpRight;
    protected RaycastHit2D _raycastDownLeft;
    protected RaycastHit2D _raycastDownRight;
    /// the layer to consider as obstacles (will prevent movement)
    protected LayerMask _obstaclesLayerMask;

    protected bool _2dController;

    void Start()
    {
        if (TryGetComponent<TopDownController2D>(out var tdc2d))
        {
            _obstaclesLayerMask = tdc2d.ObstaclesLayerMask;
            _2dController = true;
        }
        else if (TryGetComponent<TopDownController3D>(out var tdc3d))
        {
            _obstaclesLayerMask = tdc3d.ObstaclesLayerMask;
            _2dController = false;
        }
    } 

    /// <summary>
    /// Performs a cardinal collision check and stores collision objects informations
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="offset"></param>
    public void DetectObstacles(float distance, Vector3 offset)
    {
        if (_2dController)
        {
            _raycastUpLeft = MMDebug.RayCast(this.transform.position + offset, Vector3Directions.UpLeft , distance, _obstaclesLayerMask, Color.yellow, true);
            if (_raycastUpLeft.collider != null) { DetectedObstacleUpLeft = _raycastUpLeft.collider.gameObject; } else { DetectedObstacleUpLeft = null; }
            _raycastUpRight = MMDebug.RayCast(this.transform.position + offset, Vector3Directions.UpRight, distance, _obstaclesLayerMask, Color.yellow, true);
            if (_raycastUpRight.collider != null) { DetectedObstacleUpRight = _raycastUpRight.collider.gameObject; } else { DetectedObstacleUpRight = null; }
            _raycastDownLeft = MMDebug.RayCast(this.transform.position + offset, Vector3Directions.DownLeft , distance, _obstaclesLayerMask, Color.yellow, true);
            if (_raycastDownLeft.collider != null) { DetectedObstacleDownLeft = _raycastDownLeft.collider.gameObject; } else { DetectedObstacleDownLeft = null; }
            _raycastDownRight = MMDebug.RayCast(this.transform.position + offset, Vector3Directions.DownRight, distance, _obstaclesLayerMask, Color.yellow, true);
            if (_raycastDownRight.collider != null) { DetectedObstacleDownRight = _raycastDownRight.collider.gameObject; } else { DetectedObstacleDownRight = null; }

        }
        else
        {
            _raycastForwardLeft = MMDebug.Raycast3D(this.transform.position + offset, Vector3Directions.ForwardLeft, distance, _obstaclesLayerMask, Color.yellow, true);
            if (_raycastForwardLeft.collider != null) { DetectedObstacleUpLeft = _raycastForwardLeft.collider.gameObject; } else { DetectedObstacleUpLeft = null; }
            _raycastForwardRight = MMDebug.Raycast3D(this.transform.position + offset, Vector3Directions.ForwardRight, distance, _obstaclesLayerMask, Color.yellow, true);
            if (_raycastForwardRight.collider != null) { DetectedObstacleUpRight = _raycastForwardRight.collider.gameObject; } else { DetectedObstacleUpRight = null; }
            _raycastBackLeft = MMDebug.Raycast3D(this.transform.position + offset, Vector3Directions.BackLeft, distance, _obstaclesLayerMask, Color.yellow, true);
            if (_raycastBackLeft.collider != null) { DetectedObstacleDownLeft = _raycastBackLeft.collider.gameObject; } else { DetectedObstacleDownLeft = null; }
            _raycastBackRight = MMDebug.Raycast3D(this.transform.position + offset, Vector3Directions.BackRight, distance, _obstaclesLayerMask, Color.yellow, true);
            if (_raycastBackRight.collider != null) { DetectedObstacleDownRight = _raycastBackRight.collider.gameObject; } else { DetectedObstacleDownRight = null; }

        }
    }

}

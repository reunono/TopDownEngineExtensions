using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

[RequireComponent(typeof(TopDownController))]
[RequireComponent(typeof(AdvancedCharacterGridMovement))]
[AddComponentMenu("TopBack Engine/Character/Core/Advanced TopDown Controller")]
public class AdvancedTopDownController : MonoBehaviour
{
    /// the layer to consider as pushable
    public LayerMask PushablesLayerMask;

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
    protected TopDownController2D _controller2D;
    protected TopDownController3D _controller3D;
    void Start()
    {
        if (TryGetComponent<TopDownController2D>(out var tdc2d))
        {
            _controller2D = tdc2d;
            _obstaclesLayerMask = tdc2d.ObstaclesLayerMask;
            _2dController = true;
        }
        else if (TryGetComponent<TopDownController3D>(out var tdc3d))
        {
            _controller3D = tdc3d;
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

    /// <summary>
    /// Performs a cardinal pushable check and stores pushable objects informations
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="offset"></param>
    public void DetectPushables(float distance, Vector3 offset)
    {
        var pos = this.transform.position + offset;
        if (_2dController)
        {
            if (_controller2D.DetectedObstacleRight == null) { _controller2D.DetectedObstacleRight = DetectPushable2D(distance, pos, Vector3.right); };
            if (_controller2D.DetectedObstacleLeft == null) { _controller2D.DetectedObstacleLeft = DetectPushable2D(distance, pos, Vector3.left); };
            if (_controller2D.DetectedObstacleUp == null) { _controller2D.DetectedObstacleUp = DetectPushable2D(distance, pos, Vector3.up); };
            if (_controller2D.DetectedObstacleDown == null) { _controller2D.DetectedObstacleDown = DetectPushable2D(distance, pos, Vector3.down); };




            //_raycastRight = MMDebug.DrawPoint(this.transform.position + offset, Vector3.right, distance, PushablesLayerMask, Color.blue, true);
            //if (_raycastRight.collider != null) { DetectedPushableRight = _raycastRight.collider.gameObject; } else { DetectedPushableRight = null; }
            //_raycastLeft = MMDebug.RayCast(this.transform.position + offset, Vector3.left, distance, PushablesLayerMask, Color.blue, true);
            //if (_raycastLeft.collider != null) { DetectedPushableLeft = _raycastLeft.collider.gameObject; } else { DetectedPushableLeft = null; }
            //_raycastUp = MMDebug.RayCast(this.transform.position + offset, Vector3.up, distance, PushablesLayerMask, Color.blue, true);
            //if (_raycastUp.collider != null) { DetectedPushableUp = _raycastUp.collider.gameObject; } else { DetectedPushableUp = null; }
            //_raycastDown = MMDebug.RayCast(this.transform.position + offset, Vector3.down, distance, PushablesLayerMask, Color.blue, true);
            //if (_raycastDown.collider != null) { DetectedPushableDown = _raycastDown.collider.gameObject; } else { DetectedPushableDown = null; }

        }
        else
        {
            //_controller3D.DetectedObstacleRight = DetectPushable3D(distance, pos, Vector3.right);
            //_controller3D.DetectedObstacleLeft = DetectPushable3D(distance, pos, Vector3.left);
            //_controller3D.DetectedObstacleUp = DetectPushable3D(distance, pos, Vector3.forward);
            //_controller3D.DetectedObstacleDown = DetectPushable3D(distance, pos, Vector3.back);
            //_raycastRight3d = MMDebug.Raycast3D(this.transform.position + offset, Vector3.right, distance, PushablesLayerMask, Color.blue, true);
            //if (_raycastRight3d.collider != null) { DetectedPushableRight = _raycastRight.collider.gameObject; } else { DetectedPushableRight = null; }
            //_raycastLeft3d = MMDebug.Raycast3D(this.transform.position + offset, Vector3.left, distance, PushablesLayerMask, Color.blue, true);
            //if (_raycastLeft3d.collider != null) { DetectedPushableLeft = _raycastLeft.collider.gameObject; } else { DetectedPushableLeft = null; }
            //_raycastForward = MMDebug.Raycast3D(this.transform.position + offset, Vector3.forward, distance, PushablesLayerMask, Color.blue, true);
            //if (_raycastUp.collider != null) { DetectedPushableUp = _raycastUp.collider.gameObject; } else { DetectedPushableUp = null; }
            //_raycastBack = MMDebug.Raycast3D(this.transform.position + offset, Vector3.back, distance, PushablesLayerMask, Color.blue, true);
            //if (_raycastDown.collider != null) { DetectedPushableDown = _raycastDown.collider.gameObject; } else { DetectedPushableDown = null; }

        }
    }

    private GameObject DetectPushable2D(float distance, Vector3 position, Vector3 direction)
    {
        var collider = Physics2D.OverlapCircle(position + (direction * distance), .25f, PushablesLayerMask);
        if (collider == null)
        {
            collider = Physics2D.OverlapCircle(position + (direction * distance), .25f, _obstaclesLayerMask);
            if (collider != null)
            {
                return collider.gameObject;
            }
        }
        else if (DetectPushable2D(distance, collider.gameObject.transform.position, direction) != null)
        {
            return collider.gameObject;
        }

        return null;
    }

}

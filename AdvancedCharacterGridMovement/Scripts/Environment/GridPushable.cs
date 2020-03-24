using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("TopDown Engine/Character/Abilities/Object Grid Pushable")]
public class GridPushable : MonoBehaviour
{
    /// the layer to consider as pushable
    public LayerMask PushablesLayerMask;

    private Rigidbody2D _rb2D;
    //private bool moving;
    //private float smoothTime = .1f;

    private Vector3 lastPosition;
    private Vector3 lastVelocity;
    private Vector3 lastAcceleration;

    // Use this for initialization
    void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        lastPosition = transform.position;
        lastVelocity = Vector3.zero;
        lastAcceleration = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		//Todo: WIP
        Vector3 position = transform.position;
        Vector3 velocity = (position - lastPosition) / Time.deltaTime;
        Vector3 acceleration = (velocity - lastVelocity) / Time.deltaTime;
        if (Mathf.Approximately(Mathf.Abs(acceleration.magnitude - lastAcceleration.magnitude),0f))
        {
            // Vector3 pos = transform.position / GridManager.Instance.GridUnitSize;

            // Vector3 targetPosition = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));

            // targetPosition *= GridManager.Instance.GridUnitSize;

            // transform.position = targetPosition;
            Debug.Log("Stop");        }
        else if (acceleration.magnitude > lastAcceleration.magnitude)
        {
            Debug.Log("Accelerating");
            // Accelerating
        }
        else
        {
            // Debug.Log("Decelerating");
            // Vector3 pos = transform.position / GridManager.Instance.GridUnitSize;

            // Vector3 targetPosition = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));

            // transform.position = Vector3.Lerp(transform.position, targetPosition, .1f);

            // _rb2D.velocity = velocity;
        }
        lastAcceleration = acceleration;
        lastVelocity = velocity;
        lastPosition = position;

        //if (Mathf.Approximately(_rb2D.velocity.magnitude, 0f))
        //{
        //    moving = false;

        //    Vector3 pos = transform.position / GridManager.Instance.GridUnitSize;

        //    Vector3 placePosition = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));

        //    placePosition *= GridManager.Instance.GridUnitSize;

        //    transform.position = placePosition;
        //}

        //Collider[] hitColliders = Physics.OverlapBox(new Vector3(transform.position.x, transform.position.y - GridManager.Instance.GridUnitSize, transform.position.z), new Vector3(GridManager.Instance.GridUnitSize / 2, GridManager.Instance.GridUnitSize / 2, GridManager.Instance.GridUnitSize / 2), Quaternion.Euler(Vector3.zero), PushablesLayerMask);

        //if (hitColliders.Length == 0)
        //{
        //    moving = false;


        //}

        //if (!moving)
        //{
        //    Vector3 targetPosition = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));

        //    Vector3 velocity = _rb2D.velocity;
        //    // Smoothly move the camera towards that target position
        //    transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        //    //rb.isKinematic = false;
        //    _rb2D.velocity = velocity;
        //}
    }
}

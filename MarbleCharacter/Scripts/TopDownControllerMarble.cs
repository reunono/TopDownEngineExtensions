using MoreMountains.TopDownEngine;
using UnityEngine;

// Physics driven marble controller. Requires dynamic Rigidbody and a sphere collider
public class TopDownControllerMarble : TopDownController
{
    // grounded check from Catlike Coding's movement tutorial https://catlikecoding.com/unity/tutorials/movement/physics/
    [Range(0f, 90f)]
    public float MaxGroundAngle = 45f;
    [Tooltip("the speed at which external forces get lerped to zero")]
    public float ImpactFalloff = 5f;
    private Rigidbody _rigidbody;
    private float _minGroundDotProduct;

    private void OnValidate()
    {
        _minGroundDotProduct = Mathf.Cos(MaxGroundAngle * Mathf.Deg2Rad);
    }

    protected override void Awake()
    {
        base.Awake();
        OnValidate();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision) { EvaluateCollision(collision); }
    private void OnCollisionStay(Collision collision) { EvaluateCollision(collision); }
    private void EvaluateCollision(Collision collision)
    {
        for (var i = 0; i < collision.contactCount; i++)
        {
            var normal = collision.GetContact(i).normal;
            Grounded |= normal.y >= _minGroundDotProduct;
        }
    }

    public override void Impact(Vector3 direction, float force) { _impact += direction.normalized * force; }

    private void ApplyImpact()
    {
        if (_impact.magnitude > .2f) _rigidbody.AddForce(_impact);
        _impact = Vector3.Lerp(_impact, Vector3.zero, ImpactFalloff * Time.deltaTime);
    }
    
    public override void AddForce(Vector3 movement) { Impact(movement, movement.magnitude); }
    
    public override void SetMovement(Vector3 movement) { CurrentMovement = movement; }
    
    protected override void FixedUpdate()
    {
        ApplyImpact();
        _rigidbody.AddForce(CurrentMovement);
        Grounded = false;
    }

    public override void SetKinematic(bool state) { _rigidbody.isKinematic = state; }
}

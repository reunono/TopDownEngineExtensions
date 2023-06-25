using UnityEngine;

public class VehicleController : MonoBehaviour 
{
	[Header("Rigidbody")]
	[SerializeField] private Vector3 CenterOfMass;
	[Header("Acceleration")]
	[SerializeField] private float AccelerationSpeed;
	[Tooltip("How to apply the turning force relative to mass.")]
	[SerializeField] private ForceMode AccelerationMode;
	[SerializeField] private Vector3 AccelerationOrigin;
	[Header("Turning")]
	[SerializeField] private float TurnSpeed;
	[Tooltip("How to apply the turning force relative to mass.")]
	[SerializeField] private ForceMode TurnMode;
	[Tooltip("Spin the vehicle when stationary, to allow tank turns.")]
	[SerializeField] private bool TurnWhenStationary;
	[Header("Traction")]
	[SerializeField] private float NormalTractionConstant;
	[SerializeField] private float DriftTractionConstant;
	private float _tractionConstant;

	private Rigidbody _rigidbody;
	private float _axisH;
	private float _axisV;

	private void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_rigidbody.centerOfMass = CenterOfMass;
		_tractionConstant = NormalTractionConstant;
	} 

	private void Update()
	{
		// Get Input
		_axisH = Input.GetAxis("Player1_Horizontal");
		_axisV = Input.GetAxis("Player1_Vertical");
	}

	private void FixedUpdate()
	{
		// Acceleration
		var desiredAcceleration = AccelerationSpeed * _axisV * Time.fixedDeltaTime * transform.forward;
		_rigidbody.AddForceAtPosition(desiredAcceleration, transform.position + AccelerationOrigin, AccelerationMode);

		// Turn
		var forwardVelocity = transform.InverseTransformDirection(_rigidbody.velocity).z;
		if (_axisV != 0 || Mathf.Abs(forwardVelocity) > 3 || TurnWhenStationary)
		{
			var desiredTurn = TurnSpeed * _axisH * Time.fixedDeltaTime * Mathf.Sign(forwardVelocity) * Vector3.up;
			_rigidbody.AddTorque(desiredTurn, TurnMode);
		}

		// Traction
		if (Input.GetKeyDown(KeyCode.LeftShift)) _tractionConstant = DriftTractionConstant;
		if (Input.GetKeyUp(KeyCode.LeftShift)) _tractionConstant = NormalTractionConstant;
		var desiredTraction = transform.InverseTransformDirection(_rigidbody.velocity);
		_rigidbody.AddForce(-desiredTraction.x * _tractionConstant * transform.right);
	}
}

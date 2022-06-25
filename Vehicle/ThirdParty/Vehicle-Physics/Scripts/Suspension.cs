using System;
using UnityEngine;

public class Suspension : MonoBehaviour 
{
	private Rigidbody _rigidbody;
	[SerializeField] private float WheelRadius = 0.5f;
	[SerializeField] private float SuspensionDistanceConstant = 1f;
	[SerializeField] private float SpringConstant = 30000f;
	[SerializeField] private float DamperConstant = 4000f;

	private float _previousSuspensionDistance;
	private float _currentSuspensionDistance;
	private float _springVelocity;
	private float _springForce;
	private float _damperForce;

	private void Awake()
	{
		_rigidbody = GetComponentInParent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		// Vehicle uses a raycast suspension system
		var ray = new Ray(transform.position, -transform.up);
		if (!Physics.Raycast(ray, out var hit, SuspensionDistanceConstant + WheelRadius)) return;
		// Hooke's Law
		_previousSuspensionDistance = _currentSuspensionDistance;
		_currentSuspensionDistance = SuspensionDistanceConstant - (hit.distance - WheelRadius);
		_springVelocity = (_currentSuspensionDistance - _previousSuspensionDistance) / Time.fixedDeltaTime;
		_springForce = SpringConstant * _currentSuspensionDistance;
		_damperForce = DamperConstant * _springVelocity;

		// Apply force to car body
		_rigidbody.AddForceAtPosition(transform.up * (_springForce + _damperForce), transform.position, ForceMode.Force);
	}

	private void OnDrawGizmos()
	{
		Debug.DrawRay(transform.position, -transform.up * SuspensionDistanceConstant, Color.blue, 0f);
	}
}

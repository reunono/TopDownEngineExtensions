using System;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class ProjectileHoming : MonoBehaviour
{
    [Tooltip("The angular speed (in degrees per second) at which the projectile will rotate towards its target")]
    public float AngularSpeed = 100;
    [Header("Target Detection")]
    public LayerMask TargetLayerMask = LayerManager.EnemiesLayerMask;
    public LayerMask ObstacleLayerMask = LayerManager.ObstaclesLayerMask;
    public int NumberOfRays = 15;
    [Range(0, 360)]
    public float MaxAngle = 60;
    public float MaxDistance = 30;
    private bool _is3D;
    private Collider _target;
    private Collider2D _target2D;
    private float _speed;
    private Projectile _projectile;

    private void Awake()
    {
        OnValidate();
        _is3D = GetComponent<Collider>();
        _projectile = GetComponent<Projectile>();
    }

    private void OnEnable()
    {
        _projectile.OnSpawnComplete -= OnSpawnComplete;
        _projectile.OnSpawnComplete += OnSpawnComplete;
    }

    private void OnSpawnComplete()
    {
        if (_is3D) _target = TargetDetector.GetTarget3D(transform.position, _projectile.Direction, MaxDistance, MaxAngle, NumberOfRays, TargetLayerMask, ObstacleLayerMask);
        else _target2D = TargetDetector.GetTarget2D(transform.position, _projectile.Direction, MaxDistance, MaxAngle, NumberOfRays, TargetLayerMask, ObstacleLayerMask);
        enabled = _target || _target2D;
    }
    
    private void Update()
    {
        Vector3 targetDirection;
        if (_is3D) targetDirection = _target.bounds.center - transform.position;
        else targetDirection = _target2D.bounds.center - transform.position;
        _projectile.Direction = Vector3.RotateTowards(_projectile.Direction, targetDirection, _speed * Time.deltaTime, 0);
        if (Vector3.Angle(targetDirection, _projectile.Direction) > 90) enabled = false; // stops the homing behaviour if we missed the target
    }

    private void OnValidate()
    {
        NumberOfRays = Math.Max(1, NumberOfRays);
        _speed = Mathf.Deg2Rad * AngularSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            _is3D = GetComponent<Collider>();
            _projectile = GetComponent<Projectile>();
        }
        var projectileDirection = _projectile.Direction;
        float step;
        float start;
        var numberOfRays = NumberOfRays;
        if (NumberOfRays % 2 == 1)
        {
            DrawLine(0);
            numberOfRays--;
            step = MaxAngle / numberOfRays;
            start = step;
        }
        else
        {
            step = MaxAngle / (numberOfRays + 1);
            start = step / 2;
        }

        for (var i = 0; i < numberOfRays/2; i++)
        {
            var angle = start + i * step;
            DrawLine(-angle);
            DrawLine(angle);
        }
        void DrawLine(float angle) => Gizmos.DrawRay(transform.position, (_is3D ? Quaternion.Euler(0, angle, 0) : Quaternion.Euler(0, 0, angle)) * projectileDirection * MaxDistance);
    }
}

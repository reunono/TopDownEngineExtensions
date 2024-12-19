using System;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine;

public class EllipticMotion : MonoBehaviour
{
    [MMVector("Width", "Height")] public Vector2 Size = new(.1f, 0);
    public float Speed = 9;
    public bool Custom;
    [MMCondition("Custom", true, true)] public bool Horizontal = true;
    [MMCondition("Custom", true)] public Vector3 XAxis = Vector3.right;
    [MMCondition("Custom", true)] public Vector3 YAxis = Vector3.forward;
    private Vector3 _x;
    private Vector3 _y;
    private float _time;
    private Rigidbody2D _rigidbody2D;
    private Action<EllipticMotion> _move;

    private void Awake()
    {
        if (TryGetComponent(out _rigidbody2D)) _move = Move2D;
        else _move = Move3D;

        void Move2D(EllipticMotion e) => e._rigidbody2D.position = e.transform.position + Mathf.Sin(e._time) * e.Size.y * e._y + Mathf.Cos(e._time) * e.Size.x * e._x;
        void Move3D(EllipticMotion e) => e.transform.Translate(Mathf.Sin(e._time) * e.Size.y * e._y + Mathf.Cos(e._time) * e.Size.x * e._x, Space.World);
    }

    private void OnEnable()
    {
        StartCoroutine(Initialize());
        IEnumerator Initialize()
        {
            yield return null;
            if (Custom)
            {
                _x = transform.TransformVector(XAxis.normalized);
                _y = transform.TransformVector(YAxis.normalized);
            }
            else
            {
                _x = !Horizontal || _rigidbody2D ? transform.up : transform.right;
                _y = _rigidbody2D ? transform.right : transform.forward;
            }
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;
        _move(this);
        _time += Time.deltaTime * Speed;
    }
}

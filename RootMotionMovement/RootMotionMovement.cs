using MoreMountains.TopDownEngine;
using UnityEngine;

public class RootMotionMovement : MonoBehaviour
{
    private TopDownController3D _controller;
    private Animator _animator;

    private void Awake()
    {
        _controller = GetComponentInParent<TopDownController3D>();
        _animator = GetComponent<Animator>();
    }

    public void OnAnimatorMove()
    {
        _controller.MovePosition(_controller.transform.position + _animator.deltaPosition);
        transform.rotation *= _animator.deltaRotation;
    }
}
using MoreMountains.Tools;
using UnityEngine;

namespace ConveyorBelts.Scripts
{
    [DisallowMultipleComponent]
    public class ConveyorBelt : MonoBehaviour
    {
        [SerializeField] private Vector2 Direction = Vector2.up;
        [SerializeField] private float Speed = 3;
        private Material _material;
        private void Awake() => _material = GetComponentInChildren<MeshRenderer>().material;
        private void Update() => _material.mainTextureOffset += Time.deltaTime * 1/transform.lossyScale.z * Speed * Direction.normalized;
        private Vector3 WorldSpaceDirection => transform.TransformDirection(new Vector3(Direction.x, 0, Direction.y)).normalized;
        private void OnTriggerStay(Collider other) => other.GetComponent<IConveyorMovable>()?.Move(Speed * WorldSpaceDirection);
        private void OnDrawGizmos() => MMDebug.DrawGizmoArrow(transform.position-transform.lossyScale.z/3*WorldSpaceDirection+Vector3.up*.3f, .6f*transform.lossyScale.z*WorldSpaceDirection, MMColors.ReunoYellow, 1);
    }
}

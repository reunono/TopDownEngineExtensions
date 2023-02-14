using MoreMountains.Tools;
using UnityEngine;

namespace ConveyorBelts.Scripts
{
    [DisallowMultipleComponent]
    public class ConveyorBelt : MonoBehaviour
    {
        [SerializeField] private LayerMask TargetLayerMask;
        [SerializeField] private Vector2 Direction = Vector2.up;
        [SerializeField] private float Speed = 3;
        private Material _material;
        private Vector3 WorldSpaceDirection => transform.TransformDirection(new Vector3(Direction.x, 0, Direction.y)).normalized;

        private void Reset() => TargetLayerMask = LayerMask.GetMask("Obstacles", "Default", "Player", "Enemies");
        private void Awake() => _material = GetComponentInChildren<MeshRenderer>().material;
        private void OnTriggerEnter(Collider other)
        {
            if (MMLayers.LayerInLayerMask(other.gameObject.layer, TargetLayerMask))
                other.GetComponent<IConveyorMovable>()?.AddMovement(Speed * WorldSpaceDirection);
        }
        private void OnTriggerExit(Collider other)
        {
            if (MMLayers.LayerInLayerMask(other.gameObject.layer, TargetLayerMask))
                other.GetComponent<IConveyorMovable>()?.AddMovement(-Speed * WorldSpaceDirection);
        }
        private void Update() => _material.mainTextureOffset += Time.deltaTime * Speed * Direction.normalized;
        private void OnDrawGizmosSelected() => MMDebug.DrawGizmoArrow(transform.position, WorldSpaceDirection, MMColors.ReunoYellow, .3f);
    }
}

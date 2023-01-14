using UnityEngine;

namespace TopDownEngineExtensions.GizmoHelper
{
    [RequireComponent(typeof(Collider))]
    public class GizmoHelper : MonoBehaviour
    {
        public Collider _collider;
        public Collider _confinerCollider;
        public Rigidbody _confinerRigidbody;
        public BoxCollider _boxCollider;
        public SphereCollider _sphereCollider;

        [Header("Debug")]
        /// whether or not to draw shape gizmos to help visualize the zone's bounds
        [Tooltip("whether or not to draw shape gizmos to help visualize the zone's bounds")]
        public bool DrawGizmos = true;

        /// the color of the gizmos to draw in edit mode
        [Tooltip("the color of the gizmos to draw in edit mode")]
        public Color GizmosColor;

        protected Vector3 _gizmoSize;

        void OnValidate()
        {
            _collider = GetComponent<Collider>();
            _boxCollider = gameObject.GetComponent<BoxCollider>();
            _sphereCollider = GetComponent<SphereCollider>();
        }


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!DrawGizmos)
            {
                return;
            }

            Gizmos.color = GizmosColor;

            if ((_boxCollider != null) && _boxCollider.enabled)
            {
                _gizmoSize = _boxCollider.bounds.size;
                Gizmos.DrawCube(_boxCollider.bounds.center, _gizmoSize);
            }

            if (_sphereCollider != null && _sphereCollider.enabled)
            {
                Gizmos.DrawSphere(this.transform.position + _sphereCollider.center, _sphereCollider.radius);
            }
        }
#endif
    }
}
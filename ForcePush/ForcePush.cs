using System;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions.ForcePush 
{
    /// <summary>
    /// Add this component to an object and it will push objects that collide with it. 
    /// </summary>
    [AddComponentMenu("TopDownEngineExtensions/Force Push")]
    public class ForcePush : MonoBehaviour
    {
        [Flags]
        public enum TriggerAndCollisionMask
        {
            IgnoreAll = 0,
            OnTriggerEnter = 1 << 0,
            OnTriggerStay = 1 << 1,
            OnTriggerEnter2D = 1 << 6,
            OnTriggerStay2D = 1 << 7,

            All_3D = OnTriggerEnter | OnTriggerStay,
            All_2D = OnTriggerEnter2D | OnTriggerStay2D,
            All = All_3D | All_2D
        }


        /// the possible push directions
        public enum PushDirections
        {
            BasedOnOwnerPosition,
            BasedOnSpeed,
            BasedOnDirection,
            BasedOnTransformForward,
            BasedOnScriptDirection
        }

        protected const TriggerAndCollisionMask AllowedTriggerCallbacks = TriggerAndCollisionMask.OnTriggerEnter
                                                                          | TriggerAndCollisionMask.OnTriggerStay
                                                                          | TriggerAndCollisionMask.OnTriggerEnter2D
                                                                          | TriggerAndCollisionMask.OnTriggerStay2D;

        [MMInspectorGroup("Targets", true, 3)]
        [MMInformation(
            "This component will make your object cause damage to objects that collide with it. Here you can define what layers will be affected by the damage (for a standard enemy, choose Player), how much damage to give, and how much force should be applied to the object that gets the damage on hit. You can also specify how long the post-hit invincibility should last (in seconds).",
            MMInformationAttribute.InformationType.Info, false)]
        /// the layers that will be damaged by this object
        [Tooltip("the layers that will be damaged by this object")]
        public LayerMask TargetLayerMask;

        /// the owner of the DamageOnTouch zone
        [MMReadOnly] [Tooltip("the owner of the DamageOnTouch zone")]
        public GameObject Owner;

        /// Defines on what triggers the damage should be applied, by default on enter and stay (both 2D and 3D) but this field will let you exclude triggers if needed
        [Tooltip(
            "Defines on what triggers the damage should be applied, by default on enter and stay (both 2D and 3D) but this field will let you exclude triggers if needed")]
        public TriggerAndCollisionMask TriggerFilter = AllowedTriggerCallbacks;


        [Header("Push")] [Tooltip("The direction to apply the push ")]
        public PushDirections PushDirection;

        /// The force to apply to the object that gets damaged
        [Tooltip("The force to apply to the object that gets damaged")]
        public Vector3 PushForce = new Vector3(10, 10, 0);

        // storage		
        protected Vector3 _lastPosition, _lastDamagePosition, _velocity, _pushForce, _damageDirection;
        protected float _startTime = 0f;
        protected Health _colliderHealth;
        protected TopDownController _topDownController;
        protected TopDownController _colliderTopDownController;
        protected Health _health;
        protected List<GameObject> _ignoredGameObjects;
        protected Vector3 _pushForceApplied;
        protected CircleCollider2D _circleCollider2D;
        protected BoxCollider2D _boxCollider2D;
        protected SphereCollider _sphereCollider;
        protected BoxCollider _boxCollider;
        protected Color _gizmosColor;
        protected Vector3 _gizmoSize;
        protected Vector3 _gizmoOffset;
        protected Transform _gizmoTransform;
        protected bool _twoD = false;
        protected bool _initializedFeedbacks = false;
        protected Vector3 _positionLastFrame;
        protected Vector3 _pushScriptDirection;

        #region Initialization

        /// <summary>
        /// On Awake we initialize our damage on touch area
        /// </summary>
        protected virtual void Awake()
        {
            Initialization();
        }

        /// <summary>
        /// OnEnable we set the start time to the current timestamp
        /// </summary>
        protected virtual void OnEnable()
        {
            _startTime = Time.time;
            _lastPosition = transform.position;
            _lastDamagePosition = transform.position;
        }

        /// <summary>
        /// Initializes ignore list, feedbacks, colliders and grabs components
        /// </summary>
        public virtual void Initialization()
        {
            InitializeIgnoreList();
            GrabComponents();
            InitalizeGizmos();
            InitializeColliders();
        }

        /// <summary>
        /// Stores components
        /// </summary>
        protected virtual void GrabComponents()
        {
            _health = GetComponent<Health>();
            _topDownController = GetComponent<TopDownController>();
            _boxCollider = GetComponent<BoxCollider>();
            _sphereCollider = GetComponent<SphereCollider>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _circleCollider2D = GetComponent<CircleCollider2D>();
            _lastDamagePosition = transform.position;
        }

        /// <summary>
        /// Initializes colliders, setting them as trigger if needed
        /// </summary>
        protected virtual void InitializeColliders()
        {
            _twoD = _boxCollider2D != null || _circleCollider2D != null;
            if (_boxCollider2D != null)
            {
                SetGizmoOffset(_boxCollider2D.offset);
                _boxCollider2D.isTrigger = true;
            }

            if (_boxCollider != null)
            {
                SetGizmoOffset(_boxCollider.center);
                _boxCollider.isTrigger = true;
            }

            if (_sphereCollider != null)
            {
                SetGizmoOffset(_sphereCollider.center);
                _sphereCollider.isTrigger = true;
            }

            if (_circleCollider2D != null)
            {
                SetGizmoOffset(_circleCollider2D.offset);
                _circleCollider2D.isTrigger = true;
            }
        }

        /// <summary>
        /// Initializes the _ignoredGameObjects list if needed
        /// </summary>
        protected virtual void InitializeIgnoreList()
        {
            if (_ignoredGameObjects == null) _ignoredGameObjects = new List<GameObject>();
        }


        /// <summary>
        /// On disable we clear our ignore list
        /// </summary>
        protected virtual void OnDisable()
        {
            ClearIgnoreList();
        }

        /// <summary>
        /// On validate we ensure our inspector is in sync
        /// </summary>
        protected virtual void OnValidate()
        {
            TriggerFilter &= AllowedTriggerCallbacks;
        }

        #endregion

        #region Gizmos

        /// <summary>
        /// Initializes gizmo colors & settings
        /// </summary>
        protected virtual void InitalizeGizmos()
        {
            _gizmosColor = Color.red;
            _gizmosColor.a = 0.25f;
        }


        /// <summary>
        /// A public method letting you specify a gizmo offset
        /// </summary>
        /// <param name="newOffset"></param>
        public virtual void SetGizmoOffset(Vector3 newOffset)
        {
            _gizmoOffset = newOffset;
        }

        /// <summary>
        /// draws a cube or sphere around the damage area
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = _gizmosColor;

            if (_boxCollider2D != null)
            {
                if (_boxCollider2D.enabled)
                    MMDebug.DrawGizmoCube(transform,
                        _gizmoOffset,
                        _boxCollider2D.size,
                        false);
                else
                    MMDebug.DrawGizmoCube(transform,
                        _gizmoOffset,
                        _boxCollider2D.size,
                        true);
            }

            if (_circleCollider2D != null)
            {
                if (_circleCollider2D.enabled)
                    Gizmos.DrawSphere((Vector2) transform.position + _circleCollider2D.offset,
                        _circleCollider2D.radius);
                else
                    Gizmos.DrawWireSphere((Vector2) transform.position + _circleCollider2D.offset,
                        _circleCollider2D.radius);
            }

            if (_boxCollider != null)
            {
                if (_boxCollider.enabled)
                    MMDebug.DrawGizmoCube(transform,
                        _gizmoOffset,
                        _boxCollider.size,
                        false);
                else
                    MMDebug.DrawGizmoCube(transform,
                        _gizmoOffset,
                        _boxCollider.size,
                        true);
            }

            if (_sphereCollider != null)
            {
                if (_sphereCollider.enabled)
                    Gizmos.DrawSphere(transform.position, _sphereCollider.radius);
                else
                    Gizmos.DrawWireSphere(transform.position, _sphereCollider.radius);
            }
        }

        #endregion
        
        [Tooltip("a button used to test the spawn of one text")]
        [MMInspectorButton("SetDirectionForward")]
        public bool SetDirectionForwardBtn;

        public void SetDirectionForward()
        {
            SetScriptDirection(transform.forward);
        }

        public void Start()
        {
            SetScriptDirection(transform.forward);
        }

        #region PublicAPIs

        /// <summary>
        /// When push is in script direction mode, lets you specify the direction of the push
        /// </summary>
        /// <param name="newDirection"></param>
        public virtual void SetScriptDirection(Vector3 newDirection)
        {
            _pushScriptDirection = newDirection;
        }

        /// <summary>
        /// Adds the gameobject set in parameters to the ignore list
        /// </summary>
        /// <param name="newIgnoredGameObject">New ignored game object.</param>
        public virtual void IgnoreGameObject(GameObject newIgnoredGameObject)
        {
            InitializeIgnoreList();
            _ignoredGameObjects.Add(newIgnoredGameObject);
        }

        /// <summary>
        /// Removes the object set in parameters from the ignore list
        /// </summary>
        /// <param name="ignoredGameObject">Ignored game object.</param>
        public virtual void StopIgnoringObject(GameObject ignoredGameObject)
        {
            if (_ignoredGameObjects != null) _ignoredGameObjects.Remove(ignoredGameObject);
        }

        /// <summary>
        /// Clears the ignore list.
        /// </summary>
        public virtual void ClearIgnoreList()
        {
            InitializeIgnoreList();
            _ignoredGameObjects.Clear();
        }

        #endregion

        #region Loop

        /// <summary>
        /// During last update, we store the position and velocity of the object
        /// </summary>
        protected virtual void Update()
        {
            ComputeVelocity();
        }

        /// <summary>
        /// On Late Update we store our position
        /// </summary>
        protected void LateUpdate()
        {
            _positionLastFrame = transform.position;
        }

        /// <summary>
        /// Computes the velocity based on the object's last position
        /// </summary>
        protected virtual void ComputeVelocity()
        {
            if (Time.deltaTime != 0f)
            {
                _velocity = (_lastPosition - (Vector3) transform.position) / Time.deltaTime;

                if (Vector3.Distance(_lastDamagePosition, transform.position) > 0.5f)
                {
                    _damageDirection = transform.position - _lastDamagePosition;
                    _lastDamagePosition = transform.position;
                }

                _lastPosition = transform.position;
            }
        }

        #endregion

        #region CollisionDetection

        /// <summary>
        /// When a collision with the player is triggered, we give damage to the player and knock it back
        /// </summary>
        /// <param name="collider">what's colliding with the object.</param>
        public virtual void OnTriggerStay2D(Collider2D collider)
        {
            if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerStay2D)) return;
            Colliding(collider.gameObject);
        }

        /// <summary>
        /// On trigger enter 2D, we call our colliding endpoint
        /// </summary>
        /// <param name="collider"></param>S
        public virtual void OnTriggerEnter2D(Collider2D collider)
        {
            if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerEnter2D)) return;
            Colliding(collider.gameObject);
        }

        /// <summary>
        /// On trigger stay, we call our colliding endpoint
        /// </summary>
        /// <param name="collider"></param>
        public virtual void OnTriggerStay(Collider collider)
        {
            if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerStay)) return;
            Colliding(collider.gameObject);
        }

        /// <summary>
        /// On trigger enter, we call our colliding endpoint
        /// </summary>
        /// <param name="collider"></param>
        public virtual void OnTriggerEnter(Collider collider)
        {
            if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerEnter)) return;
            Colliding(collider.gameObject);
        }

        #endregion

        /// <summary>
        /// When colliding, we apply the appropriate damage
        /// </summary>
        /// <param name="collider"></param>
        protected virtual void Colliding(GameObject collider)
        {
            if (!EvaluateAvailability(collider))
            {
                return;
            }

            // cache reset 
            _colliderTopDownController = null;
            _colliderHealth = collider.gameObject.MMGetComponentNoAlloc<Health>();

            if (_colliderHealth != null)
            {
                if (_colliderHealth.CurrentHealth > 0)
                {
                    OnCollideWithDamageable(_colliderHealth);
                }
            }
        }

        protected virtual void OnCollideWithDamageable(Health health)
        {
            if (!health.CanTakeDamageThisFrame())
            {
                return;
            }

            // if what we're colliding with is a TopDownController, we apply a push force
            _colliderTopDownController = health.gameObject.MMGetComponentNoAlloc<TopDownController>();

            ApplyPush();
        }

        /// <summary>
        /// Checks whether or not damage should be applied this frame
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        protected virtual bool EvaluateAvailability(GameObject collider)
        {
            // if we're inactive, we do nothing
            if (!isActiveAndEnabled)
            {
                return false;
            }

            // if the object we're colliding with is part of our ignore list, we do nothing and exit
            if (_ignoredGameObjects.Contains(collider))
            {
                return false;
            }

            // if what we're colliding with isn't part of the target layers, we do nothing and exit
            if (!MMLayers.LayerInLayerMask(collider.layer, TargetLayerMask))
            {
                return false;
            }

            // if we're on our first frame, we don't apply damage
            if (Time.time == 0f)
            {
                return false;
            }

            return true;
        }

        #region Push

        /// <summary>
        /// Applies push if needed
        /// </summary>
        protected virtual void ApplyPush()
        {
            if (ShouldApplyPush(0, null))
            {
                _pushForce = PushForce * _colliderHealth.KnockbackForceMultiplier;

                if (_twoD) // if we're in 2D
                {
                    ApplyPush2D();
                }
                else // if we're in 3D
                {
                    ApplyPush3D();
                }

                _colliderTopDownController.Impact(_pushForce.normalized, _pushForce.magnitude);
            }
        }

        /// <summary>
        /// Determines whether or not push should be applied
        /// </summary>
        /// <returns></returns>
        protected virtual bool ShouldApplyPush(float damage, List<TypedDamage> typedDamages)
        {
            return (_colliderTopDownController != null)
                   && (PushForce != Vector3.zero)
                   && !_colliderHealth.Invulnerable
                   && !_colliderHealth.ImmuneToKnockback;
        }

        /// <summary>
        /// Applies push if we're in a 2D context
        /// </summary>
        protected virtual void ApplyPush2D()
        {
            switch (PushDirection)
            {
                case PushDirections.BasedOnSpeed:
                    var totalVelocity = _colliderTopDownController.Speed + _velocity;
                    _pushForce = Vector3.RotateTowards(_pushForce,
                        totalVelocity.normalized, 10f, 0f);
                    break;
                case PushDirections.BasedOnOwnerPosition:
                    if (Owner == null)
                    {
                        Owner = gameObject;
                    }

                    var relativePosition = _colliderTopDownController.transform.position - Owner.transform.position;
                    _pushForce = Vector3.RotateTowards(_pushForce, relativePosition.normalized, 10f, 0f);
                    break;
                case PushDirections.BasedOnDirection:
                    var direction = transform.position - _positionLastFrame;
                    _pushForce = direction * _pushForce.magnitude;
                    break;
                case PushDirections.BasedOnTransformForward:
                    _pushForce = transform.forward * _pushForce.magnitude;
                    break;
                case PushDirections.BasedOnScriptDirection:
                    _pushForce = _pushScriptDirection * _pushForce.magnitude;
                    break;
            }
        }

        /// <summary>
        /// Applies push if we're in a 3D context
        /// </summary>
        protected virtual void ApplyPush3D()
        {
            switch (PushDirection)
            {
                case PushDirections.BasedOnSpeed:
                    var totalVelocity = _colliderTopDownController.Speed + _velocity;
                    _pushForce = _pushForce * totalVelocity.magnitude;
                    break;
                case PushDirections.BasedOnOwnerPosition:
                    if (Owner == null)
                    {
                        Owner = gameObject;
                    }

                    var relativePosition = _colliderTopDownController.transform.position - Owner.transform.position;
                    _pushForce.x = relativePosition.normalized.x * _pushForce.x;
                    _pushForce.z = relativePosition.normalized.z * _pushForce.z;
                    break;
                case PushDirections.BasedOnTransformForward:
                    _pushForce = transform.forward * _pushForce.magnitude;
                    break;
                case PushDirections.BasedOnDirection:
                    var direction = transform.position - _positionLastFrame;
                    _pushForce = direction * _pushForce.magnitude;
                    break;
                case PushDirections.BasedOnScriptDirection:
                    _pushForce = _pushScriptDirection * _pushForce.magnitude;
                    break;
            }
        }

        #endregion
    }
}
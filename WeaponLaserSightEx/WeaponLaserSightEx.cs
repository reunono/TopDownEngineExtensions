using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;
using MoreMountains.Feedbacks;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// An extended version of the base WeaponLaserSight to create a widening laser when player turns
    /// </summary>
    [AddComponentMenu("TopDown Engine/Weapons/Weapon Laser Sight Ex")]
    public class WeaponLaserSightEx : WeaponLaserSight, MMEventListener<MMStateChangeEvent<CharacterStates.MovementStates>>
    {
        [Tooltip("Specify which momement state you want the laser sight to be turned off")]
        public CharacterStates.MovementStates[] BlockMovementStates;
        [Tooltip("How long before the the laser is being turned back on")]
        public float TurnOffDuration = 0.7f;
        [Tooltip("Multiplies the player's turning speed for more exaggerated effect.")]
        public float RotationMultiplier = 2f;
        [Tooltip("The particle to draw the laser sight dot")]
        public ParticleSystem LaserSightDot;


        protected Quaternion _currentRotation;
        protected Quaternion _prevRotation;
        protected float _rotationDifference;
        protected bool _isTurnedOff = false;

        protected override void Initialization()
        {
            if (DrawLaser)
            {
                _line = gameObject.AddComponent<LineRenderer>();
                _line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                _line.receiveShadows = true;
                _line.startWidth = LaserWidth.x;
                _line.endWidth = LaserWidth.y;
                _line.material = LaserMaterial;
                _line.SetPosition(0, this.transform.position);
                _line.SetPosition(1, this.transform.position);
            }
            _weapon = GetComponent<Weapon>();

            if (_weapon == null)
            {
                _weapon = GetComponentInParent<Weapon>();

            }

            _initFrame = Time.frameCount;

        }

        protected override void Update()
        {
            base.Update();

            UpdateLaserWidth();
        }

        /// <summary>
        /// Draws the actual laser
        /// </summary>
        public override void ShootLaser()
        {
            if (!PerformRaycast)
            {
                return;
            }

            _laserOffset = LaserOriginOffset;
            _weaponPosition = _weapon.transform.position;
            _weaponRotation = _weapon.transform.rotation;
            _thisPosition = this.transform.position;
            _thisRotation = this.transform.rotation;
            _thisForward = this.transform.forward;

            if (Mode == Modes.ThreeD)
            {
                // our laser will be shot from the weapon's laser origin
                _origin = MMMaths.RotatePointAroundPivot(_thisPosition + _laserOffset, _thisPosition, _thisRotation);
                _raycastOrigin = MMMaths.RotatePointAroundPivot(_thisPosition + RaycastOriginOffset, _thisPosition, _thisRotation);

                // we cast a ray in front of the weapon to detect an obstacle
                _hit = MMDebug.Raycast3D(_raycastOrigin, _thisForward, LaserMaxDistance, LaserCollisionMask, Color.red, true);

                // if we've hit something, our destination is the raycast hit
                if (_hit.transform != null)
                {
                    _destination = _hit.point;

                    if(!_isTurnedOff)
                        LaserSightDot?.gameObject.SetActive(true);

                    if(LaserSightDot != null)
                        LaserSightDot.transform.position = _destination;
                }
                // otherwise we just draw our laser in front of our weapon 
                else
                {
                    _destination = _origin + _thisForward * LaserMaxDistance;

                    LaserSightDot?.gameObject.SetActive(false);

                }
            }
            else
            {
                if (_direction == Vector3.left)
                {
                    _laserOffset.x = -LaserOriginOffset.x;
                }

                _raycastOrigin = MMMaths.RotatePointAroundPivot(_weaponPosition + _laserOffset, _weaponPosition, _weaponRotation);
                _origin = _raycastOrigin;
                _direction = _weapon.Flipped ? Vector3.left : Vector3.right;

                // we cast a ray in front of the weapon to detect an obstacle
                _hit2D = MMDebug.RayCast(_raycastOrigin, _weaponRotation * _direction, LaserMaxDistance, LaserCollisionMask, Color.red, true);
                if (_hit2D)
                {
                    _destination = _hit2D.point;
                }
                // otherwise we just draw our laser in front of our weapon 
                else
                {
                    _destination = _origin;
                    _destination.x = _destination.x + LaserMaxDistance * _direction.x;
                    _destination = MMMaths.RotatePointAroundPivot(_destination, _weaponPosition, _weaponRotation);
                }
            }

            if (Time.frameCount <= _initFrame + 1)
            {
                return;
            }

            // we set our laser's line's start and end coordinates
            if (DrawLaser)
            {
                _line.SetPosition(0, _origin);
                _line.SetPosition(1, _destination);
            }
        }

        private void UpdateLaserWidth()
        {
            if (!DrawLaser) return;

            _currentRotation = transform.rotation;
            _rotationDifference = Quaternion.Angle(_currentRotation, _prevRotation);
            _rotationDifference *= RotationMultiplier;
            _rotationDifference = Mathf.Clamp(_rotationDifference, 1f, 100f);
            _prevRotation = _currentRotation;

            _line.endWidth = _rotationDifference * LaserWidth.y;
        }


        public void OnMMEvent(MMStateChangeEvent<CharacterStates.MovementStates> movementStateChange)
        {
            if (this._weapon == null)
                return;

            if (movementStateChange.Target != this._weapon.Owner.gameObject)
            {
                return;
            }

            for (int i = 0; i < BlockMovementStates.Length; i++)
            {
                if (movementStateChange.NewState == BlockMovementStates[i])
                {
                    StartCoroutine(TurnOffLaser(TurnOffDuration));
                    
                }
            }
            
        }

        IEnumerator TurnOffLaser(float duration = 1f)
        {

            if (_isTurnedOff)
                yield break;

            _isTurnedOff = true;

            _line.enabled = false;
            LaserSightDot.gameObject.SetActive(false);

            float timeLapse = duration;

            while(timeLapse > 0)
            {
                timeLapse -= Time.deltaTime;
                yield return null;
            }

            _isTurnedOff = false;

            _line.enabled = true;
            LaserSightDot.gameObject.SetActive(true);

        }

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

    }
}

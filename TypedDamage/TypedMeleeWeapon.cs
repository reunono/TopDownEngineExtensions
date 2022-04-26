using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TypedDamage
{
    public class TypedMeleeWeapon : MeleeWeapon
    {
        [MMInspectorGroup("Damage type", true, 25)]
        public DamageType DamageType;

        private TypedDamageOnTouch _typedDamageOnTouch;
        protected override void CreateDamageArea()
        {
            _damageArea = new GameObject();
            _damageArea.name = name + "DamageArea";
            _damageArea.transform.position = transform.position;
            _damageArea.transform.rotation = transform.rotation;
            _damageArea.transform.SetParent(transform);
            _damageArea.layer = gameObject.layer;
            
            if (DamageAreaShape == MeleeDamageAreaShapes.Rectangle)
            {
                _boxCollider2D = _damageArea.AddComponent<BoxCollider2D>();
                _boxCollider2D.offset = AreaOffset;
                _boxCollider2D.size = AreaSize;
                _damageAreaCollider2D = _boxCollider2D;
                _damageAreaCollider2D.isTrigger = true;
            }
            if (DamageAreaShape == MeleeDamageAreaShapes.Circle)
            {
                _circleCollider2D = _damageArea.AddComponent<CircleCollider2D>();
                _circleCollider2D.transform.position = transform.position + transform.rotation * AreaOffset;
                _circleCollider2D.radius = AreaSize.x / 2;
                _damageAreaCollider2D = _circleCollider2D;
                _damageAreaCollider2D.isTrigger = true;
            }

            if ((DamageAreaShape == MeleeDamageAreaShapes.Rectangle) || (DamageAreaShape == MeleeDamageAreaShapes.Circle))
            {
                Rigidbody2D rigidBody = _damageArea.AddComponent<Rigidbody2D>();
                rigidBody.isKinematic = true;
                rigidBody.sleepMode = RigidbodySleepMode2D.NeverSleep;
            }            

            if (DamageAreaShape == MeleeDamageAreaShapes.Box)
            {
                _boxCollider = _damageArea.AddComponent<BoxCollider>();
                _boxCollider.center = AreaOffset;
                _boxCollider.size = AreaSize;
                _damageAreaCollider = _boxCollider;
                _damageAreaCollider.isTrigger = true;
            }
            if (DamageAreaShape == MeleeDamageAreaShapes.Sphere)
            {
                _sphereCollider = _damageArea.AddComponent<SphereCollider>();
                _sphereCollider.transform.position = transform.position + transform.rotation * AreaOffset;
                _sphereCollider.radius = AreaSize.x / 2;
                _damageAreaCollider = _sphereCollider;
                _damageAreaCollider.isTrigger = true;
            }

            if ((DamageAreaShape == MeleeDamageAreaShapes.Box) || (DamageAreaShape == MeleeDamageAreaShapes.Sphere))
            {
                Rigidbody rigidBody = _damageArea.AddComponent<Rigidbody>();
                rigidBody.isKinematic = true;
            }

            _typedDamageOnTouch = _damageArea.AddComponent<TypedDamageOnTouch>();
            _typedDamageOnTouch.DamageType = DamageType;
            _damageOnTouch = _typedDamageOnTouch;
            _damageOnTouch.SetGizmoSize(AreaSize);
            _damageOnTouch.SetGizmoOffset(AreaOffset);
            _damageOnTouch.TargetLayerMask = TargetLayerMask;
            _damageOnTouch.MinDamageCaused = MinDamageCaused;
            _damageOnTouch.MaxDamageCaused = MaxDamageCaused;
            _damageOnTouch.DamageCausedKnockbackType = Knockback;
            _damageOnTouch.DamageCausedKnockbackForce = KnockbackForce;
            _damageOnTouch.InvincibilityDuration = InvincibilityDuration;
            _damageOnTouch.HitDamageableFeedback = HitDamageableFeedback;
            _damageOnTouch.HitNonDamageableFeedback = HitNonDamageableFeedback;
            
            if (!CanDamageOwner && (Owner != null))
            {
                _damageOnTouch.IgnoreGameObject(Owner.gameObject);    
            }
        }

        private void OnValidate()
        {
            if (_typedDamageOnTouch == null) return;
            _typedDamageOnTouch.DamageType = DamageType;
        }
    }
}

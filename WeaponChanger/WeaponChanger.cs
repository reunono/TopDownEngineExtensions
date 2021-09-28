using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions
{
    public class WeaponChanger : MonoBehaviour
    {
        public List<Weapon> Weapons;
        private CharacterHandleWeapon _characterHandleWeapon;
        private int _lastEquippedWeapon = -1;
        public bool AutoEquipFirstWeapon = true;

        private void Awake()
        {
            _characterHandleWeapon = GetComponentInParent<CharacterHandleWeapon>();
            if (AutoEquipFirstWeapon) StartCoroutine(EquipFirstWeapon());

            IEnumerator EquipFirstWeapon()
            {
                yield return new WaitUntil(() => _characterHandleWeapon.AbilityInitialized && Weapons.Count > 0);
                ChangeWeapon(0);
            }
        }

        private int ScrollWheelIndex()
        {
            var scrollWheelIndex = _lastEquippedWeapon;
            if (Input.mouseScrollDelta.y < 0) scrollWheelIndex--;
            else scrollWheelIndex++;

            if (scrollWheelIndex < 0) return Weapons.Count - 1;
            return scrollWheelIndex >= Weapons.Count ? 0 : scrollWheelIndex;
        }
    
        private void Update()
        {
            if (Input.mouseScrollDelta != Vector2.zero) ChangeWeapon(ScrollWheelIndex());
            if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeWeapon(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeWeapon(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeWeapon(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeWeapon(3);
            if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeWeapon(4);
            if (Input.GetKeyDown(KeyCode.Alpha6)) ChangeWeapon(5);
            if (Input.GetKeyDown(KeyCode.Alpha7)) ChangeWeapon(6);
            if (Input.GetKeyDown(KeyCode.Alpha8)) ChangeWeapon(7);
            if (Input.GetKeyDown(KeyCode.Alpha9)) ChangeWeapon(8);
            if (Input.GetKeyDown(KeyCode.Alpha0)) ChangeWeapon(9);
        }

        private void ChangeWeapon(int index)
        {
            if (index < 0 || index >= Weapons.Count || index == _lastEquippedWeapon) return;
            _characterHandleWeapon.ChangeWeapon(Weapons[index], Weapons[index].WeaponName);
            _lastEquippedWeapon = index;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions
{
    public class CharacterInventoryWeaponChanger : MonoBehaviour, MMEventListener<MMInventoryEvent>
    {
        private readonly List<string> _weapons = new List<string>();
        private CharacterInventory _characterInventory;
        private CharacterHandleWeapon _characterHandleWeapon;
        private int _lastEquippedWeapon = -1;
        public bool AutoEquipFirstWeapon = true;
        public AudioClip WeaponChangeSfx;
        public MMFeedbacks WeaponChangeFeedbacks;

        private void Awake()
        {
            var character = GetComponentInParent<Character>();
            _characterInventory = character.FindAbility<CharacterInventory>();
            _characterHandleWeapon = character.FindAbility<CharacterHandleWeapon>();
            if (AutoEquipFirstWeapon) StartCoroutine(EquipFirstWeapon());

            IEnumerator EquipFirstWeapon()
            {
                yield return new WaitUntil(() => _characterHandleWeapon.AbilityInitialized && _weapons.Count > 0);
                ChangeWeapon(0);
            }
        }

        private int ScrollWheelIndex()
        {
            var scrollWheelIndex = _lastEquippedWeapon;
            if (Input.mouseScrollDelta.y < 0) scrollWheelIndex--;
            else scrollWheelIndex++;

            if (scrollWheelIndex < 0) return _weapons.Count - 1;
            return scrollWheelIndex >= _weapons.Count ? 0 : scrollWheelIndex;
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
            if (index < 0 || index >= _weapons.Count || index == _lastEquippedWeapon) return;
            EquipWeapon(_weapons[index]);
            _lastEquippedWeapon = index;
            PlayWeaponChangeSfx();
            WeaponChangeFeedbacks?.PlayFeedbacks(transform.position);
        }

        public void OnMMEvent(MMInventoryEvent inventoryEvent)
        {
            switch (inventoryEvent.InventoryEventType)
            {
                case MMInventoryEventType.Pick when inventoryEvent.EventItem.GetType() == typeof(InventoryWeapon):
                    _weapons.Add(inventoryEvent.EventItem.ItemID);
                    break;
                case MMInventoryEventType.Drop when inventoryEvent.EventItem.GetType() == typeof(InventoryWeapon):
                    _weapons.Remove(inventoryEvent.EventItem.ItemID);
                    break;
            }
        }
        
        private void EquipWeapon(string weaponID)
        {
            for (var i = 0; i < _characterInventory.MainInventory.Content.Length ; i++)
            {
                if (InventoryItem.IsNull(_characterInventory.MainInventory.Content[i]) || _characterInventory.MainInventory.Content[i].ItemID != weaponID) continue;
                MMInventoryEvent.Trigger(MMInventoryEventType.EquipRequest, null, _characterInventory.MainInventory.name, _characterInventory.MainInventory.Content[i], 0, i);
                break;
            }
        }

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }
        
        private void PlayWeaponChangeSfx()
        {
            if (WeaponChangeSfx == null) return;
            MMSoundManagerSoundPlayEvent.Trigger(WeaponChangeSfx, MMSoundManager.MMSoundManagerTracks.Sfx, this.transform.position);
        }
    }
}

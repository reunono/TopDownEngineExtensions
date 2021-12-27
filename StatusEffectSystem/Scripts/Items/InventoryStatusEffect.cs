using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace StatusSystem
{
    [CreateAssetMenu(fileName = "InventoryStatusEffect", menuName = "Status System/Items/Inventory Status Effect")]
    public class InventoryStatusEffect : InventoryItem
    {
        public StatusEffect[] StatusEffects;

        public override bool Use(string playerID)
        {
            var character = TargetInventory(playerID).Owner.GetComponentInParent<Character>();
            foreach (var statusEffect in StatusEffects)
                StatusEffectEvent.Trigger(statusEffect, character, StatusEffectEventTypes.Apply);
            return true;
        }

        public override bool Equip(string playerID)
        {
            var character = TargetInventory(playerID).Owner.GetComponentInParent<Character>();
            foreach (var statusEffect in StatusEffects)
                StatusEffectEvent.Trigger(statusEffect, character, StatusEffectEventTypes.Apply);
            return true;
        }

        public override bool UnEquip(string playerID)
        {
            var character = TargetInventory(playerID).Owner.GetComponentInParent<Character>();
            foreach (var statusEffect in StatusEffects)
                StatusEffectEvent.Trigger(statusEffect, character, StatusEffectEventTypes.Unapply);
            return true;
        }
    }
}

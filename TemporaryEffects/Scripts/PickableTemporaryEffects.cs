using System;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TemporaryEffects
{
    [Serializable]
    public struct TemporaryMultiplier
    {
        public Multipliers Multipliers;
        public float Multiplier;
        public float DurationInSeconds;
    }

    [Serializable]
    public struct TemporarySpread
    {
        public ProjectileWeaponSpreadEvents ProjectileWeaponSpreadEvents;
        public Vector3 Spread;
        public float DurationInSeconds;
    }
    
    public class PickableTemporaryEffects : PickableItem
    {
        public TemporaryMultiplier[] TemporaryMultiplierList;
        public TemporarySpread[] TemporarySpreadList;

        protected override void Pick(GameObject picker)
        {
            base.Pick(picker);
            foreach (var temporaryMultiplier in TemporaryMultiplierList)
                temporaryMultiplier.Multipliers.AddTemporaryMultiplier(temporaryMultiplier.Multiplier, temporaryMultiplier.DurationInSeconds);
            foreach (var temporarySpread in TemporarySpreadList)
                temporarySpread.ProjectileWeaponSpreadEvents.SetTemporarySpread(temporarySpread.Spread, temporarySpread.DurationInSeconds);
        }
    }
}

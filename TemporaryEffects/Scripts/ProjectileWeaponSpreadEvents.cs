using System;
using System.Threading.Tasks;
using UnityEngine;

namespace TemporaryEffects
{
    [CreateAssetMenu]
    public class ProjectileWeaponSpreadEvents : ScriptableObject
    {
        public event Action<Vector3> OnSetSpread;
        public event Action OnResetSpread;

        public void SetSpread(Vector3Variable spread) => SetSpread(spread.Value);
        public void SetSpread(Vector3 spread) => OnSetSpread?.Invoke(spread);
        public void ResetSpread() => OnResetSpread?.Invoke();
        
        public async void SetTemporarySpread(Vector3 spread, float durationInSeconds)
        {
            SetSpread(spread);
            await Task.Delay((int)(durationInSeconds * 1000));
            ResetSpread();
        }
    }
}

using System.Linq;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions
{
    /// <summary>
    /// Add this class to an object and it will count as a target to clear (if it is inside a collider with a TriggerEventOnAreaClear component)
    /// </summary>
    public class AreaClearTarget : MonoBehaviour
    {
        private Health _health;
        private TriggerEventOnAreaClear _triggerEventOnAreaClear;
        private bool _alreadyRemoved;

        private void Awake()
        {
            _health = GetComponent<Health>();
            if (_health != null) _health.OnDeath += RemoveEnemy;
        }

        private void AddEnemy()
        {
            if (_triggerEventOnAreaClear == null) return;
            _triggerEventOnAreaClear.AddEnemy();
            _alreadyRemoved = false;
        }
        
        private void RemoveEnemy()
        {
            if (_triggerEventOnAreaClear == null || _alreadyRemoved) return;
            _triggerEventOnAreaClear.RemoveEnemy();
            _alreadyRemoved = true;
        }
        
        private void OnEnable()
        {
            _triggerEventOnAreaClear = FindObjectsOfType<TriggerEventOnAreaClear>().SingleOrDefault(room => room.GetComponent<Collider2D>().bounds.Contains(transform.position));
            AddEnemy();
        }

        private void OnDisable() { RemoveEnemy(); }
    }
}

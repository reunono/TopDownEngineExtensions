using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Events;

namespace ProgressionSystem.Scripts.Events
{
    public class OnDeath : MonoBehaviour
    {
        [SerializeField] private UnityEvent OnDeathEvent;
        private Health _health;
        private void Awake() { _health = GetComponent<Health>(); }
        private void OnEnable() { _health.OnDeath += OnDeathEvent.Invoke; }
        private void OnDisable() { _health.OnDeath -= OnDeathEvent.Invoke; }
    }
}

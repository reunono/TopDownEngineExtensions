using UnityEngine;
using UnityEngine.Events;

namespace TopDownEngineExtensions
{
    /// <summary>
    /// Requires a Collider and objects with the AreaClearTarget component inside said collider
    /// </summary>
    public class TriggerEventOnAreaClear : MonoBehaviour
    {
        [Tooltip("the event to trigger when the area is cleared (all objects marked with AreaClearTarget have been destroyed)")]
        public UnityEvent OnAreaCleared;
        private int _numberOfTargetsAlive;

        public void AddEnemy() { _numberOfTargetsAlive++; }

        public void RemoveEnemy()
        {
            _numberOfTargetsAlive--;
            if (_numberOfTargetsAlive == 0) OnAreaCleared?.Invoke();
        }
    }
}

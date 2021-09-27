using UnityEngine;
using UnityEngine.Events;

namespace TopDownEngineExtensions
{
    /// <summary>
    /// Requires a Collider and objects with the AreaClearTarget component inside said collider
    /// </summary>
    public class TriggerEventOnAreaClear : MonoBehaviour
    {
        /// the event to trigger when the area is cleared (all objects marked with AreaClearTarget have been destroyed)
        [Tooltip("the event to trigger when the room is cleared (all enemies are dead)")]
        public UnityEvent OnRoomCleared;
        private int _numberOfEnemiesAlive;

        public void AddEnemy() { _numberOfEnemiesAlive++; }

        public void RemoveEnemy()
        {
            _numberOfEnemiesAlive--;
            if (_numberOfEnemiesAlive == 0) OnRoomCleared?.Invoke();
        }
    }
}

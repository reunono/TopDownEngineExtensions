using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Stamina
{
    public class StaminaBarUpdater : MonoBehaviour, MMEventListener<StaminaUpdateEvent>, MMEventListener<TopDownEngineEvent>
    {
        [SerializeField]
        [Tooltip("if this is false, the player character will be set as target automatically")]
        private bool UseCustomTarget;
        [MMCondition(nameof(UseCustomTarget), true)]
        [SerializeField]
        private GameObject Target;

        private MMProgressBar _bar;

        private void Awake()
        {
            _bar = GetComponent<MMProgressBar>();
            this.MMEventStartListening<TopDownEngineEvent>();
        }

        public void OnMMEvent(StaminaUpdateEvent staminaUpdateEvent)
        {
            if (staminaUpdateEvent.Target != Target) return;
            _bar.UpdateBar(staminaUpdateEvent.Stamina, 0, staminaUpdateEvent.MaxStamina);
        }

        public void OnMMEvent(TopDownEngineEvent topDownEngineEvent)
        {
            if (topDownEngineEvent.EventType == TopDownEngineEventTypes.SpawnCharacterStarts && !UseCustomTarget) Target = LevelManager.Instance.Players[0].gameObject;
        }
        
        private void OnEnable()
        {
            this.MMEventStartListening<StaminaUpdateEvent>();
        }
    
        private void OnDisable()
        {
            this.MMEventStopListening<StaminaUpdateEvent>();
        }
    }
}

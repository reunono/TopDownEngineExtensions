using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace ProgressionSystem
{
    public class LevelEventTester : MonoBehaviour, MMEventListener<TopDownEngineEvent>
    {
        public LevelEventType Type;
        public GameObject Target;
        public int Level = 2;
        public int MaxLevel = 2;
        [MMInspectorButton(nameof(TriggerEvent))] 
        public bool TriggerEventButton;

        private void TriggerEvent()
        {
            LevelEvent.Trigger(Type, Target, Level, MaxLevel);
        }

        public void OnMMEvent(TopDownEngineEvent topDownEngineEvent)
        {
            if (topDownEngineEvent.EventType == TopDownEngineEventTypes.SpawnCharacterStarts && Target == null) Target = LevelManager.Instance.Players[0].gameObject;
        }

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }
    }
}

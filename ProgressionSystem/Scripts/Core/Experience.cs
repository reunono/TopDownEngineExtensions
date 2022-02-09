using System;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace ProgressionSystem
{
    [Serializable]
    public enum ExperienceEventType
    {
        Add
    }
    public struct ExperienceEvent
    {
        public ExperienceEventType Type;
        public GameObject Target;
        public int Experience;
        private static ExperienceEvent e;
        public static void Trigger(ExperienceEventType type, GameObject target, int experience)
        {
            e.Type = type;
            e.Target = target;
            e.Experience = experience;
            MMEventManager.TriggerEvent(e);
        }
    }

    [Serializable]
    public enum LevelEventType
    {
        LevelUp
    }
    public struct LevelEvent
    {
        public LevelEventType Type;
        public GameObject Target;
        public int Level;
        public int MaxLevel;
        private static LevelEvent e;
        public static void Trigger(LevelEventType type, GameObject target, int level, int maxLevel)
        {
            e.Type = type;
            e.Target = target;
            e.Level = level;
            e.MaxLevel = maxLevel;
            MMEventManager.TriggerEvent(e);
        }
    }
    
    [AddComponentMenu("Custom/Character/Core/Experience")]
    public class Experience : MonoBehaviour, MMEventListener<ExperienceEvent>, MMEventListener<LevelEvent>
    {
        [SerializeField]
        private int CurrentExperience;
        [SerializeField]
        private AnimationCurve ExperienceLevelCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
        [SerializeField]
        private int MaxLevel = 20;
        [SerializeField]
        private int MaxExperience = 10000;
        [SerializeField]
        private MMFeedbacks LevelUpFeedbacks;
        private int Level { get; set; }

        private void OnValidate() { UpdateLevel(); }
        private void Start() { UpdateLevel(); }
        
        public void OnMMEvent(ExperienceEvent experienceEvent)
        {
            if (experienceEvent.Target != gameObject) return;
            switch (experienceEvent.Type)
            {
                case ExperienceEventType.Add:
                    CurrentExperience = Math.Min(CurrentExperience + experienceEvent.Experience, MaxExperience);
                    UpdateLevel();
                    break;
            }
        }
        
        public void OnMMEvent(LevelEvent levelEvent)
        {
            if (levelEvent.Target != gameObject) return;
            switch (levelEvent.Type)
            {
                case LevelEventType.LevelUp:
                    LevelUpFeedbacks?.PlayFeedbacks(transform.position, levelEvent.Level);
                    break;
            }
        }
        
        private void UpdateLevel()
        {
            var oldLevel = Level;
            Level = (int)(MaxLevel*ExperienceLevelCurve.Evaluate((float)CurrentExperience/MaxExperience));
            if (Level > oldLevel) LevelEvent.Trigger(LevelEventType.LevelUp, gameObject, Level, MaxLevel);
        }
        
        private void OnEnable()
        {
            this.MMEventStartListening<ExperienceEvent>();
            this.MMEventStartListening<LevelEvent>();
        }
        
        private void OnDisable()
        {
            this.MMEventStopListening<ExperienceEvent>();
            this.MMEventStopListening<LevelEvent>();
        }
    }
}
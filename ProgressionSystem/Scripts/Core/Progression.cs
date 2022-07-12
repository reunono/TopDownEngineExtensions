using System;
using ProgressionSystem.Scripts.Variables;
using UnityEngine;

namespace ProgressionSystem.Scripts.Core
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Progression/Progression")]
    public class Progression : ScriptableObject
    {
        [SerializeField] private IntVariable Experience;
        [SerializeField] private IntVariable Level;
        [SerializeField] private LevelValueCurveVariable LevelExperienceCurve;
        [SerializeField] private bool ResetExperienceOnEnable = true;
        public Action Progressed;
        public int LevelExperience => Experience.Value - LevelExperienceCurve.EvaluateInt(Level.Value);
        public int NextLevelExperience => LevelExperienceCurve.EvaluateInt(Level.Value + 1) - LevelExperienceCurve.EvaluateInt(Level.Value);

        private void UpdateLevel()
        {
            while (Experience.Value >= LevelExperienceCurve.EvaluateInt(Level.Value + 1) && Level.Value < LevelExperienceCurve.MaxLevel) Level.Value++;
            Progressed?.Invoke();
        }

        private void OnEnable()
        {
            if (ResetExperienceOnEnable) Experience.Value = LevelExperienceCurve.EvaluateInt(LevelExperienceCurve.MinLevel);
            Level.Value = 0;
            UpdateLevel();

            Experience.Changed += UpdateLevel;
            LevelExperienceCurve.Changed += UpdateLevel;
        }

        private void OnDisable()
        {
            Experience.Changed -= UpdateLevel;
            LevelExperienceCurve.Changed -= UpdateLevel;
        }
    }
}

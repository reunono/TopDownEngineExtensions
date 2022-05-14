using ProgressionSystem.Scripts.Variables;
using UnityEngine;

namespace ProgressionSystem.Scripts.Updaters
{
    public abstract class LevelBasedVariableUpdater : ScriptableObject
    {
        [SerializeField] protected IntVariable Level;
        [SerializeField] protected LevelValueCurveVariable LevelValueCurve;

        protected abstract void UpdateVariable();

        private void OnEnable()
        {
            UpdateVariable();
            Level.Changed += UpdateVariable;
        }
        private void OnDisable() { Level.Changed -= UpdateVariable; }
    }
}

using ProgressionSystem.Scripts.Variables;
using UnityEngine;

namespace ProgressionSystem.Scripts.Updaters
{
    [CreateAssetMenu(fileName = "LevelFloatVariableUpdater", menuName = "Progression/Updaters/LevelFloatVariableUpdater")]
    public class LevelBasedFloatVariableUpdater : LevelBasedVariableUpdater
    {
        [SerializeField] protected FloatVariable Variable;
        protected override void UpdateVariable() { Variable.Value = LevelValueCurve.Evaluate(Level.Value); }
    }
}

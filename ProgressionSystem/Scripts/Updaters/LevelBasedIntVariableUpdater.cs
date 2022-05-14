using ProgressionSystem.Scripts.Variables;
using UnityEngine;

namespace ProgressionSystem.Scripts.Updaters
{
    [CreateAssetMenu(fileName = "LevelIntVariableUpdater", menuName = "Progression/Updaters/LevelIntVariableUpdater")]
    public class LevelBasedIntVariableUpdater : LevelBasedVariableUpdater
    {
        [SerializeField] protected IntVariable Variable;
        protected override void UpdateVariable() { Variable.Value = LevelValueCurve.EvaluateInt(Level.Value); }
    }
}

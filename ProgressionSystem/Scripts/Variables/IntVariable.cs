using UnityEngine;

namespace ProgressionSystem.Scripts.Variables
{
    [CreateAssetMenu(fileName = "IntVariable", menuName = "Progression/Variables/IntVariable")]
    public class IntVariable : NumericVariable<int>
    {
        public void Add(int value) { Value += value; }
        protected override bool NotEquals(int value1, int value2) { return value1 != value2; }
    }
}

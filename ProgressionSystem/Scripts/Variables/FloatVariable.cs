using System;
using UnityEngine;

namespace ProgressionSystem.Scripts.Variables
{
    [CreateAssetMenu(fileName = "FloatVariable", menuName = "Progression/Variables/FloatVariable")]
    public class FloatVariable : NumericVariable<float>
    {
        public void Add(float value) { Value += value; }
        protected override bool NotEquals(float value1, float value2) { return Math.Abs(value1 - value2) > 0.0001; }
    }
}
using System;
using UnityEngine;

namespace ProgressionSystem.Scripts.Variables
{
    public abstract class NumericVariable<T> : ScriptableObject
    {
        [SerializeField] private T CurrentValue;
        public Action Changed;
        public T Value
        {
            get => CurrentValue;
            set
            {
                var oldValue = CurrentValue;
                CurrentValue = value;
                if (NotEquals(oldValue, CurrentValue)) OnChange();
            }
        }

        private void OnValidate() { OnChange(); }
        private void OnChange() { Changed?.Invoke(); }
        protected abstract bool NotEquals(T value1, T value2);
    }
}

using System;
using UnityEngine;

namespace SpeedMultipliers.Scripts
{
    [CreateAssetMenu]
    public class FloatVariable : ScriptableObject
    {
        public float InitialValue;
        public Action<float> ValueChanged;
        [SerializeField]
        private float CurrentValue;

        public float Value
        {
            get => CurrentValue;
            set
            {
                CurrentValue = value;
                OnValueChange();
            }
        }

        private void OnEnable()
        {
            CurrentValue = InitialValue;
        }

        private void OnValidate()
        {
            OnValueChange();
        }

        private void OnValueChange()
        {
            ValueChanged?.Invoke(CurrentValue);
        }
    }
}

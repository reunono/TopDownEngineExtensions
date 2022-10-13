using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoreMountains.Tools;
using UnityEngine;
using System.Linq;

namespace TemporaryEffects
{
    [CreateAssetMenu(fileName = "Multipliers", menuName = "Multipliers")]
    public class Multipliers : ScriptableObject
    {
        [SerializeField] private List<float> List;
        [MMReadOnly] public float TotalMultiplier = 1;
        public event Action OnChange;

        private float Multiplier
        {
            set
            {
                TotalMultiplier = value;
                OnChange?.Invoke();
            }
        }
        public void OnEnable()
        {
            List = new List<float>();
            Multiplier = 1;
        }

        private void OnValidate() => Multiplier = List.Aggregate(1f, (acc, val) => acc * val);

        public void AddMultiplier(float value)
        {
            List.Add(value);
            Multiplier = TotalMultiplier * value;
        }

        public async void AddTemporaryMultiplier(float value, float durationInSeconds)
        {
            AddMultiplier(value);
            await Task.Delay((int)(durationInSeconds * 1000));
            RemoveMultiplier(value);
        }

        private void RemoveMultiplier(float value)
        {
            List.Remove(value);
            Multiplier = TotalMultiplier / value;
        }
    }
}
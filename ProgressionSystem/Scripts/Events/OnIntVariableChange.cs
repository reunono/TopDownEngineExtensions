using ProgressionSystem.Scripts.Variables;
using UnityEngine;
using UnityEngine.Events;

namespace ProgressionSystem.Scripts.Events
{
    public class OnIntVariableChange : MonoBehaviour
    {
        [SerializeField] private IntVariable IntVariable;
        [SerializeField] private UnityEvent OnIntVariableChangeEvent;
        private void OnEnable() { IntVariable.Changed += OnIntVariableChangeEvent.Invoke; }
        private void OnDisable() { IntVariable.Changed -= OnIntVariableChangeEvent.Invoke; }
    }
}

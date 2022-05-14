using System.Globalization;
using ProgressionSystem.Scripts.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace ProgressionSystem.Scripts.UI
{
    public class FloatVariableText : MonoBehaviour
    {
        [SerializeField] private FloatVariable FloatVariable;
        [SerializeField] private float Offset;
        [SerializeField] private string Prefix = "Walk Speed: ";
        private Text _text;

        private void Awake() { _text = GetComponent<Text>(); }
        private void UpdateText() { _text.text = Prefix + (FloatVariable.Value+Offset).ToString("F", CultureInfo.InvariantCulture);}

        private void OnEnable()
        {
            UpdateText();
            FloatVariable.Changed += UpdateText;
        }
        private void OnDisable() { FloatVariable.Changed -= UpdateText; }
    }
}

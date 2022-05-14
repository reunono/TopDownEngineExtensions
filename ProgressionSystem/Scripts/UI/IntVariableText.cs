using ProgressionSystem.Scripts.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace ProgressionSystem.Scripts.UI
{
    public class IntVariableText : MonoBehaviour
    {
        [SerializeField] private IntVariable IntVariable;
        [SerializeField] private int Offset;
        [SerializeField] private string Prefix = "LVL ";
        private Text _text;

        private void Awake() { _text = GetComponent<Text>(); }
        private void UpdateText() { _text.text = Prefix + (IntVariable.Value+Offset);}

        private void OnEnable()
        {
            UpdateText();
            IntVariable.Changed += UpdateText;
        }
        private void OnDisable() { IntVariable.Changed -= UpdateText; }
    }
}

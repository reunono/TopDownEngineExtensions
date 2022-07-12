using MoreMountains.Tools;
using ProgressionSystem.Scripts.Core;
using UnityEngine;

namespace ProgressionSystem.Scripts.UI
{
    public class ExperienceProgressBar : MonoBehaviour
    {
        [SerializeField] private Progression Progression;
        private MMProgressBar _bar;
        
        private void Awake() { _bar = GetComponent<MMProgressBar>(); }

        private void UpdateBar()
        {
            if (Progression.NextLevelExperience > 0)
                _bar.UpdateBar(Progression.LevelExperience, 0, Progression.NextLevelExperience);
            else
                _bar.SetBar01(1);
        }
        private void OnEnable()
        {
            _bar.SetBar(Progression.LevelExperience, 0, Progression.NextLevelExperience);
            Progression.Progressed += UpdateBar;
        }
        private void OnDisable() { Progression.Progressed -= UpdateBar; }
    }
}

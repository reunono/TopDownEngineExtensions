using MoreMountains.TopDownEngine;
using UnityEngine;

namespace ProgressionSystem
{
    public class GiveExperienceToPlayerOnDeath : MonoBehaviour
    {
        public int Experience = 50;
        private void Start()
        {
            var health = GetComponent<Health>();
            if (health != null) health.OnDeath += () => ExperienceEvent.Trigger(ExperienceEventType.Add, LevelManager.Instance.Players[0].gameObject, Experience);
        }
    }
}

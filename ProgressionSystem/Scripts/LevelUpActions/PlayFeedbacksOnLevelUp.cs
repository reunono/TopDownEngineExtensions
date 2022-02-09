using MoreMountains.Feedbacks;

namespace ProgressionSystem
{
    public class PlayFeedbacksOnLevelUp : DoSomethingOnLevelUp
    {
        public MMFeedbacks LevelUpFeedbacks;
        protected override void OnLevelUp(int level, int maxLevel)
        {
            LevelUpFeedbacks?.PlayFeedbacks(transform.position, level);
        }
    }
}

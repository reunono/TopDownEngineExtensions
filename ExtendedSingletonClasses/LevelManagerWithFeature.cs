using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// This LevelManager will extend the base class and provide new functionality
    /// </summary>
    public class LevelManagerWithFeature : LevelManager
    {
        protected override void Start()
        {
            Debug.Log("Start was called from extended LevelManagerWithFeature");
            base.Start();
        }

        public void NewFeature()
        {
            Debug.Log("New Feature was invoked in LevelManager");
        }
    }
}

using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// This GameManager will extend the base class and provide new functionality
    /// It also creates a new ExtendedInstance of the Singleton already cast as the new class
    /// </summary>
    public class GameManagerWithFeature : GameManager
    {
        // Static extension of Instance property which converts to new class
        public static GameManagerWithFeature ExtendedInstance => (GameManagerWithFeature) Instance;

        protected override void Start()
        {
            Debug.Log("Start was called from extended GameManagerWithFeature");
            base.Start();
        }

        public void NewFeature()
        {
            Debug.Log("New Feature was invoked in GameManager");
        }
    }
}

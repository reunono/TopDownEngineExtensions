using UnityEngine;
using MoreMountains.TopDownEngine;

public class ExampleInvokingNewFeatures : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Explicitly define the Singleton Instance from LevelManagerWithFeature as the LevelManagerWithFeature class
        // Then invoke the new feature
        // This is quite verbose and redundant, which is why GameManager has a new Singleton accessor
        ((LevelManagerWithFeature)LevelManagerWithFeature.Instance).NewFeature();

        // Access the Singleton Instance already cast inside the GameManagerWithFeature's ExtendedInstance member
        GameManagerWithFeature.ExtendedInstance.NewFeature();
    }
}
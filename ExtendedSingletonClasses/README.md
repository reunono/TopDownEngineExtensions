## Extending Singleton Managers

The Engine does a great job making its base classes extendable by virtue of all the methods being `virtual`.  One area that was a bit more difficult to extend was various classes in the `/Managers` folder.  All of the methods are `virtual`, but all the classes are inherit `MMPersistentSingleton` or `MMSingleton`.  They define their `Instance` property as the class type.  This causes some confusion when extending the various Manager classes and trying to continue the Singleton pattern.

For example, we can very easily create a class that extends GameManager and replace the base scripts with it
```C#
public class GameManagerWithFeature : GameManager
{
    public void NewFeature()
    {
        Debug.Log("New Feature was invoked in GameManager");
    }
}
```

Issues arise when trying to access the Instance of the new feature in a different script.  The following will not compile
```C#
public class ExampleInvokingNewFeatures : MonoBehaviour
{
    void Start()
    {
        GameManagerWithFeature.Instance.NewFeature();
    }
}
```

With C# explicit conversions we can access the new feature.  This can be seen by replacing the line in `Start()` with the following
```C#
((GameManagerWithFeature)GameManagerWithFeature).Instance.NewFeature();
```

The code above is verbose to use, and we could instead add a new property on the extended class that casts the Instance for us.  This would look like
```C#
public static GameManagerWithFeature ExtendedInstance => (GameManagerWithFeature) Instance;
```

With this we can replace the line in `Start()` with
```C#
GameManagerWithFeature.ExtendedInstance.NewFeature();
```

### How to use these extended classes

- Replace the `GameManager` script on the GameManager object with the `GameManagerWithFeature`
- Replace the `LevelManager` script on the LevelManager object with the `LevelManagerWithFeature`
- Create an empty game object and add the `ExampleInvokingNewFeature` script
- Run the scene and check the Console for the debug logs
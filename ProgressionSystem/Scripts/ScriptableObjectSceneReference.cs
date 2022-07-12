using UnityEngine;

// Scriptable objects that are not referenced in a scene are not loaded in a build
// so I wrote this to keep a scene reference of the scriptable objects
public class ScriptableObjectSceneReference : MonoBehaviour
{
    public ScriptableObject[] ScriptableObjects;
}

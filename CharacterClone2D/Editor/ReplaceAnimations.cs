using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ReplaceAnimations : EditorWindow
{
    [MenuItem("Assets/Replace Animations", true)]
    private static bool ReplaceAnimationsValidation()
    {
        return Selection.activeObject is AnimatorOverrideController || Selection.activeObject is AnimatorController;
    }

    [MenuItem("Assets/Replace Animations")]
    private static void ReplaceAnimationsOption()
    {
        if (Selection.activeObject is AnimatorOverrideController overrideController)
        {
            Debug.Log("Selected Animator Override Controller: " + overrideController.name);
            ReplaceAnimationsInController(overrideController);
        }
        else if (Selection.activeObject is AnimatorController animatorController)
        {
            Debug.Log("Selected Animator Controller: " + animatorController.name);
            ReplaceAnimationsInAnimatorController(animatorController);
        }

        Debug.Log("Animation replacement process completed.");
    }

    public static void ReplaceAnimationsInController(AnimatorOverrideController overrideController)
    {
        string path = AssetDatabase.GetAssetPath(overrideController);
        Debug.Log("Path of the selected controller: " + path);

        string directory = Path.GetDirectoryName(path);
        Debug.Log("Directory of the controller: " + directory);

        var allAnimationClips = GetAllAnimationClipsInDirectory(directory);

        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrideController.GetOverrides(overrides);
        Debug.Log("Number of overrides in controller: " + overrides.Count);

        foreach (var pair in overrides)
        {
            if (pair.Value != null)
            {
                AnimationClip newClip = allAnimationClips.Find(clip => clip.name == pair.Value.name);
                if (newClip != null)
                {
                    Debug.Log("Replacing " + pair.Value.name + " with " + newClip.name);
                    overrideController[pair.Key.name] = newClip;
                }
                else
                {
                    Debug.Log("No matching animation found for: " + pair.Value.name);
                }
            }
            else
            {
                Debug.Log("Override is empty for: " + pair.Key.name + ". No replacement made.");
            }
        }
    }

    public static void ReplaceAnimationsInAnimatorController(AnimatorController animatorController)
    {
        string path = AssetDatabase.GetAssetPath(animatorController);
        string directory = Path.GetDirectoryName(path);
        var allAnimationClips = GetAllAnimationClipsInDirectory(directory);

        foreach (var layer in animatorController.layers)
        {
            ReplaceAnimationsInStateMachine(layer.stateMachine, allAnimationClips);
        }
    }

    private static void ReplaceAnimationsInStateMachine(AnimatorStateMachine stateMachine, List<AnimationClip> allAnimationClips)
    {
        foreach (var state in stateMachine.states)
        {
            Motion motion = state.state.motion;
            if (motion is AnimationClip clip)
            {
                AnimationClip newClip = allAnimationClips.Find(c => c.name == clip.name);
                if (newClip != null)
                {
                    Debug.Log("Replacing " + clip.name + " with " + newClip.name);
                    state.state.motion = newClip;
                }
            }
        }

        foreach (var subStateMachine in stateMachine.stateMachines)
        {
            ReplaceAnimationsInStateMachine(subStateMachine.stateMachine, allAnimationClips);
        }
    }

    private static List<AnimationClip> GetAllAnimationClipsInDirectory(string directory)
    {
        var allAnimationClips = new List<AnimationClip>();
        var files = Directory.GetFiles(directory);

        foreach (var file in files)
        {
            var asset = AssetDatabase.LoadAssetAtPath<AnimationClip>(file);
            if (asset != null)
            {
                allAnimationClips.Add(asset);
            }
        }

        Debug.Log("Number of Animation Clips found: " + allAnimationClips.Count);
        return allAnimationClips;
    }
}

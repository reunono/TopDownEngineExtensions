using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SpriteReplacer : EditorWindow
{
    private string animationsFolderPath = "";
    private string spritesFolderPath = "";

    [MenuItem("Tools/Sprite Replacer")]
    public static void ShowWindow()
    {
        GetWindow<SpriteReplacer>("Sprite Replacer");
    }

    void OnGUI()
    {
        GUILayout.Label("Replace Sprites in Animation Clips", EditorStyles.boldLabel);

        if (GUILayout.Button("Select Animations Folder"))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Folder with Animation Clips", "", "");
            animationsFolderPath = ConvertToRelativePath(selectedPath);
            Debug.Log("Selected Animations Folder: " + animationsFolderPath);
        }

        if (GUILayout.Button("Select Sprites Folder"))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Folder with Sprites", "", "");
            spritesFolderPath = ConvertToRelativePath(selectedPath);
            Debug.Log("Selected Sprites Folder: " + spritesFolderPath);
        }

        if (!string.IsNullOrEmpty(animationsFolderPath) && !string.IsNullOrEmpty(spritesFolderPath))
        {
            EditorGUILayout.LabelField("Animations Folder:", animationsFolderPath);
            EditorGUILayout.LabelField("Sprites Folder:", spritesFolderPath);

            if (GUILayout.Button("Replace Sprites"))
            {
                Debug.Log("Replacing Sprites in Animation Clips...");
                ReplaceSpritesInFolder(animationsFolderPath, spritesFolderPath);
            }
        }
    }

    private string ConvertToRelativePath(string absolutePath)
    {
        if (!string.IsNullOrEmpty(absolutePath))
        {
            if (absolutePath.StartsWith(Application.dataPath))
            {
                return "Assets" + absolutePath.Substring(Application.dataPath.Length);
            }
            else
            {
                Debug.LogWarning("Selected folder is outside of the Assets folder.");
                return string.Empty;
            }
        }
        return string.Empty;
    }

    public static void ReplaceSpritesInFolder(string animationsFolder, string spritesFolder)
    {
        if (Directory.Exists(animationsFolder))
        {
            var spriteDict = LoadSprites(spritesFolder);
            Debug.Log("Loaded " + spriteDict.Count + " Sprites");

            string[] animationPaths = Directory.GetFiles(animationsFolder, "*.anim", SearchOption.AllDirectories);

            foreach (string animationPath in animationPaths)
            {
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animationPath);

                if (clip != null)
                {
                    ReplaceSpritesInClip(clip, spriteDict);
                }
            }
        }
        else
        {
            Debug.LogWarning("Animations folder does not exist.");
        }
    }

    private static void ReplaceSpritesInClip(AnimationClip clip, Dictionary<string, Sprite> spriteDict)
    {
        foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
        {
            ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);
            List<ObjectReferenceKeyframe> newKeyframes = new List<ObjectReferenceKeyframe>();

            foreach (var frame in keyframes)
            {
                Sprite sprite = frame.value as Sprite;
                if (sprite != null && spriteDict.TryGetValue(sprite.name, out Sprite newSprite))
                {
                    newKeyframes.Add(new ObjectReferenceKeyframe { time = frame.time, value = newSprite });
                }
                else
                {
                    newKeyframes.Add(frame);
                }
            }

            AnimationUtility.SetObjectReferenceCurve(clip, binding, newKeyframes.ToArray());
        }

        Debug.Log("Replaced Sprites in Animation: " + clip.name);
    }

    private static Dictionary<string, Sprite> LoadSprites(string path)
    {
        Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        var spritePaths = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);

        foreach (string spritePath in spritePaths)
        {
            string assetPath = spritePath.Replace("\\", "/").Replace(Application.dataPath, "Assets");
            Sprite[] loadedSprites = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Sprite>().ToArray();
            foreach (Sprite sprite in loadedSprites)
            {
                sprites[sprite.name] = sprite;
            }
        }

        return sprites;
    }
}

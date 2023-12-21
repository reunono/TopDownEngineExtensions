using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor.Animations;

public class CloneCharacter : EditorWindow
{
    [MenuItem("Assets/Clone Character", true)]
    private static bool CloneCharacterValidation()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        return Directory.Exists(path); // Validar si es un directorio
    }

    [MenuItem("Assets/Clone Character")]
    private static void CloneCharacterOption()
    {
        string originalFolderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        string clonedFolderPath = originalFolderPath + "_Cloned"; // Define el nombre de la carpeta clonada

        // Clonar la carpeta
        AssetDatabase.CopyAsset(originalFolderPath, clonedFolderPath);
        AssetDatabase.Refresh();

        // Reemplazar animaciones y sprites
        ReplaceAnimationsInClonedFolder(clonedFolderPath);
        ReplaceSpritesInClonedFolder(clonedFolderPath);

        // Actualizar el prefab
        UpdatePrefabInClonedFolder(clonedFolderPath);

        Debug.Log("Character cloned and updated successfully.");
    }

    private static void ReplaceAnimationsInClonedFolder(string clonedFolderPath)
    {
        string animationsFolderPath = Path.Combine(clonedFolderPath, "animations");
        string[] overrideControllerFiles = Directory.GetFiles(animationsFolderPath, "*.overrideController", SearchOption.AllDirectories);
        string[] animatorControllerFiles = Directory.GetFiles(animationsFolderPath, "*.controller", SearchOption.AllDirectories);

        foreach (string file in overrideControllerFiles)
        {
            AnimatorOverrideController overrideController = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(file);
            if (overrideController != null)
            {
                ReplaceAnimations.ReplaceAnimationsInController(overrideController);
            }
        }

        foreach (string file in animatorControllerFiles)
        {
            AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(file);
            if (animatorController != null)
            {
                ReplaceAnimations.ReplaceAnimationsInAnimatorController(animatorController);
            }
        }
    }

    private static void ReplaceSpritesInClonedFolder(string clonedFolderPath)
    {
        string animationsFolderPath = Path.Combine(clonedFolderPath, "animations");
        string spritesFolderPath = Path.Combine(clonedFolderPath, "sprites");

        SpriteReplacer.ReplaceSpritesInFolder(animationsFolderPath, spritesFolderPath);
    }

    private static void UpdatePrefabInClonedFolder(string clonedFolderPath)
    {
        Debug.Log("Updating prefab in cloned folder: " + clonedFolderPath);
        string prefabPath = FindPrefabInFolder(clonedFolderPath);

        if (!string.IsNullOrEmpty(prefabPath))
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            Debug.Log("Loaded prefab: " + prefabPath);

            if (prefab != null)
            {
                GameObject model = prefab.transform.Find("Model")?.gameObject;
                if (model != null)
                {
                    Debug.Log("Found 'Model' GameObject in prefab.");

                    // Actualizar Sprite Renderer
                    UpdateSpriteRenderer(model, clonedFolderPath);

                    // Actualizar Animator
                    UpdateAnimator(model, clonedFolderPath);
                }
                else
                {
                    Debug.LogWarning("Model GameObject not found in prefab.");
                }

                // Guardar cambios en el prefab
                EditorUtility.SetDirty(prefab);
                AssetDatabase.SaveAssets();
                Debug.Log("Prefab updated and changes saved.");
            }
            else
            {
                Debug.LogWarning("Prefab could not be loaded from path: " + prefabPath);
            }
        }
        else
        {
            Debug.LogWarning("No prefab found in folder: " + clonedFolderPath);
        }
    }

    private static void UpdateSpriteRenderer(GameObject model, string clonedFolderPath)
    {
        SpriteRenderer spriteRenderer = model.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Debug.Log("Found SpriteRenderer component in 'Model'.");
            Sprite newSprite = FindSpriteInFolder(clonedFolderPath, spriteRenderer.sprite.name);

            if (newSprite != null)
            {
                Debug.Log("Replacing sprite: " + newSprite.name);
                spriteRenderer.sprite = newSprite;
            }
            else
            {
                Debug.LogWarning("New sprite not found: " + spriteRenderer.sprite.name);
            }
        }
    }

    private static void UpdateAnimator(GameObject model, string clonedFolderPath)
    {
        Animator animator = model.GetComponent<Animator>();
        if (animator != null)
        {
            Debug.Log("Found Animator component in 'Model'.");
            RuntimeAnimatorController currentController = animator.runtimeAnimatorController;
            string controllerPath = Path.Combine(clonedFolderPath, "animations", currentController.name);

            // Check and assign the appropriate controller type
            AnimatorOverrideController overrideController = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(controllerPath + ".overrideController");
            AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath + ".controller");

            if (overrideController != null)
            {
                Debug.Log("Replacing animator override controller with: " + overrideController.name);
                animator.runtimeAnimatorController = overrideController;
            }
            else if (animatorController != null)
            {
                Debug.Log("Replacing animator controller with: " + animatorController.name);
                animator.runtimeAnimatorController = animatorController;
            }
            else
            {
                Debug.LogWarning("New animator controller or override controller not found at path: " + controllerPath);
            }
        }
    }


    private static string FindPrefabInFolder(string folderPath)
    {
        Debug.Log("Searching for prefab in folder: " + folderPath);
        string[] prefabFiles = Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories);

        if (prefabFiles.Length > 0)
        {
            Debug.Log("Prefab found: " + prefabFiles[0]);
            return prefabFiles.FirstOrDefault();
        }
        else
        {
            Debug.LogWarning("No prefabs found in folder: " + folderPath);
            return null;
        }
    }
    private static Sprite FindSpriteInFolder(string folderPath, string spriteName)
    {
        Debug.Log("Searching for sprite: " + spriteName + " in folder: " + folderPath);
        var spritePaths = Directory.GetFiles(folderPath, "*.png", SearchOption.AllDirectories);
        foreach (string spritePath in spritePaths)
        {
            string assetPath = spritePath.Replace("\\", "/").Replace(Application.dataPath, "Assets");
            Sprite[] loadedSprites = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Sprite>().ToArray();
            foreach (Sprite sprite in loadedSprites)
            {
                if (sprite.name == spriteName)
                {
                    return sprite;
                }
            }
        }
        return null;
    }

}

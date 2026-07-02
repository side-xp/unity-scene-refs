using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEditor;

using SideXP.Core;
using SideXP.Core.EditorOnly;

namespace SideXP.SceneRefs.EditorOnly
{

    /// <summary>
    /// Utility functions for working with <see cref="SceneRefSO"/> in the editor.
    /// </summary>
    public static class SceneRefsEditorUtility
    {

        private const int MenuOrder = 201;
        private const string SceneMenu = "Assets/Create/Scene Ref Asset";
        private const string CreateMenu = "Assets/Create/" + Constants.CreateAssetMenu + "/Scene Ref Asset";

        /// <summary>
        /// Generates <see cref="SceneRefSO"/> assets for all the existing scenes in the project.
        /// </summary>
        /// <remarks>The new <see cref="SceneRefSO"/> assets are generated in the same folder as the scene they represent.</remarks>
        public static void GenerateAllSceneRefs()
        {
            // Collect the scenes that already have a ref once, so we don't rescan every ref for each scene (which would be O(n²)).
            HashSet<SceneAsset> referencedScenes = new HashSet<SceneAsset>();
            foreach (SceneRefSO sceneRef in ObjectUtility.FindAssets<SceneRefSO>(false))
            {
                if (sceneRef.SceneAsset != null)
                    referencedScenes.Add(sceneRef.SceneAsset);
            }

            foreach (SceneAsset sceneAsset in ObjectUtility.FindAssets<SceneAsset>())
            {
                string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                // Only generate refs for scenes inside this project's /Assets directory that don't already have one.
                if (!scenePath.StartsWith(PathUtility.AssetsDirectory))
                    continue;
                if (referencedScenes.Contains(sceneAsset))
                    continue;

                CreateSceneRefAsset(sceneAsset, scenePath);
            }
        }

        /// <inheritdoc cref="GenerateSceneRef(SceneAsset, bool)"/>
        public static bool GenerateSceneRef(SceneAsset sceneAsset)
        {
            return GenerateSceneRef(sceneAsset, false);
        }

        /// <summary>
        /// Gets an existing <see cref="SceneRefSO"/> asset that represents a given scene.
        /// </summary>
        /// <param name="sceneAsset">The scene of which to get the related <see cref="SceneRefSO"/> asset.</param>
        /// <returns>Returns the found <see cref="SceneRefSO"/> asset that represents the given scene.</returns>
        public static SceneRefSO GetSceneRef(SceneAsset sceneAsset)
        {
            // Check if a scene ref exists for the given scene
            foreach (SceneRefSO existingSceneRef in ObjectUtility.FindAssets<SceneRefSO>(false))
            {
                if (existingSceneRef.SceneAsset == sceneAsset)
                    return existingSceneRef;
            }
            return null;
        }

        /// <param name="sceneRef">Outputs the <see cref="SceneRefSO"/> asset that represents the given scene.</param>
        /// <returns>Returns true if a related <see cref="SceneRefSO"/> asset has been found.</returns>
        /// <inheritdoc cref="GetSceneRef(SceneAsset)"/>
        public static bool GetSceneRef(SceneAsset sceneAsset, out SceneRefSO sceneRef)
        {
            sceneRef = GetSceneRef(sceneAsset);
            return sceneRef != null;
        }

        /// <param name="sceneRef">The scene to check.</param>
        /// <inheritdoc cref="SceneEditorUtility.IsIncluded(string)"/>
        public static bool IsIncluded(this SceneRefSO sceneRef)
        {
            return SceneEditorUtility.IsIncluded(sceneRef.ScenePath);
        }

        /// <param name="sceneRef">The scene to check.</param>
        /// <inheritdoc cref="SceneEditorUtility.IsEnabled(string)"/>
        public static bool IsEnabled(this SceneRefSO sceneRef)
        {
            return SceneEditorUtility.IsEnabled(sceneRef.ScenePath);
        }

        /// <param name="sceneRef">The scene to add.</param>
        /// <inheritdoc cref="SceneEditorUtility.AddToBuildSettings(string)"/>
        public static bool AddToBuildSettings(this SceneRefSO sceneRef)
        {
            return SceneEditorUtility.AddToBuildSettings(sceneRef.ScenePath);
        }

        /// <param name="sceneRef">The scene to enable.</param>
        /// <inheritdoc cref="SceneEditorUtility.Enable(string)"/>
        public static bool Enable(this SceneRefSO sceneRef)
        {
            return SceneEditorUtility.Enable(sceneRef.ScenePath);
        }

        /// <param name="sceneRef">The scene to disable.</param>
        /// <inheritdoc cref="SceneEditorUtility.Disable(string)"/>
        public static bool Disable(this SceneRefSO sceneRef)
        {
            return SceneEditorUtility.Disable(sceneRef.ScenePath);
        }

        /// <param name="sceneRef">The scene to remove.</param>
        /// <inheritdoc cref="SceneEditorUtility.RemoveFromBuildSettings(string)"/>
        public static bool RemoveFromBuildSettings(this SceneRefSO sceneRef)
        {
            return SceneEditorUtility.RemoveFromBuildSettings(sceneRef.ScenePath);
        }

        /// <summary>
        /// Check if the <see cref="SceneRefSO"/> in the project are still valid:<br/>
        /// - Update the scene path of the ref asset if the related scene has been moved
        /// - Destroy the ref asset if the related scene has been deleted or is not valid
        /// </summary>
        internal static void ReloadSceneRefs()
        {
            // For each scene ref asset in the project
            foreach (SceneRefSO sceneRef in ObjectUtility.FindAssets<SceneRefSO>())
            {
                SceneAsset sceneAsset = sceneRef.SceneAsset;

                // Destroy the scene ref asset if the related scene has been deleted or is not valid.
                if (sceneAsset == null)
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(sceneRef));
                    continue;
                }

                string path = AssetDatabase.GetAssetPath(sceneAsset);

                // Update scene path if needed
                if (sceneRef.ScenePath != path)
                {
                    SerializedObject sceneRefObj = new SerializedObject(sceneRef);
                    sceneRefObj.FindProperty(SceneRefSO.ScenePathProp).stringValue = path;
                    sceneRefObj.ApplyModifiedPropertiesWithoutUndo();
                }

                if (sceneRef.name != sceneAsset.name)
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(sceneRef), sceneAsset.name);
            }
        }

        /// <summary>
        /// Generates a <see cref="SceneRefSO"/> asset for each selected scene.
        /// </summary>
        [MenuItem(SceneMenu, false, MenuOrder)]
        [MenuItem(CreateMenu, false)]
        private static void GenerateSelectedScenesRef()
        {
            foreach (Object obj in Selection.objects)
            {
                if (obj is not SceneAsset sceneAsset)
                    continue;

                GenerateSceneRef(sceneAsset);
            }
        }

        [MenuItem(SceneMenu, true, MenuOrder)]
        [MenuItem(CreateMenu, true)]
        private static bool GenerateSelectedScenesRefValidation()
        {
            foreach (Object obj in Selection.objects)
            {
                if (obj is SceneAsset sceneAsset)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Generates a <see cref="SceneRefSO"/> asset for a given <see cref="SceneAsset"/>.
        /// </summary>
        /// <param name="sceneAsset">The scene asset for which to create a runtime ref.</param>
        /// <param name="ignoreExisting">If enabled, this function won't log a message if a <see cref="SceneRefSO"/> asset already exists
        /// for the given scene.</param>
        /// <returns>Returns true if a <see cref="SceneRefSO"/> asset has been generated successfully.</returns>
        private static bool GenerateSceneRef(SceneAsset sceneAsset, bool ignoreExisting)
        {
            string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            // Cancel if the given scene is not contained in the /Assets directory of this project
            if (!scenePath.StartsWith(PathUtility.AssetsDirectory))
                return false;

            // Cancel if a scene ref asset already exists for the given scene
            if (GetSceneRef(sceneAsset, out SceneRefSO existingSceneRef))
            {
                if (!ignoreExisting)
                    Debug.Log($"A {nameof(SceneRefSO)} asset already exists for the scene \"{sceneAsset.name}\" at {AssetDatabase.GetAssetPath(existingSceneRef)}", existingSceneRef);
                return false;
            }

            CreateSceneRefAsset(sceneAsset, scenePath);
            return true;
        }

        /// <summary>
        /// Creates and saves a <see cref="SceneRefSO"/> asset next to a given scene, without checking whether one already exists.
        /// </summary>
        /// <param name="sceneAsset">The scene asset to create a ref for.</param>
        /// <param name="scenePath">The asset path of <paramref name="sceneAsset"/>.</param>
        /// <returns>Returns the created <see cref="SceneRefSO"/> asset.</returns>
        private static SceneRefSO CreateSceneRefAsset(SceneAsset sceneAsset, string scenePath)
        {
            SceneRefSO sceneRef = ScriptableObject.CreateInstance<SceneRefSO>();
            string path = Path.GetDirectoryName(scenePath);
            path = Path.Combine(path, $"{sceneAsset.name}.asset");
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            // Set serialized properties
            {
                SerializedObject sceneRefObj = new SerializedObject(sceneRef);
                sceneRefObj.FindProperty(SceneRefSO.SceneAssetProp).objectReferenceValue = sceneAsset;
                sceneRefObj.FindProperty(SceneRefSO.ScenePathProp).stringValue = scenePath;
                sceneRefObj.ApplyModifiedPropertiesWithoutUndo();
            }

            AssetDatabase.CreateAsset(sceneRef, path);
            Debug.Log($"{nameof(SceneRefSO)} asset created for scene {sceneAsset.name} at {path}", sceneRef);
            return sceneRef;
        }

    }

}
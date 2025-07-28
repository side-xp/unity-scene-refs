// MIT License
// Sideways Experiments (c) 2025
// https://sideways-experiments.com
// Author:	Sideways Experiments
// Contact:	dev@side-xp.com

using UnityEditor;

namespace SideXP.SceneRefs.EditorOnly
{

    /// <summary>
    /// Process <see cref="SceneRefSO"/> to ensure they all target a valid scene, even if that scene has been moved.
    /// </summary>
    internal class SceneRefsProcessor : AssetPostprocessor
    {

        /// <inheritdoc cref="AssetPostprocessor"/>
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            // Ensure existing scene refs
            SceneRefsEditorUtility.ReloadSceneRefs();

            // Generate missing scene refs
            if (SceneRefsEditorConfig.I.AutoGenerateSceneRefs)
            {
                foreach (string importedAsset in importedAssets)
                {
                    SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(importedAsset);
                    if (sceneAsset != null)
                        SceneRefsEditorUtility.GenerateSceneRef(sceneAsset);
                }
            }
        }

    }

}
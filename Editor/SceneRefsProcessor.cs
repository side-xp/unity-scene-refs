using System;

using UnityEditor;

namespace SideXP.SceneRefs.EditorOnly
{

    /// <summary>
    /// Process <see cref="SceneRefSO"/> to ensure they all target a valid scene, even if that scene has been moved.
    /// </summary>
    internal class SceneRefsProcessor : AssetPostprocessor
    {

        private const string SceneExtension = ".unity";

        /// <inheritdoc cref="AssetPostprocessor"/>
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            // Only reconcile existing scene refs when a scene was actually imported, moved or deleted. ReloadSceneRefs() scans every
            // SceneRefSO in the project, so running it on every unrelated import (textures, scripts, generated ref assets, ...) would be
            // wasteful and could cause needless reimport churn.
            if (ContainsScene(importedAssets) || ContainsScene(movedAssets) || ContainsScene(movedFromAssetPaths) || ContainsScene(deletedAssets))
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

        /// <summary>
        /// Checks whether the given list of asset paths contains at least one scene asset.
        /// </summary>
        private static bool ContainsScene(string[] assetPaths)
        {
            foreach (string path in assetPaths)
            {
                if (path.EndsWith(SceneExtension, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

    }

}
// MIT License
// Sideways Experiments (c) 2025
// https://sideways-experiments.com
// Author:	Sideways Experiments
// Contact:	dev@side-xp.com

using UnityEngine;
using UnityEditor;

using SideXP.Core;
using SideXP.Core.EditorOnly;

namespace SideXP.SceneRefs.EditorOnly
{

    /// <summary>
    /// Custom editor for <see cref="SceneAsset"/>.
    /// </summary>
    /// <remarks>THis will overwrite the custom editor from Core package, adding a button to generate a <see cref="SceneRefSO"/> asset for
    /// the selected <see cref="SceneAsset"/>.</remarks>
    [CustomEditor(typeof(SceneAsset))]
    public class SceneAssetEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SceneAsset sceneAsset = target as SceneAsset;

            using (new EnabledScope())
            {
                if (!sceneAsset.IsIncluded())
                {
                    EditorGUILayout.HelpBox("This scene is not included in Build Settings.", MessageType.Warning);
                    if (GUILayout.Button("Add Scene to Build Settings", GUI.skin.button.Bold().FontSizeDiff(3), MoreGUI.HeightLOpt))
                        sceneAsset.AddToBuildSettings();
                }
                else if (!sceneAsset.IsEnabled())
                {
                    EditorGUILayout.HelpBox("This scene is disabled in Build Settings.", MessageType.Warning);
                    if (GUILayout.Button("Enable Scene in Build Settings", GUI.skin.button.Bold().FontSizeDiff(3), MoreGUI.HeightLOpt))
                        sceneAsset.Enable();
                }
                else
                {
                    EditorGUILayout.HelpBox("This scene is included and enabled in Build Settings.", MessageType.Info);
                    if (GUILayout.Button("Disable Scene in Build Settings"))
                        sceneAsset.Disable();
                }

                EditorGUILayout.Space();
                if (!SceneRefsEditorUtility.GetSceneRef(sceneAsset, out SceneRefSO sceneRef))
                {
                    if (GUILayout.Button(new GUIContent("Generate Scene Ref Asset", "Generates a Scene Ref asset next to this original Scene asset, so you can reference it in your components instead of using names and ids.")))
                        SceneRefsEditorUtility.GenerateSceneRef(sceneAsset);
                }
                else
                {
                    if (GUILayout.Button("Locate Scene Ref Asset"))
                        EditorHelpers.FocusObject(sceneRef, true, false);
                }
            }
        }

    }

}
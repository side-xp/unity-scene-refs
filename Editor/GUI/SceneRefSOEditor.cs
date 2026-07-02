using UnityEngine;
using UnityEditor;

using SideXP.Core;

namespace SideXP.SceneRefs.EditorOnly
{

    /// <summary>
    /// Custom editor for <see cref="SceneRefSO"/>.
    /// </summary>
    [CustomEditor(typeof(SceneRefSO))]
    public class SceneRefSOEditor : Editor
    {

        #region Fields

        private SerializedProperty _sceneAssetProp = null;
        private SerializedProperty _scenePathProp = null;

        #endregion


        #region Lifecycle

        private void OnEnable()
        {
            _sceneAssetProp = serializedObject.FindProperty(SceneRefSO.SceneAssetProp);
            _scenePathProp = serializedObject.FindProperty(SceneRefSO.ScenePathProp);
        }

        #endregion


        #region UI

        public override void OnInspectorGUI()
        {
            using (new EnabledScope(false))
            {
                EditorGUILayout.PropertyField(_sceneAssetProp);
                EditorGUILayout.PropertyField(_scenePathProp);
            }
            EditorGUILayout.Space();

            SceneRefSO sceneRefAsset = target as SceneRefSO;

            if (!sceneRefAsset.IsIncluded())
            {
                EditorGUILayout.HelpBox("The referenced scene is not included in Build Settings.", MessageType.Warning);
                if (GUILayout.Button("Add Scene to Build Settings", GUI.skin.button.Bold().FontSizeDiff(3), MoreGUI.HeightLOpt))
                    sceneRefAsset.AddToBuildSettings();
            }
            else if (!sceneRefAsset.IsEnabled())
            {
                EditorGUILayout.HelpBox("The referenced scene is disabled in Build Settings.", MessageType.Warning);
                if (GUILayout.Button("Enable Scene in Build Settings", GUI.skin.button.Bold().FontSizeDiff(3), MoreGUI.HeightLOpt))
                    sceneRefAsset.Enable();
            }
            else
            {
                EditorGUILayout.HelpBox("The referenced scene is included and enabled in Build Settings.", MessageType.Info);
                if (GUILayout.Button("Disable Scene in Build Settings"))
                    sceneRefAsset.Disable();
            }
        }

        #endregion

    }

}
using UnityEditor;

using SideXP.Core.EditorOnly;

namespace SideXP.SceneRefs.EditorOnly
{

    /// <summary>
    /// Generate menus to edit the Scene Refs package editor settings.
    /// </summary>
    public static class SceneRefsEditorConfigProvider
    {

        /// <summary>
        /// Registers the page to edit the Scene Refs package settings from the Project Settings window.
        /// </summary>
        /// <returns>Returns the created settings provider.</returns>
        [SettingsProvider]
        private static SettingsProvider RegisterProjectSettingsMenu()
        {
            return EditorConfigSettingsProvider.Create<SceneRefsEditorConfig>(
                EditorConstants.ProjectSettings + "/Scene Refs",
                SettingsScope.Project
            );
        }

    }

}

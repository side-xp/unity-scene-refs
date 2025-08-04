using UnityEditor;

using SideXP.Core.EditorOnly;

namespace SideXP.SceneRefs.EditorOnly
{

    /// <summary>
    /// Generate menus to edit the Scene Refs package editor settings.
    /// </summary>
    public class SceneRefsEditorSettingsProvider : DefaultConfigSettingsProvider
    {

        [SettingsProvider]
        private static SettingsProvider RegisterProjectSettingsMenu()
        {
            return MakeSettingsProvider(SceneRefsEditorConfig.I, EditorConstants.ProjectSettings + "/Scene Refs", SettingsScope.Project);
        }

    }

}

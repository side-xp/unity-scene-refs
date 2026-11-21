using UnityEngine;

using SideXP.Core;
using SideXP.Core.EditorOnly;

namespace SideXP.SceneRefs.EditorOnly
{

    /// <summary>
    /// Editor config for the Scene Refs package features.
    /// </summary>
    [EditorConfig(EEditorConfigScope.Project)]
    public class SceneRefsEditorConfig : EditorConfig<SceneRefsEditorConfig>
    {

        #region Fields

        [SerializeField]
        [Boolton(nameof(AutoGenerateSceneRefs), Toggle = true)]
        [Tooltip("If enabled, " + nameof(SceneRefSO) + " assets will automatically be created for all existing scenes and all the ones created in the future. You can use these assets to avoid using names or indexes directly for loading/unloading scenes, even at runtime.")]
        private bool _autoGenerateSceneRefs = false;

        #endregion


        #region Lifecycle

        /// <inheritdoc/>
        protected override void OnLoaded()
        {
            AutoGenerateSceneRefs = _autoGenerateSceneRefs;
        }

        #endregion


        #region Public API

        /// <inheritdoc cref="_autoGenerateSceneRefs"/>
        public bool AutoGenerateSceneRefs
        {
            get => _autoGenerateSceneRefs;
            set
            {
                _autoGenerateSceneRefs = value;
                if (_autoGenerateSceneRefs)
                    SceneRefsEditorUtility.GenerateAllSceneRefs();
            }
        }

        #endregion

    }

}

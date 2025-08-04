using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace SideXP.SceneRefs
{

    /// <summary>
    /// Represents a scene in the project at runtime.
    /// </summary>
    [HelpURL(Constants.BaseHelpUrl)]
    public class SceneRefSO : ScriptableObject
    {

        #region Fields

#if UNITY_EDITOR
        /// <summary>
        /// In the editor, we store the reference to the actual scene asset represented by this asset. It makes things easier when it comes
        /// to check if the scene actually exists or has moved in the project. Everything here is private to avoid unintended usage.
        /// </summary>
        [SerializeField, HideInInspector]
        private UnityEditor.SceneAsset _sceneAsset = null;
        internal UnityEditor.SceneAsset SceneAsset => _sceneAsset;
#endif

        /// <summary>
        /// The path to the scene asset.
        /// </summary>
        [SerializeField, HideInInspector]
        private string _scenePath = string.Empty;

        #endregion


        #region Public API

        /// <inheritdoc cref="_scenePath"/>
        public string ScenePath => _scenePath;

        /// <summary>
        /// Gets the represented scene value.
        /// </summary>
        /// <remarks>This value is valid only if the scene is loaded.</remarks>
        public Scene Scene => SceneManager.GetSceneByPath(_scenePath);

        /// <summary>
        /// Gets the name of the represented scene.
        /// </summary>
        public string SceneName
        {
            get
            {
                Scene scene = Scene;
                return scene.IsValid() ? scene.name : Path.GetFileNameWithoutExtension(_scenePath);
            }
        }

        /// <summary>
        /// Gets the index of the represented scene.
        /// </summary>
        /// <remarks>This value is valid only if the scene is loaded.</remarks>
        public int SceneIndex => Scene.buildIndex;

        /// <summary>
        /// Checks if the represented scene is the active one.
        /// </summary>
        public bool IsActive
        {
            get
            {
                Scene scene = Scene;
                return scene.IsValid() && SceneManager.GetActiveScene() == scene;
            }
        }

        /// <summary>
        /// Checks if this scene is loaded and valid.
        /// </summary>
        public bool IsLoaded => Scene.IsValid();

        /// <inheritdoc cref="SceneManager.LoadScene(string)"/>
        public void Load()
        {
            SceneManager.LoadScene(_scenePath);
        }

        /// <inheritdoc cref="SceneManager.LoadScene(string, LoadSceneMode)"/>
        public void Load(LoadSceneMode mode)
        {
            SceneManager.LoadScene(_scenePath, mode);
        }

        /// <inheritdoc cref="SceneManager.LoadScene(string, LoadSceneParameters)"/>
        public Scene Load(LoadSceneParameters parameters)
        {
            return SceneManager.LoadScene(_scenePath, parameters);
        }

        /// <inheritdoc cref="SceneManager.LoadSceneAsync(string)"/>
        public AsyncOperation LoadAsync()
        {
            return SceneManager.LoadSceneAsync(_scenePath);
        }

        /// <inheritdoc cref="SceneManager.LoadSceneAsync(string, LoadSceneMode)"/>
        public AsyncOperation LoadAsync(LoadSceneMode mode)
        {
            return SceneManager.LoadSceneAsync(_scenePath, mode);
        }

        /// <inheritdoc cref="SceneManager.LoadSceneAsync(string, LoadSceneParameters)"/>
        public AsyncOperation LoadAsync(LoadSceneParameters parameters)
        {
            return SceneManager.LoadSceneAsync(_scenePath, parameters);
        }

        /// <inheritdoc cref="SceneManager.UnloadSceneAsync(string)"/>
        public AsyncOperation UnloadAsync()
        {
            return SceneManager.UnloadSceneAsync(_scenePath);
        }

        /// <inheritdoc cref="SceneManager.UnloadSceneAsync(string, UnloadSceneOptions)"/>
        public AsyncOperation UnloadAsync(UnloadSceneOptions options)
        {
            return SceneManager.UnloadSceneAsync(_scenePath, options);
        }

        #endregion

    }

}
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;

namespace SideXP.SceneRefs.Tests
{

    /// <summary>
    /// Tests for the runtime <see cref="SceneRefSO"/> API.
    /// </summary>
    /// <remarks>
    /// A real <c>.unity</c> scene can't be fabricated without opening a scene (which the test environment forbids), so these tests cover
    /// the behaviour of a ref whose scene is <em>not loaded</em> — most importantly the <see cref="SceneRefSO.SceneName"/> fallback that
    /// derives the name from the stored path. The refs are in-memory objects whose private <c>_scenePath</c> is seeded through a
    /// <see cref="SerializedObject"/> (the field name is exposed via the internal <see cref="SceneRefSO.ScenePathProp"/>). Scene loading
    /// and the <c>Load*/Unload*</c> forwarders to <see cref="UnityEngine.SceneManagement.SceneManager"/> are intentionally not exercised.
    /// </remarks>
    public class SceneRefSOTests
    {

        private const string ScenePath = "Assets/__SceneRefSOTests__/SampleLevel.unity";

        private readonly List<Object> _toDestroy = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (Object obj in _toDestroy)
            {
                if (obj != null)
                    Object.DestroyImmediate(obj);
            }
            _toDestroy.Clear();
        }

        /// <summary>
        /// Creates an in-memory <see cref="SceneRefSO"/> with the given stored scene path, tracked for teardown.
        /// </summary>
        private SceneRefSO NewSceneRef(string scenePath)
        {
            SceneRefSO sceneRef = ScriptableObject.CreateInstance<SceneRefSO>();
            _toDestroy.Add(sceneRef);

            SerializedObject serializedObject = new SerializedObject(sceneRef);
            serializedObject.FindProperty(SceneRefSO.ScenePathProp).stringValue = scenePath;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            return sceneRef;
        }

        [Test]
        public void ScenePath_ReturnsStoredPath()
        {
            Assert.AreEqual(ScenePath, NewSceneRef(ScenePath).ScenePath);
        }

        [Test]
        public void SceneName_SceneNotLoaded_ReturnsFileNameWithoutExtension()
        {
            // The scene is not loaded, so the name must be derived from the stored path.
            Assert.AreEqual("SampleLevel", NewSceneRef(ScenePath).SceneName);
        }

        [Test]
        public void SceneName_EmptyPath_ReturnsEmpty()
        {
            Assert.AreEqual(string.Empty, NewSceneRef(string.Empty).SceneName);
        }

        [Test]
        public void IsLoaded_SceneNotLoaded_ReturnsFalse()
        {
            Assert.IsFalse(NewSceneRef(ScenePath).IsLoaded);
        }

        [Test]
        public void Scene_SceneNotLoaded_ReturnsInvalidScene()
        {
            Assert.IsFalse(NewSceneRef(ScenePath).Scene.IsValid());
        }

        [Test]
        public void IsActive_SceneNotLoaded_ReturnsFalse()
        {
            Assert.IsFalse(NewSceneRef(ScenePath).IsActive);
        }

    }

}

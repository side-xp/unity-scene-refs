using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using UnityEditor;

using SideXP.SceneRefs.EditorOnly;

using Object = UnityEngine.Object;

namespace SideXP.SceneRefs.Tests
{

    /// <summary>
    /// Tests for <see cref="SceneRefsEditorUtility"/>.
    /// </summary>
    /// <remarks>
    /// The build-settings forwarders (<see cref="SceneRefsEditorUtility.IsIncluded"/>, <see cref="SceneRefsEditorUtility.Enable"/>, ...)
    /// carry no logic of their own: they pass the ref's <see cref="SceneRefSO.ScenePath"/> to the Core scene utility, which mutates the
    /// global <see cref="EditorBuildSettings.scenes"/>. So the original array is captured and restored around every test, and the refs are
    /// seeded with a synthetic path (no real <c>.unity</c> needed). The asset-generation logic (<c>GenerateSceneRef</c>/<c>ReloadSceneRefs</c>)
    /// needs a real scene asset to operate on and is only covered here through its null-input guard.
    /// </remarks>
    public class SceneRefsEditorUtilityTests
    {

        private const string SyntheticPath = "Assets/__SceneRefsEditorUtility_Synthetic__.unity";

        private EditorBuildSettingsScene[] _originalScenes;
        private readonly List<Object> _toDestroy = new List<Object>();

        [SetUp]
        public void SetUp()
        {
            _originalScenes = EditorBuildSettings.scenes;
        }

        [TearDown]
        public void TearDown()
        {
            EditorBuildSettings.scenes = _originalScenes;

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

        private static bool BuildSettingsContains(string path)
        {
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.path == path)
                    return true;
            }
            return false;
        }

        #region Build settings forwarders

        [Test]
        public void IsIncluded_NotIncluded_ReturnsFalse()
        {
            Assert.IsFalse(NewSceneRef(SyntheticPath).IsIncluded());
        }

        [Test]
        public void IsEnabled_NotIncluded_ReturnsFalse()
        {
            Assert.IsFalse(NewSceneRef(SyntheticPath).IsEnabled());
        }

        [Test]
        public void AddToBuildSettings_NewPath_AddsIncludedAndEnabled()
        {
            SceneRefSO sceneRef = NewSceneRef(SyntheticPath);

            Assert.IsTrue(sceneRef.AddToBuildSettings());
            Assert.IsTrue(sceneRef.IsIncluded());
            Assert.IsTrue(sceneRef.IsEnabled(), "A newly added scene should be enabled by default.");
            Assert.IsTrue(BuildSettingsContains(SyntheticPath), "The ref's path should have been forwarded to the build settings.");
        }

        [Test]
        public void AddToBuildSettings_AlreadyIncluded_ReturnsFalse()
        {
            SceneRefSO sceneRef = NewSceneRef(SyntheticPath);
            sceneRef.AddToBuildSettings();

            Assert.IsFalse(sceneRef.AddToBuildSettings());
        }

        [Test]
        public void DisableThenEnable_IncludedScene_TogglesEnabledState()
        {
            SceneRefSO sceneRef = NewSceneRef(SyntheticPath);
            sceneRef.AddToBuildSettings();

            Assert.IsTrue(sceneRef.Disable());
            Assert.IsFalse(sceneRef.IsEnabled());
            Assert.IsTrue(sceneRef.IsIncluded(), "Disabling should keep the scene included.");

            Assert.IsTrue(sceneRef.Enable());
            Assert.IsTrue(sceneRef.IsEnabled());
        }

        [Test]
        public void RemoveFromBuildSettings_Included_RemovesAndReturnsTrue()
        {
            SceneRefSO sceneRef = NewSceneRef(SyntheticPath);
            sceneRef.AddToBuildSettings();

            Assert.IsTrue(sceneRef.RemoveFromBuildSettings());
            Assert.IsFalse(sceneRef.IsIncluded());
        }

        [Test]
        public void RemoveFromBuildSettings_NotIncluded_ReturnsFalse()
        {
            Assert.IsFalse(NewSceneRef(SyntheticPath).RemoveFromBuildSettings());
        }

        #endregion


        #region Generation guards

        [Test]
        public void GenerateSceneRef_NullSceneAsset_ReturnsFalse()
        {
            // A null scene resolves to an empty asset path, which is outside /Assets, so nothing is generated.
            Assert.IsFalse(SceneRefsEditorUtility.GenerateSceneRef(null));
        }

        #endregion

    }

}

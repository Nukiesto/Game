// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2019 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.EditorHelpers.ScreenCapturing
{
    /// <summary>
    /// Screen capturer, can be started in play mode from "Window / LeopotamGroupLibrary / Capture screenshots..." menu.
    /// </summary>
    [ExecuteInEditMode]
    internal sealed class ScreenCapturer : MonoBehaviour
    {
        private static readonly List<Vector2Int> _resList = new List<Vector2Int> {
            new Vector2Int (800, 480),
            new Vector2Int (960, 640),
            new Vector2Int (1024, 768),
            new Vector2Int (1136, 640),
            new Vector2Int (1280, 800),
            new Vector2Int (1920, 1080),
            new Vector2Int (2048, 1536),
            new Vector2Int (2732, 2048),
        };

        private const string FileNameMask = "Screenshot_{0}_{1}x{2}.png";

        /// <summary>
        /// Start capturing from code.
        /// </summary>
        [MenuItem("Window/LeopotamGroupLibrary/Capture screenshots...")]
        public static void Capture()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("ScreenCapturer", "You can capture shots only at play mode!", "Close");
                return;
            }
            var go = new GameObject("_CAPTURER");
            go.hideFlags = HideFlags.HideAndDontSave;
            go.AddComponent<ScreenCapturer>();
        }

        private void Awake()
        {
            if (!Application.isPlaying)
            {
                FinishCapturing("ScreenCapturer can works only at play mode!");
            }
        }

        private IEnumerator Start()
        {
            var path = EditorUtility.SaveFolderPanel("Select target folder to screenshots", string.Empty, string.Empty);
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                FinishCapturing("Invalid path");
                yield break;
            }

            var platform = EditorUserBuildSettings.activeBuildTarget.ToString();
            var waiter = new WaitForEndOfFrame();
            var savedPreset = GameViewResolution.GetPreset();
            string err = null;
            int tempPreset;
            foreach (var res in _resList)
            {
                tempPreset = GameViewResolution.AddCustomPreset(res.x, res.y, "_capturer");
                GameViewResolution.SetPreset(tempPreset);
                yield return waiter;
                try
                {
                    ScreenCapture.CaptureScreenshot(
                        Path.Combine(path, string.Format(FileNameMask, platform, res.x, res.y)));
                }
                catch (Exception ex)
                {
                    err = ex.ToString();
                    break;
                }
                GameViewResolution.SetPreset(savedPreset);
                GameViewResolution.RemoveCustomPreset(tempPreset);
            }

            FinishCapturing(err);
        }

        private void FinishCapturing(string result)
        {
            var msg = string.Format("[ScreenCapturer] {0}", result ?? "Success");
            if (result != null)
            {
                Debug.LogWarning(msg);
            }
            else
            {
                Debug.Log(msg);
            }
            DestroyImmediate(gameObject);
        }

        private static class GameViewResolution
        {
            private static EditorWindow _gvWindow;

            private static PropertyInfo _gvSelectedPreset;

            private static PropertyInfo _gvsCurrentGroup;

            private static MethodInfo _gvRepaintAll;

            private static MethodInfo _gvgGetTotalCount;

            private static MethodInfo _gvgAddCustomSize;

            private static MethodInfo _gvgRemoveCustomSize;

            private static object _gvsInstance;

            private static ConstructorInfo _gvsCtor;

            static GameViewResolution()
            {
                var gvType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
                _gvSelectedPreset = gvType.GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.NonPublic);
                _gvRepaintAll = gvType.GetMethod("RepaintAll", BindingFlags.Static | BindingFlags.Public);
                _gvWindow = EditorWindow.GetWindow(gvType);

                var gvgType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeGroup");
                _gvgGetTotalCount = gvgType.GetMethod("GetTotalCount", BindingFlags.Instance | BindingFlags.Public);
                _gvgAddCustomSize = gvgType.GetMethod("AddCustomSize", BindingFlags.Instance | BindingFlags.Public);
                _gvgRemoveCustomSize = gvgType.GetMethod("RemoveCustomSize", BindingFlags.Instance | BindingFlags.Public);

                var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
                _gvsCurrentGroup = sizesType.GetProperty("currentGroup", BindingFlags.Instance | BindingFlags.Public);
                var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
                var instanceProp = singleType.GetProperty("instance");
                _gvsInstance = instanceProp.GetValue(null, null);

                var gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
                _gvsCtor = gvsType.GetConstructor(new[] { typeof(int), typeof(int), typeof(int), typeof(string) });
            }

            private static object GetCurrentGroup()
            {
                return _gvsCurrentGroup.GetValue(_gvsInstance, null);
            }

            public static int GetPreset()
            {
                return (int)_gvSelectedPreset.GetValue(_gvWindow, null);
            }

            public static void SetPreset(int id)
            {
                _gvSelectedPreset.SetValue(_gvWindow, id, null);
                _gvRepaintAll.Invoke(null, null);
            }

            public static int AddCustomPreset(int width, int height, string text)
            {
                var grp = GetCurrentGroup();
                var newSize = _gvsCtor.Invoke(new object[] { 1, width, height, text });
                _gvgAddCustomSize.Invoke(grp, new[] { newSize });
                return (int)_gvgGetTotalCount.Invoke(grp, null) - 1;
            }

            public static void RemoveCustomPreset(int id)
            {
                _gvgRemoveCustomSize.Invoke(GetCurrentGroup(), new object[] { id });
            }
        }
    }
}

#endif
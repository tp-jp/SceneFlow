using System.Linq;
using TpLab.SceneFlow.Editor.Pass;
using UnityEditor;
using UnityEngine;

namespace TpLab.SceneFlow.Editor.Tools
{
    /// <summary>
    /// SceneFlow のデバッグ用ツールウィンドウ
    /// 登録されている Pass の一覧と実行順序を表示
    /// </summary>
    public class SceneFlowDebugWindow : EditorWindow
    {
        Vector2 _scrollPosition;

        [MenuItem("Tools/SceneFlow/Debug Window")]
        static void Open()
        {
            GetWindow<SceneFlowDebugWindow>("SceneFlow Debug");
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("SceneFlow Pass Debug", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // BuildPass
            DrawPassSection("Build Passes", PassDiscovery.DiscoverBuildPasses().ToList());

            EditorGUILayout.Space();

            // ProjectPass
            DrawPassSection("Project Passes", PassDiscovery.DiscoverProjectPasses().ToList());

            EditorGUILayout.Space();

            // ScenePass
            DrawPassSection("Scene Passes", PassDiscovery.DiscoverScenePasses().ToList());

            EditorGUILayout.EndScrollView();
        }

        void DrawPassSection<T>(string title, System.Collections.Generic.List<T> passes)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            if (passes.Count == 0)
            {
                EditorGUILayout.LabelField("  (No passes found)", EditorStyles.miniLabel);
                return;
            }

            EditorGUILayout.LabelField($"  Total: {passes.Count}", EditorStyles.miniLabel);

            try
            {
                // 依存関係でソート
                var sorted = typeof(T) == typeof(IBuildPass)
                    ? PassSorter.Sort(passes.Cast<IBuildPass>(), p => p.RunAfter, p => p.RunBefore).Cast<T>().ToList()
                    : typeof(T) == typeof(IProjectPass)
                        ? PassSorter.Sort(passes.Cast<IProjectPass>(), p => p.RunAfter, p => p.RunBefore).Cast<T>().ToList()
                        : PassSorter.Sort(passes.Cast<IScenePass>(), p => p.RunAfter, p => p.RunBefore).Cast<T>().ToList();

                for (int i = 0; i < sorted.Count; i++)
                {
                    var pass = sorted[i];
                    EditorGUILayout.LabelField($"  {i + 1}. {pass.GetType().Name}");

                    // 依存関係を表示
                    var runAfter = GetRunAfter(pass);
                    var runBefore = GetRunBefore(pass);

                    if (runAfter.Any())
                    {
                        EditorGUILayout.LabelField($"     → After: {string.Join(", ", runAfter.Select(t => t.Name))}", EditorStyles.miniLabel);
                    }

                    if (runBefore.Any())
                    {
                        EditorGUILayout.LabelField($"     → Before: {string.Join(", ", runBefore.Select(t => t.Name))}", EditorStyles.miniLabel);
                    }
                }
            }
            catch (System.Exception ex)
            {
                EditorGUILayout.HelpBox($"Error sorting passes: {ex.Message}", MessageType.Error);
            }
        }

        System.Collections.Generic.IEnumerable<System.Type> GetRunAfter(object pass)
        {
            if (pass is IBuildPass buildPass) return buildPass.RunAfter;
            if (pass is IProjectPass projectPass) return projectPass.RunAfter;
            if (pass is IScenePass scenePass) return scenePass.RunAfter;
            return System.Linq.Enumerable.Empty<System.Type>();
        }

        System.Collections.Generic.IEnumerable<System.Type> GetRunBefore(object pass)
        {
            if (pass is IBuildPass buildPass) return buildPass.RunBefore;
            if (pass is IProjectPass projectPass) return projectPass.RunBefore;
            if (pass is IScenePass scenePass) return scenePass.RunBefore;
            return System.Linq.Enumerable.Empty<System.Type>();
        }
    }
}


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

        void DrawPassSection<T>(string sectionTitle, System.Collections.Generic.List<T> passes) where T : IPass
        {
            EditorGUILayout.LabelField(sectionTitle, EditorStyles.boldLabel);

            if (passes.Count == 0)
            {
                EditorGUILayout.LabelField("  (No passes found)", EditorStyles.miniLabel);
                return;
            }

            EditorGUILayout.LabelField($"  Total: {passes.Count}", EditorStyles.miniLabel);

            try
            {
                // 依存関係でソート
                var sorted = PassSorter.Sort(passes);

                for (var i = 0; i < sorted.Count; i++)
                {
                    var pass = sorted[i];
                    EditorGUILayout.LabelField($"  {i + 1}. {pass.GetType().Name}");

                    // 依存関係を表示（型ベース）
                    var runAfterTypes = pass.RunAfter.ToList();
                    var runBeforeTypes = pass.RunBefore.ToList();
                    
                    // 依存関係を表示（文字列ベース）
                    var runAfterNames = pass.RunAfterNames.ToList();
                    var runBeforeNames = pass.RunBeforeNames.ToList();

                    if (runAfterTypes.Any())
                    {
                        EditorGUILayout.LabelField($"     → After: {string.Join(", ", runAfterTypes.Select(t => t.Name))}", EditorStyles.miniLabel);
                    }

                    if (runAfterNames.Any())
                    {
                        EditorGUILayout.LabelField($"     → After (by name): {string.Join(", ", runAfterNames)}", EditorStyles.miniLabel);
                    }

                    if (runBeforeTypes.Any())
                    {
                        EditorGUILayout.LabelField($"     → Before: {string.Join(", ", runBeforeTypes.Select(t => t.Name))}", EditorStyles.miniLabel);
                    }

                    if (runBeforeNames.Any())
                    {
                        EditorGUILayout.LabelField($"     → Before (by name): {string.Join(", ", runBeforeNames)}", EditorStyles.miniLabel);
                    }
                }
            }
            catch (System.Exception ex)
            {
                EditorGUILayout.HelpBox($"Error sorting passes: {ex.Message}", MessageType.Error);
            }
        }
    }
}


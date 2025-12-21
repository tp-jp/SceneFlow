using System.Collections.Generic;
using System.Linq;
using TpLab.SceneFlow.Editor.Passes;
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
        string _searchFilter = "";
        readonly Dictionary<string, bool> _passFoldouts = new Dictionary<string, bool>();
        
        // キャッシュ
        List<IPass> _cachedPasses;
        bool _needsRefresh = true;

        [MenuItem("Tools/SceneFlow/Debug Window")]
        static void Open()
        {
            GetWindow<SceneFlowDebugWindow>("SceneFlow Debug");
        }

        void OnEnable()
        {
            _needsRefresh = true;
        }

        void OnGUI()
        {
            DrawToolbar();
            
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Pass リストの取得（キャッシュ使用）
            if (_needsRefresh || _cachedPasses == null)
            {
                _cachedPasses = PassDiscovery.DiscoverPasses<IPass>().ToList();
                _needsRefresh = false;
            }

            DrawPassList(_cachedPasses);

            EditorGUILayout.EndScrollView();
        }

        void DrawToolbar()
        {
            // タイトル
            EditorGUILayout.LabelField("SceneFlow Pass Debug", EditorStyles.boldLabel);
            
            // セパレーター
            var rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
            
            EditorGUILayout.Space(4);

            // 検索バー
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Search", GUILayout.Width(50));
            _searchFilter = EditorGUILayout.TextField(_searchFilter);
            if (GUILayout.Button("✕", GUILayout.Width(24)))
            {
                _searchFilter = "";
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);

            // コントロールバー
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.FlexibleSpace();
            
            // 右側：リフレッシュボタン
            if (GUILayout.Button("Expand All", GUILayout.Width(80)))
            {
                ExpandAll(true);
            }
            if (GUILayout.Button("Collapse All", GUILayout.Width(80)))
            {
                ExpandAll(false);
            }
            if (GUILayout.Button("↻", GUILayout.Width(30)))
            {
                _needsRefresh = true;
            }
            
            EditorGUILayout.EndHorizontal();
        }

        void DrawPassList(List<IPass> passes)
        {
            if (passes == null || passes.Count == 0)
            {
                EditorGUILayout.HelpBox("No passes found. Try refreshing or check your Pass implementations.", MessageType.Info);
                return;
            }

            try
            {
                // 依存関係でソート（全Passを対象にする）
                // フィルタリング時は警告を抑制（一部のPassが除外されるため）
                var suppressWarnings = !string.IsNullOrEmpty(_searchFilter);
                var sorted = PassSorter.Sort(passes, suppressWarnings);
                
                // ソート後に検索フィルター適用
                var filteredPasses = sorted;
                if (!string.IsNullOrEmpty(_searchFilter))
                {
                    filteredPasses = sorted.Where(p =>
                    {
                        var typeName = p.GetType().Name;
                        var fullName = p.GetType().FullName;
                        return typeName.IndexOf(_searchFilter, System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                               (fullName != null && fullName.IndexOf(_searchFilter, System.StringComparison.OrdinalIgnoreCase) >= 0);
                    }).ToList();
                }

                // ヘッダー情報
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField($"Pass Execution Order", EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField($"{filteredPasses.Count} of {passes.Count}", EditorStyles.miniLabel, GUILayout.Width(80));
                }

                if (filteredPasses.Count == 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox($"No passes match '{_searchFilter}'", MessageType.Info);
                    return;
                }

                EditorGUILayout.Space(4);

                DrawTableView(filteredPasses);
            }
            catch (System.Exception ex)
            {
                EditorGUILayout.HelpBox($"Error sorting passes: {ex.Message}", MessageType.Error);
            }
        }

        void DrawTableView(List<IPass> sortedPasses)
        {
            // テーブルヘッダー
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                EditorGUILayout.LabelField("#", EditorStyles.boldLabel, GUILayout.Width(35));
                EditorGUILayout.LabelField("Pass Name", EditorStyles.boldLabel, GUILayout.MinWidth(200));
                EditorGUILayout.LabelField("Dependencies", EditorStyles.boldLabel, GUILayout.MinWidth(150));
            }

            // テーブル行
            for (var i = 0; i < sortedPasses.Count; i++)
            {
                var pass = sortedPasses[i];
                DrawTableRow(pass, i + 1);
            }
        }

        void DrawTableRow(IPass pass, int index)
        {
            var passTypeName = pass.GetType().Name;
            var passKey = pass.GetType().FullName ?? pass.GetType().Name;
            
            // Pass ごとの折りたたみ状態を管理
            _passFoldouts.TryAdd(passKey, false);
            
            // 依存関係の有無を判定
            var hasDetails = pass.Dependencies.Any();
            
            // 背景色（交互）
            var bgColor = index % 2 == 0 
                ? new Color(0f, 0f, 0f, 0.05f) 
                : new Color(0f, 0f, 0f, 0f);
            
            // メイン行
            var rect = EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(20));
            EditorGUI.DrawRect(rect, bgColor);
            
            // インデックス
            EditorGUILayout.LabelField(index.ToString(), GUILayout.Width(35));
            
            // Pass名（展開可能な場合はクリック可能）
            if (hasDetails)
            {
                var foldoutIcon = _passFoldouts[passKey] ? "▼" : "▶";
                var clicked = GUILayout.Button($"{foldoutIcon} {passTypeName}", EditorStyles.label, GUILayout.MinWidth(200));
                
                if (clicked)
                {
                    _passFoldouts[passKey] = !_passFoldouts[passKey];
                }
            }
            else
            {
                EditorGUILayout.LabelField(passTypeName, GUILayout.MinWidth(200));
            }
            
            // 依存関係の要約または詳細表示の状態
            if (hasDetails && !_passFoldouts[passKey])
            {
                // 折りたたまれている場合は要約を表示
                var summary = GetDependencySummary(pass);
                EditorGUILayout.LabelField(summary, EditorStyles.miniLabel, GUILayout.MinWidth(150));
            }
            else if (hasDetails)
            {
                // 展開されている場合
                EditorGUILayout.LabelField("(click to collapse)", EditorStyles.miniLabel, GUILayout.MinWidth(150));
            }
            else
            {
                // 依存関係がない場合
                EditorGUILayout.LabelField("—", EditorStyles.miniLabel, GUILayout.MinWidth(150));
            }
            
            EditorGUILayout.EndHorizontal();
            
            // 詳細行（展開時のみ）
            if (hasDetails && _passFoldouts[passKey])
            {
                DrawDependencyDetailsInline(pass, bgColor);
            }
        }

        void DrawDependencyDetailsInline(IPass pass, Color bgColor)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                // 背景色を継続
                var rect = EditorGUILayout.BeginVertical();
                EditorGUI.DrawRect(rect, bgColor);
                
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(50); // インデント
                    
                    using (new EditorGUILayout.VerticalScope())
                    {
                        // 新API: Dependencies
                        var dependencies = pass.Dependencies.ToList();
                        if (dependencies.Any())
                        {
                            var afterDeps = dependencies
                                .Where(d => d.Relation == PassDependency.Direction.After)
                                .Select(d => d.IsTypeReference ? d.TargetType.Name : d.TargetTypeName)
                                .ToList();
                            var beforeDeps = dependencies
                                .Where(d => d.Relation == PassDependency.Direction.Before)
                                .Select(d => d.IsTypeReference ? d.TargetType.Name : d.TargetTypeName)
                                .ToList();
                            
                            if (afterDeps.Any())
                            {
                                EditorGUILayout.LabelField($"After: {string.Join(", ", afterDeps)}", EditorStyles.miniLabel);
                            }
                            if (beforeDeps.Any())
                            {
                                EditorGUILayout.LabelField($"Before: {string.Join(", ", beforeDeps)}", EditorStyles.miniLabel);
                            }
                        }
                    }
                }
                
                EditorGUILayout.EndVertical();
            }
        }

        string GetDependencySummary(IPass pass)
        {
            var parts = new List<string>();
            
            var dependencies = pass.Dependencies.ToList();
            var afterCount = dependencies.Count(d => d.Relation == PassDependency.Direction.After);
            var beforeCount = dependencies.Count(d => d.Relation == PassDependency.Direction.Before);
            
            if (afterCount > 0)
            {
                var afterNames = dependencies
                    .Where(d => d.Relation == PassDependency.Direction.After)
                    .Select(d => d.IsTypeReference ? d.TargetType.Name : d.TargetTypeName)
                    .Take(2);
                var afterText = string.Join(", ", afterNames);
                if (afterCount > 2) afterText += $"... (+{afterCount - 2})";
                parts.Add($"After: {afterText}");
            }
            
            if (beforeCount > 0)
            {
                var beforeNames = dependencies
                    .Where(d => d.Relation == PassDependency.Direction.Before)
                    .Select(d => d.IsTypeReference ? d.TargetType.Name : d.TargetTypeName)
                    .Take(2);
                var beforeText = string.Join(", ", beforeNames);
                if (beforeCount > 2) beforeText += $"... (+{beforeCount - 2})";
                parts.Add($"Before: {beforeText}");
            }
            
            return parts.Count > 0 ? string.Join(" | ", parts) : "—";
        }


        void ExpandAll(bool expand)
        {
            var keys = _passFoldouts.Keys.ToList();
            foreach (var key in keys)
            {
                _passFoldouts[key] = expand;
            }
            Repaint();
        }
    }
}